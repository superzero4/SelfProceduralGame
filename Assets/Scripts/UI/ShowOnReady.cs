using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnReady : MonoBehaviour
{
    [SerializeField] private ConstrainedChatter _chatter;
    private void Awake()
    {
        gameObject.SetActive(false);
        _chatter.allAnswerTreated.AddListener(f=>Show());
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
