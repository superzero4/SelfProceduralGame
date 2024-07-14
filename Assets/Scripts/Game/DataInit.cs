using System;
using System.Collections.Generic;
using Game;
using RTSToolkitFree;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FinalData = Game.FinalData;

public class DataInit : MonoBehaviour
{
    [NonSerialized] public static FinalData Data;
    
    // String nom bâtiment donne 
    [NonSerialized] public static Dictionary<string, List<Unit>> UnitLookup = new();
    
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
        
        // Remplissage liste dépendences
        // Pour l'instant plusieurs dépendences non gérées

        for (int i = 0; i < Data.units.Length; i++)
        {
            // On fait quoi quand 0 dependencies ? Bâtiment par défaut ?
            if(Data.units[i].dependencies.Length == 0)
            {
                continue;
            }
            else
            {
                string buildingName = Data.units[i].dependencies[0];
                
                if (!UnitLookup.ContainsKey(buildingName))
                {
                    UnitLookup.Add(buildingName, new List<Unit>());
                }
                
                UnitLookup[buildingName].Add(Data.units[i]);
                // UnitLookup.Add(Data.units[i].dependencies[^1], Data.units[i]);
            }
            
        }
        
        for (var i = 0; i < Data.buildings.Length; i++)
        {
            var b = Data.buildings[i];
            int buildingIndex = i;
            Button newItem = Instantiate(buttonPrefab, buttonParent.transform).GetComponent<Button>();
            newItem.onClick.AddListener(() => cam.SpawnBuilding(buildingIndex));
            newItem.GetComponentInChildren<TMP_Text>().text = b.name;
            cam.buttons.Add(newItem);
        }

        cam.SelectedBuilding = Data.buildings[0];
        cam.UpdateButtons();
    }
}