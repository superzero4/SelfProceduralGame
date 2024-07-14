using System;
using Game;
using RTSToolkitFree;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FinalData = Game.FinalData;

public class DataInit : MonoBehaviour
{
    [NonSerialized] public static FinalData Data;
    
    [SerializeField] private AnswerHistory history;
    
    [SerializeField] private GameObject buttonParent;
    [SerializeField] private GameObject buttonPrefab;
    
    void Start()
    {
        CrossSceneInfo crossData = FindObjectOfType<CrossSceneInfo>();
        RTSCamera cam = FindObjectOfType<RTSCamera>();

        if (crossData != null)
        {
            Data = crossData.data;
        }
        else
        {
            Debug.LogWarning("Using history");
            Data = history.Get(0).data;
        }
        
        // Todo: construire liste de dépendences
        for (var i = 0; i < Data.buildings.Length; i++)
        {
            var b = Data.buildings[i];
            int buildingIndex = i;
            Button newItem = Instantiate(buttonPrefab, buttonParent.transform).GetComponent<Button>();
            newItem.onClick.AddListener(() => cam.SpawnBuilding(buildingIndex));
            newItem.GetComponentInChildren<TMP_Text>().text = b.name;
        }

        cam.SelectedBuilding = Data.buildings[0];
    }
}