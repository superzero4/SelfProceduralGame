using Game;
using Newtonsoft.Json;
using UnityEngine;

public class CrossSceneInfo : MonoBehaviour
{
    public FinalData data { get; private set; }

    public void SetFinalData(FinalData d)
    {
        this.data = d;
        //Debug.Log(JsonSerializer.Serialize(d));
    }
}