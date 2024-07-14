using System.Collections;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;
    
    private IEnumerator Start()
    {
        cam = Camera.main;

        while (true)
        {
            transform.LookAt(-cam.transform.position);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
