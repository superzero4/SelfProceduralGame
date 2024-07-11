using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private ConstrainedChatter _chat;
    [SerializeField] private TileController _buildings, _units;

    [SerializeField] private bool _initWithLast = true;

    [Header("UI references")] [SerializeField]
    private Slider _unitsCount;

    [SerializeField] private Slider _buildingCount;
    [SerializeField] private Button _start;
    [SerializeField] private Button _next;
    [SerializeField] private TMPro.TMP_InputField _inputText;
    [SerializeField] private TMPro.TMP_Text _infoText;
    private string _baseInfo;

    private void Awake()
    {
        _inputText.text = _chat.Random().info;
        _baseInfo = _start.GetComponentInChildren<TMPro.TMP_Text>().text;
        _baseInfo = _infoText.text;
        _start.onClick.AddListener(() => _chat.SendConstrainedPrompt(_inputText.text, ""));
        _unitsCount.onValueChanged.AddListener(f => _chat._nbUnit = (int)f);
        _unitsCount.onValueChanged.AddListener(f => UpdateInfo());
        _buildingCount.onValueChanged.AddListener(f => _chat._nbBuilding = (int)f);
        _buildingCount.onValueChanged.AddListener(f => UpdateInfo());
        _next.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
        _buildingCount.value = UnityEngine.Random.Range(0,(int)_buildingCount.maxValue);
        _unitsCount.value = UnityEngine.Random.Range(0,(int)_unitsCount.maxValue);
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        _infoText.text = _baseInfo + $" with {_chat._nbBuilding} buildings and {_chat._nbUnit} units";
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_initWithLast)
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