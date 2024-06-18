using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Game;
using UnityEngine.Events;

public class ConstrainedChatter : ChatInteractor
{
    private bool ThemeOk(string value)
    {
        return _theme.Length < 30 && _theme.Trim(' ').Count(c => c == ' ') <= 2;
    }

    [SerializeField,
     ValidateInput(nameof(ThemeOk),
         "Consider shortening number of words and total length of them and adding more details in additional chatting, to avoid too many informations passed as system prompts",
         InfoMessageType.Error)]
    string _theme = "Cowboy";

    [SerializeField] private string _additionnalChatting =
        "I want my game to have a focus on vegetation, animals, nature and the wild west.";

    [SerializeField, Range(1, 100)] private int _nbUnit = 10;
    [SerializeField, Range(1, 100)] private int _nbBuilding = 20;
    [SerializeField, Range(1, 1000)] private int _asciiArtResolution = 50;
    AnswerHistory.HistoryEntry _last => _history.Get(0);
    public FinalData LastData => ReadHistory(0);
    public FinalData ReadHistory(int index) => _history.Get(index).data;

    [SerializeField, InlineEditor] private AnswerHistory _history;

    //Always accessible publicy through LastData and ReadHistory(0)
    public UnityEvent<FinalData> newAnswerTreated = new();
    [SerializeField, ReadOnly] private int currentChatStep = 0;

    protected override void OnAnswerReceived(string result)
    {
        if (currentChatStep == 0)
        {
            string json = ExtractJson(ExtractResultFromResponseString(result));
            var last = new AnswerHistory.HistoryEntry(_theme, json, result, Game.FinalData.FromJson(json));
            _history.Add(last);
            newAnswerTreated.Invoke(last.data);
            RequestArts();
        }
        else
        {
            var visualIndex = currentChatStep - 1;
            string visual = ExtractSubString("text = ", ", inlineData", result, false, false);
            var lengths = new int[] { _last.data.buildings.Length, _last.data.units.Length };
            (int arr, int ind) = distributedIndex(currentChatStep - 1, lengths);
            switch (arr)
            {
                default:
                    if (arr < 0) throw new System.Exception("Error in distributed index calculation");
                    else throw new NotSupportedException();
                case -1: return; //We finished
                case 0:
                    _last.data.buildings[ind].visualData.fallbackAsciiArt = visual;
                    break;
                case 1:
                    _last.data.units[ind].visualData.fallbackAsciiArt = visual;
                    break;
            }

            ind++;
            if (ind >= lengths[arr])
            {
                ind = 0;
                arr++;
                if (arr >= lengths.Length)
                    return; //We are done
            }

            var nextName = "";
            switch (arr)
            {
                default:
                    if (arr < 0) throw new System.Exception("Error in distributed index calculation");
                    else throw new NotSupportedException();
                case -1:
                    newAnswerTreated.Invoke(_last.data);
                    return; //We finished
                case 0:
                    nextName = _last.data.buildings[ind].name;
                    break;
                case 1:
                    nextName = _last.data.units[ind].name;
                    break;
                case 2:
                    throw new Exception("Not handled at all later with ");
                    break;
            }

            newAnswerTreated.Invoke(_last.data);
            RequestArts(nextName, arr == 0);
        }
    }

    private (int arr, int index) distributedIndex(int initialIndex, params int[] arrayLengths)
    {
        int index = initialIndex;
        for (int i = 0; i < arrayLengths.Length; i++)
        {
            if (index < arrayLengths[i])
                return (i, index);
            index -= arrayLengths[i];
        }

        return (-1, -1);
    }

    [Button]
    private string TestExtractJson(int history = 0)
    {
        return ExtractJson(_history.Get(history).raw);
    }

    [Button("Test creating an object from a json in the history")]
    private FinalData TestFromJson(int history = 0)
    {
        return FinalData.FromJson(_history.Get(history).json);
    }

    private string ExtractJson(string result)
    {
        return ExtractSubString("{", "}", result);
    }

    private string ExtractResultFromResponseString(string result)
    {
        return ExtractSubString("```", "```", result);
    }

    private static string BuildPrompt(string theme, int nbUnit, int nbBuilding)
    {
        var sample = JsonUtility.ToJson(FinalData.Sample());
        var baseMessage =
            $"I am building a Real Time Strategy video game, the theme is : {theme}, i want {nbUnit} units, and {nbBuilding} buildings with names and description.\n Both the units and the buildings may depends on some other buildings, identified by they name.\n" +
            $"can you generate me a JSON";
        return baseMessage + ", following this exact JSON structure : \n" + sample;
    }

    [Button, PropertyOrder(0)]
    private void TestPrompt()
    {
        SendConstrainedPrompt(_theme, _additionnalChatting);
    }

    [Button]
    private void FillWithArt()
    {
        //We consider we already have the data
        RequestArts();
    }

    /// <summary>
    /// The "in struct, in memory, codly usable"fill result will be under LastData property, check it after message has been received correctly
    /// </summary>
    /// <param name="theme"></param>
    /// <param name="additionnalFreeInformations"></param>
    public void SendConstrainedPrompt(string theme, string additionnalFreeInformations)
    {
        currentChatStep = 0;
        if (!ThemeOk(theme))
            Debug.LogError(
                "Theme isn't considered valid, check conditions for it, execution isn't guaranteed to be successful.");
        SubmitToChat("Can you do this for me :\n" + additionnalFreeInformations,
            BuildPrompt(theme, _nbUnit, _nbBuilding));
    }

    public void RequestArts()
    {
        var b = _last.data.buildings.Length > 0;
        RequestArts(b ? _last.data.buildings[0].name : _last.data.units[0].name,b);
    }

    public void RequestArts(string nextName, bool isBuilding)
    {
        var prompt = "Can you provide me an unique ASCII art for " + (isBuilding ? "building" : "unit")+" " + nextName;
        Debug.Log("Art requested for name : " + prompt);
        currentChatStep++;
        SubmitToChat(prompt,
            $"I want one and only one detailed ASCII art with at least {_asciiArtResolution}x{_asciiArtResolution} characters.");
    }
}