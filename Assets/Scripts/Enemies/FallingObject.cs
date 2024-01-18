using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class FallingObject : MonoBehaviour
{
    [SerializeField] private GameObject myRock;
    [SerializeField] private float myRockFallSpeed = 20;
    [SerializeField] private DetectionBoxScript myShakeDetectionBox;
    [SerializeField] private DetectionBoxScript myFallDetectionBox;

    [Header("Effects")]
    [SerializeField] private ParticleSystem[] myStillEffect = new ParticleSystem[2];
    [SerializeField] private ParticleSystem[] myShakingEffect = new ParticleSystem[2];
    [SerializeField] private ParticleSystem[] myFallingEffect = new ParticleSystem[2];

    private Rigidbody myRockRigidBody;
    private Animator myAnimator;
    private bool myRockShouldFall = false;
    private bool myHasPlayed = false;

    AudioSource audioRockCrumble;

    private bool myAudioHasPlayed = false;

    private void Awake()
    {
        myRockRigidBody = myRock.GetComponent<Rigidbody>();
        myAnimator = GetComponentInChildren<Animator>();
        myRockRigidBody.isKinematic = true;
        audioRockCrumble = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayStillEffect();
    }

    private void Update()
    {
        if (myShakeDetectionBox.HasDetected())
        {
            PlayShakingEffect();
            StopStillEffect();
            myAnimator.SetTrigger("Shake");
            
        }

        if (myFallDetectionBox.HasDetected())
        {
            audioRockCrumble.Stop();
            audioRockCrumble.Play(0);
            myRockShouldFall = true;
            myRockRigidBody.isKinematic = false;
            myAnimator.enabled = false;

            if (!myHasPlayed)
            {
                audioRockCrumble.Play(0);
                myAudioHasPlayed = true;
                Destroy(this);
            }

            StopShakingEffect();
            PlayFallEffect();
            Destroy(gameObject, 5f);
        }
    }

    private void FixedUpdate()
    {
        if (myRockShouldFall)
        {
            myRockRigidBody.AddForce(Vector3.down * myRockFallSpeed);
        }
    }

    private void PlayStillEffect()
    {
        myStillEffect[0].Play();
        myStillEffect[1].Play();
    }

    private void PlayShakingEffect()
    {
        myShakingEffect[0].Play();
        myShakingEffect[1].Play();
    }

    private void PlayFallEffect()
    {
        if (!myHasPlayed)
        {
            myFallingEffect[0].Play();
            myFallingEffect[1].Play();
            myHasPlayed = true;
        }
    }

    private void StopStillEffect()
    {
        myStillEffect[0].Stop();
        myStillEffect[1].Stop();
    }
    private void StopShakingEffect()
    {
        myShakingEffect[0].Stop();
        myShakingEffect[1].Stop();
    }


}
