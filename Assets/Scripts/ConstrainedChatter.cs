using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Game;

public class ConstrainedChatter : ChatInteractor
{
    private bool ThemeOk(string value)
    {
        return _theme.Length < 20 && _theme.Trim(' ').Count(c => c == ' ') <= 2;
    }

    [SerializeField,ValidateInput(nameof(ThemeOk),"Consider shortening number of words and total length of them and adding more details in additional chatting, to avoid too many informations passed as system prompts",InfoMessageType.Error)] string _theme = "Cowboy";
    [SerializeField] private string _additionnalChatting="I want my game to have a focus on vegetation, animals, nature and the wild west.";
    [SerializeField,Range(1,100)] private int _nbUnit = 10;
    [SerializeField,Range(1,100)] private int _nbBuilding = 20;
    private const string _unitsKey = "units";
    private const string _nameKey = "name";
    private const string _descriptionKey = "description";
    private const string _buildingsKey = "buildings";
    private const string _dependencyKey = "depends";
    [Header("Last")]
    AnswerHistory.HistoryEntry _last;
    public FinalData LastData => _last.data;
    [SerializeField,InlineEditor] private AnswerHistory _history;

    protected override void OnAnswerReceived(string result)
    {
        string json = ExtractJson(result);
        _last = new AnswerHistory.HistoryEntry(_theme,json, result, Game.FinalData.FromJson(json));
        _history.Add(_last);
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
        return ExtractSubString("{", "}", ExtractSubString("```","```",result));
    }

    private static string KeyPrompt(string name, string key)
    {
        return "\n-" + name + " : " + "\"" + key + "\"";
    }

    private static string BuildPrompt(string theme, int nbUnit, int nbBuilding,bool useSample = true)
    {
        var sample = JsonUtility.ToJson(FinalData.Sample());
        var baseMessage =
            $"I am building a Real Time Strategy video game, the theme is : {theme}, i want {nbUnit} units, and {nbBuilding} buildings with names and description.\n Both the units and the buildings may depends on some other buildings, identified by they name.\n" +
            $"can you generate me a JSON";
        return baseMessage + (useSample ? ", following this exact JSON structure : \n" + sample : 
             "with the game name and description, the buildings names, descriptions and dependencies and the units names, descriptions and dependencies.\n Use these keys : " +
            KeyPrompt("names", _nameKey) +
            KeyPrompt("descriptions", _descriptionKey) +
            KeyPrompt("units", _unitsKey) +
            KeyPrompt("buildings", _buildingsKey) +
            KeyPrompt("dependency key", _dependencyKey));
    }

    [Button,PropertyOrder(0)]
    private void TestPrompt()   
    {
        SendConstrainedPrompt(_theme,_additionnalChatting);
    }
    /// <summary>
    /// The "in struct, in memory, codly usable" result will be under LastData property, check it after message has been received correctly
    /// </summary>
    /// <param name="theme"></param>
    /// <param name="additionnalFreeInformations"></param>
    public void SendConstrainedPrompt(string theme,string additionnalFreeInformations)
    {
        if(!ThemeOk(theme))
            Debug.LogError("Theme isn't considered valid, check conditions for it, execution isn't guaranteed to be successful.");
        SubmitToChat(additionnalFreeInformations,BuildPrompt(theme, _nbUnit, _nbUnit));
    }
}