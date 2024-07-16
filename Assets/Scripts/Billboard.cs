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
            transform.LookAt(cam.transform.position);
            transform.forward = -transform.forward;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
