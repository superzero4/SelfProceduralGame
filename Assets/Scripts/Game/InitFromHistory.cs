using Game;
using TMPro;
using UnityEngine;

public class InitFromHistory : MonoBehaviour
{
    [SerializeField] private AnswerHistory history;
    [SerializeField] private GameObject buildingSpawner;
    
    [SerializeField] private GameObject buttonParent;
    [SerializeField] private GameObject buttonPrefab;
    
    void Start()
    {
        Building[] buidlings = history.Get(0).data.buildings;
        
        for (var i = 0; i < buidlings.Length; i++)
        {
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
