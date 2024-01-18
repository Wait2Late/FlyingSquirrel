using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * This script is for the snatching enemy; the enemy that is stationary but attacks the player if he gets close.
 */

[RequireComponent(typeof(AudioSource))]

public class Flytrap: MonoBehaviour
{
    // references to enemy kill animations to play in "OnTriggerEnter"
    private DetectionBoxScript myDetectionBox;
    private Renderer myRenderer;
    private Animator myAnimator;

    [SerializeField]
    private GameObject[] myKillBoxes;

    AudioSource audioPlantBite;

    // Gets references to various components.
    private void Awake()
    {
        myDetectionBox = GetComponentInChildren<DetectionBoxScript>();
        myRenderer = GetComponentInChildren<Renderer>();
        myAnimator = GetComponentInChildren<Animator>();

        audioPlantBite = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (myDetectionBox.HasDetected())
        {
            audioPlantBite.Play(0);
            Destroy(this);

            myAnimator.SetTrigger("Snatch");

            for (int i = 0; i < myKillBoxes.Length; ++i)
            {
                myKillBoxes[i].SetActive(true);
            }
        }
    }
}
