using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class DetectionBoxScript : MonoBehaviour
{
    private bool myHasDetected = false;
    private GameObject myDetectedObject;

    public bool HasDetected()
    {
        return myHasDetected;
    }

    public GameObject DetectedObject()
    {
        return myDetectedObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            myDetectedObject = other.gameObject;
            myHasDetected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            myHasDetected = false;
        }
    }
}
