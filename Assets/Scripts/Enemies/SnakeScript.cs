using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeScript : MonoBehaviour
{
    [SerializeField] private DetectionBoxScript[] myDetectionBoxes;
    [SerializeField] private int mySnakeVersion = 0;
    [SerializeField] private GameObject myFirstEyeLid;
    [SerializeField] private GameObject mySecondEyeLid;
    private Animator myAnimator;

    bool hasSnatched = false;

    AudioSource audioSnakeBite;

    private void Awake()
    {
        myAnimator = GetComponentInChildren<Animator>();

        audioSnakeBite = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (mySnakeVersion == 1)
        {
            SnakeOne();
        }
        else if (mySnakeVersion == 2)
        {
            SnakeTwo();
        }
        else
        {
            Debug.LogError("No valid snake version!");
        }
    }

    private void SnakeOne()
    {
        if (myDetectionBoxes[0].HasDetected())
        {
            myAnimator.SetTrigger("Anticipate");

            if (myDetectionBoxes[1].HasDetected())
            {
                myAnimator.SetTrigger("Upper Left");
                PlaySound();
            }
            else if (myDetectionBoxes[2].HasDetected())
            {
                myAnimator.SetTrigger("Upper Mid");
                PlaySound();
            }
            else if (myDetectionBoxes[3].HasDetected())
            {
                myAnimator.SetTrigger("Upper Right");
                PlaySound();
            }
            else if (myDetectionBoxes[4].HasDetected())
            {
                myAnimator.SetTrigger("Lower Left");
                PlaySound();
            }
            else if (myDetectionBoxes[5].HasDetected())
            {
                myAnimator.SetTrigger("Lower Mid");
                PlaySound();
            }
            else if (myDetectionBoxes[6].HasDetected())
            {
                myAnimator.SetTrigger("Lower Right");
                PlaySound();
            }
        }
    }

    private void SnakeTwo()
    {
        if (myDetectionBoxes[0].HasDetected())
        {
            myAnimator.SetTrigger("Anticipate");

            myAnimator.SetTrigger("Idle");

            myFirstEyeLid.SetActive(false);
            mySecondEyeLid.SetActive(false);

            if (myDetectionBoxes[1].HasDetected())
            {
                myAnimator.SetTrigger("Attack Right");
                PlaySound();
            }
            else if (myDetectionBoxes[2].HasDetected())
            {
                myAnimator.SetTrigger("Attack Middle");
                PlaySound();
            }
            else if (myDetectionBoxes[3].HasDetected())
            {
                myAnimator.SetTrigger("Attack Left");
                PlaySound();
            }
        }
    }

    private void PlaySound()
    {
        if (!hasSnatched)
        {
            // INSERT SOUND HERE
            audioSnakeBite.Play(0);
        }

        hasSnatched = true;
    }
}
