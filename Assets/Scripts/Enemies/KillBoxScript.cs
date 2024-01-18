using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBoxScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController thePlayerScript = other.gameObject.GetComponentInParent<PlayerController>();
            thePlayerScript.myIsAlive = false;
        }
    }
}
