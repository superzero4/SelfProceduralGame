using Sirenix.OdinInspector;
using UnityEngine;

public class EditorChatter : ChatInteractor
{
    [SerializeField] private string _theme = "Cowboy";
    [SerializeField,Range(1,100)] private int _nbUnit = 10;
    [SerializeField,Range(1,100)] private int _nbBuilding = 20;
    private const string _unitsKey = "units";
    private const string _nameKey = "name";
    private const string _descriptionKey = "description";
    private const string _buildingsKey = "buildings";
    private const string _dependencyKey = "depends";
    [SerializeField,ReadOnly,TextArea(3,200)] private string _result;
    [SerializeField,TextArea(3,200)] private string _json;

    protected override void OnAnswerReceived(string result)
    {
        _result = result;
        _json = result;
    }

    private static string KeyPrompt(string name, string key)
    {
        return "\n-" + name + " : " + "\"" + key + "\"";
    }

    private static string BuildPrompt(string theme, int nbUnit, int nbBuilding)
    {
        return
            $"I am building a Real Time Strategy video game, the theme is : {theme}, i want {nbUnit} units, and {nbBuilding} buildings with names and description.\n Both the units and the buildings may depends on some other buildings, identified by they name.\n" +
            $"can you generate me a json with the game name and description, the buildings names, descriptions and dependencies and the units names, descriptions and dependencies.\n Use these keys : " +
            KeyPrompt("names", _nameKey) +
            KeyPrompt("descriptions", _descriptionKey) +
            KeyPrompt("units", _unitsKey) +
            KeyPrompt("buildings", _buildingsKey) +
            KeyPrompt("dependency key", _dependencyKey);
    }

    [Button]
    private void TestPrompt()
    {
        SubmitToChat(BuildPrompt(_theme, _nbUnit, _nbBuilding));
    }
}