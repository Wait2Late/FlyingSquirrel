using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaPickup : MonoBehaviour
{
    [SerializeField] StaminaBar myStaminaBar;
    [SerializeField] GameObject myOrb;
    GameObject myCore;
    AudioSource myAudioPickup;

    void Start()
    {
        myStaminaBar = GameObject.Find("PlayerPrefab").GetComponent<StaminaBar>();
        myAudioPickup = GetComponent<AudioSource>();
        myCore = GameObject.Find("Inner Orb");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            myAudioPickup.Play(0);
            //myOrb.SetActive(false);
            myStaminaBar.RefillStamina();

            Destroy(myOrb);
            Destroy(gameObject, 1.5f);
        }
    }
}