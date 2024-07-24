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

    [SerializeField,Range(-1,100),InfoBox("-1 for no init")] private int _initWithLast = 0;

    [Header("UI references")] [SerializeField]
    private Slider _unitsCount;

    [SerializeField] private Slider _buildingCount;
    [SerializeField] private Button _start;
    [SerializeField] private Button _next;
    [SerializeField] private TMPro.TMP_InputField _inputText;
    [SerializeField] private TMPro.TMP_InputField _altInputText;
    [SerializeField] private TMPro.TMP_Text _infoText;
    private string _baseInfo;

    private void Awake()
    {
        _inputText.text = _chat.Random().info;
        _baseInfo = _start.GetComponentInChildren<TMPro.TMP_Text>().text;
        _baseInfo = _infoText.text;
        _start.onClick.AddListener(() => _chat.SendConstrainedPrompt(_inputText.text, _altInputText.text));
        _start.onClick.AddListener(()=>_start.gameObject.SetActive(false));
        _unitsCount.onValueChanged.AddListener(f => _chat._nbUnit = (int)f);
        _unitsCount.onValueChanged.AddListener(f => UpdateInfo());
        _buildingCount.onValueChanged.AddListener(f => _chat._nbBuilding = (int)f);
        _buildingCount.onValueChanged.AddListener(f => UpdateInfo());
        _next.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1));
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
        if (_initWithLast>=0)
        {
            InitUIUsingHistory(_initWithLast);
            _next.gameObject.SetActive(true);
        }
        _chat.newAnswerTreated.AddListener(UpdateUI);
    }

    [Button]
    void InitUIUsingHistory(int index = 0)
    {
        var data = _chat.ReadHistoryEntry(index);
        _inputText.text = data.info;
        _chat._crossSceneInfo.SetFinalData(data.data);
        UpdateUI(data.data);
    }

    void UpdateUI(FinalData data)
    {
        _buildings.UpdateTiles(data.buildings.Select(building => new TileController.VisualInformation()
        {
            name = building.name, description = building.description, visuals = building.visualData, values = new float[]{building.maxHealth, building.selfHealFactor,building.defence,building.spawnDelay}
        }));
        _units.UpdateTiles(data.units.Select(unit => new TileController.VisualInformation()
        {
            name = unit.name, description = unit.description, visuals = unit.visualData, values =new float[]{unit.maxHealth, unit.selfHealFactor,unit.defence,unit.strength}
        }));
    }
}