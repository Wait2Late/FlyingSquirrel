using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindScript : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float myDefaultVolume = .01f;
    [SerializeField] float myDefaultPitch = 1f;
    [Header("Player")]
    [SerializeField] GameObject myPlayer;
    private AudioSource audioSource;
    private PlayerController controller;
    private void Start()
    {
        controller = myPlayer.GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = myDefaultVolume;
        audioSource.pitch = myDefaultPitch;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.mute = true;
    }
    private void Update()
    {
        if (!myPlayer || controller.GetIsRunning() || !controller.myIsAlive)
        {
            audioSource.mute = true;
        }
        else
        {
            audioSource.mute = false;
        }

        audioSource.pitch = controller.GetSpeed() / (controller.GetBaseSpeed() * 2);
        audioSource.volume = (controller.GetSpeed() / (controller.GetBaseSpeed())) * myDefaultVolume;
    }
}
