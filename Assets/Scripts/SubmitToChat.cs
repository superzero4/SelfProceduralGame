using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class SubmitToChat : ChatInteractor
{
    [SerializeField] private TMP_Text output;
    private TMP_InputField _submit;

    void Awake()
    {
        _submit = GetComponent<TMP_InputField>();
        _submit.onSubmit.AddListener(SubmitToChat);
    }

    protected override void OnAnswerReceived(string result)
    {
        output.text = result;
    }
}