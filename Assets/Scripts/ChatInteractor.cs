using GoogleApis.Example;
using UnityEngine;

public abstract class ChatInteractor : MonoBehaviour
{
    
    [SerializeField] private string _prePrompt;
    static BasicChatExample _chat;

    protected virtual void Awake()
    {
        if (_chat == null)
            _chat = gameObject.AddComponent<BasicChatExample>();
    }

    protected abstract void OnAnswerReceived(string s);

    protected async void SubmitToChat(string s)
    {
        SubmitToChat(s, _prePrompt);
    }

    protected async void SubmitToChat(string s, string prePrompt)
    {
        string result = await _chat.SendRequest(s, prePrompt);
        Debug.LogWarning($"Result : {result}");
        OnAnswerReceived(result);
    }

    protected string ExtractSubString(string beginningFlag, string endFlag, string origin, bool keepLastBegin = true,
        bool keepFirstEnd = true)
    {
        int index = origin.IndexOf(beginningFlag) + beginningFlag.Length - (keepLastBegin ? 1 : 0);
        int endindex = origin.LastIndexOf(endFlag);
        return origin.Substring(index, endindex - index + (keepFirstEnd ? 1 : 0));
    }
}