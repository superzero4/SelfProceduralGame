using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AnswerHistory", menuName = "AnswerHistory", order = 0)]
public class AnswerHistory : ScriptableObject
{
    [SerializeField,
     ListDrawerSettings(ShowPaging = true, ShowIndexLabels = true, NumberOfItemsPerPage = 5,
         ListElementLabelName = nameof(HistoryEntry.info))]
    private List<HistoryEntry> _history;

    public void Add(HistoryEntry entry)
    {
        _history.Insert(0, entry);
    }

    public HistoryEntry Get(int index) => _history[index];

    [Serializable]
    public struct HistoryEntry
    {
        public HistoryEntry(string info, string json, string raw, Game.FinalData data)
        {
            this.info = info;
            this.json = json;
            this.raw = raw;
            this.data = data;
        }
        public FinalData data;

        [SerializeField] public string info;

        [SerializeField, TextArea(3, 20)] public string json;

        [SerializeField, TextArea(3, 10)] public string raw;

        public readonly void Deconstruct(out string json, out string raw, out FinalData data)
        {
            json = this.json;
            raw = this.raw;
            data = this.data;
        }
    }
}