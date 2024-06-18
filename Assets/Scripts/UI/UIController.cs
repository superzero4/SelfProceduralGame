using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private ConstrainedChatter _chat;
    [SerializeField] private TileController _buildings, _units;

    [SerializeField] private bool _initWithLast = true;

    // Start is called before the first frame update
    void Start()
    {
        InitUIUsingHistory();
        _chat.newAnswerTreated.AddListener(UpdateUI);
    }
    
    [Button]
    void InitUIUsingHistory(int index = 0)
    {
        UpdateUI(_chat.ReadHistory(index));
    }
    void UpdateUI(FinalData data)
    {
        _buildings.UpdateTiles(data.buildings.Select(building => new TileController.VisualInformation()
        {
            name = building.name, description = building.description, visuals = building.visualData
        }));
        _units.UpdateTiles(data.units.Select(unit => new TileController.VisualInformation()
        {
            name = unit.name, description = unit.description, visuals = unit.visualData
        }));
    }
}