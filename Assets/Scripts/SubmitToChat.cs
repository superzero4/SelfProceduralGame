using System;
using System.Collections;
using GoogleApis.Example;
using GoogleApis.GenerativeLanguage;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMPro.TMP_InputField))]
public class SubmitToChat : MonoBehaviour
{
    [SerializeField] private string _prePrompt;
    private TMPro.TMP_InputField _submit;
    private static BasicChatExample _chat;

    private void Awake()
    {
        _submit = GetComponent<TMPro.TMP_InputField>();
        _submit.onSubmit.AddListener(ToChat);
        if (_chat == null)
        {
            _chat = gameObject.AddComponent<BasicChatExample>();
        }
    }

    private async void ToChat(string s)
    {
        string result = await _chat.SendRequest(s,_prePrompt);
        Debug.LogWarning($"Result : {result}");
    }
}