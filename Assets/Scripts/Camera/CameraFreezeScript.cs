using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFreezeScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider anOther)
    {
        if (anOther.CompareTag("Player"))
        {
            CameraScript script = Camera.main.GetComponent<CameraScript>();
            if (!script)
            {
                return;
            }
            script.Freeze();
        }
       
    }
}
