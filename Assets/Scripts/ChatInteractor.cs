using GoogleApis.Example;
using UnityEngine;

public abstract class ChatInteractor : MonoBehaviour
{
    [SerializeField] private string _prePrompt;
    private static BasicChatExample _chat;

    private void Awake()
    {
        if (_chat == null)
            _chat = gameObject.AddComponent<BasicChatExample>();
    }

    protected abstract void OnAnswerReceived(string s);

    protected async void SubmitToChat(string s)
    {
        string result = await _chat.SendRequest(s,_prePrompt);
        Debug.LogWarning($"Result : {result}");
        OnAnswerReceived(result);
    }
}