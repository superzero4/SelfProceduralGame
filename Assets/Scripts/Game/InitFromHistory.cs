using Game;
using TMPro;
using UnityEngine;
using FinalData = Game.FinalData;

public class InitFromHistory : MonoBehaviour
{
    [SerializeField] private AnswerHistory history;
    [SerializeField] private GameObject buildingSpawner;
    
    [SerializeField] private GameObject buttonParent;
    [SerializeField] private GameObject buttonPrefab;

    private FinalData data;
    
    void Start()
    {
        data = FindObjectOfType<CrossSceneInfo>().data;

        Building[] buidlings;
        if (data.Equals(default(FinalData)))
        {
            Debug.LogWarning("Using history");
            buidlings = history.Get(0).data.buildings;
        }
        else
        {
            buidlings = data.buildings;
        }
        
        for (var i = 0; i < buidlings.Length; i++)
        {
            // Todo: assigner unités à ce building
            
            Building b = buidlings[i];
            GameObject newItem = Instantiate(buttonPrefab, buttonParent.transform);
            newItem.GetComponentInChildren<TMP_Text>().text = b.name;
        }
    }

    private void Spawn()
    {
        // Assign this to button
    }
}
