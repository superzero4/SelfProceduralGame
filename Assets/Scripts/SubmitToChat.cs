using GoogleApis.Example;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class SubmitToChat : MonoBehaviour
{
    [SerializeField] private string _prePrompt;
    [SerializeField] private TMP_Text output;
    private TMP_InputField _submit;
    private static BasicChatExample _chat;

    private void Awake()
    {
        _submit = GetComponent<TMP_InputField>();
        _submit.onSubmit.AddListener(ToChat);
        if (_chat == null)
        {
            _chat = gameObject.AddComponent<BasicChatExample>();
        }
    }

    private async void ToChat(string s)
    {
        string result = await _chat.SendRequest(s,_prePrompt);
        output.text = result;
        Debug.LogWarning($"Result : {result}");
    }
}