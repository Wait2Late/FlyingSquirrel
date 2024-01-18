using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;

/*
 * 
 * This script is for the Tracking Enemy.
 * It handles all the movement and tracking.
 * It has references to other scripts such as DetectionBoxScript & StringerScript for access to some variables in those (which are used for checking certain interactions)
 * 
 */

enum TrackingEnemyState
{
    Patrol,
    Alert,
    Attacking
};

public class TrackingEnemyScript : MonoBehaviour
{
    /* --- PUBLIC --- */
    /* Values */
    [SerializeField] private float mySpeed = 100f;
    [SerializeField] private float myAlertTimer = 2.0f;
    [SerializeField] private float myDestroyTimer = 3.0f;
    [SerializeField] private float myBoppingHeight = 5f;
    [SerializeField] private float myBoppingFactor = .5f;
    [SerializeField] private float myTurnFactor = 7f;
    [SerializeField] private float myPatrolLength = 25f;
    [SerializeField] private Vector3 myForwardOffset = new Vector3(0f, 0f, 16f);
    /* Components */
    [SerializeField] DetectionBoxScript myDetectionBox;
    [SerializeField] GameObject myEnemy;

    /* --- PRIVATE --- */
    /* Values */
    private bool myIsAlert = false;
    private bool myIsAttacking = false;
    private bool myFoundTarget = false;
    private TrackingEnemyState myCurrentState = TrackingEnemyState.Patrol;
    /* Components */
    private Rigidbody myRigidboy;
    private Vector3 myDirection;
    private Animator myAnimator;
    private GameObject myPlayerRef;

    /* --- PRIVATE METHODS --- */
    private void Awake()         // Initializes component references
    {
        myRigidboy = GetComponentInChildren<Rigidbody>();
        myAnimator = GetComponentInChildren<Animator>();
    }
    private void Update()           // Update function (works as a continous loop) 
    {
        myPlayerRef = myDetectionBox.DetectedObject();
        myAnimator.SetBool("IsAttacking", myIsAttacking);
        myAnimator.SetBool("IsAlert", myIsAlert);

        if (myDetectionBox.HasDetected())
        {
            myIsAlert = true;
            myCurrentState = TrackingEnemyState.Alert;

            if (!myIsAttacking)
            {
                myEnemy.transform.LookAt(myPlayerRef.transform);
            }

            if (myAlertTimer < 0)
            {
                myIsAttacking = true;
                myCurrentState = TrackingEnemyState.Attacking;
            }
        }

        if (myIsAlert && myIsAttacking)
        {
            myIsAlert = false;
        }
    }

    private void LateUpdate()
    {
        switch (myCurrentState)
        {
            case TrackingEnemyState.Patrol:
                {
                    myIsAlert = false;
                    myIsAttacking = false;
                    Patrol();
                    break;
                }
            case TrackingEnemyState.Alert:
                {
                    myAnimator.SetTrigger("Be Alert");
                    myDirection = GetTargetDirection(myPlayerRef);
                    myEnemy.transform.LookAt(myPlayerRef.transform);
                    myAlertTimer -= Time.deltaTime;
                    break;
                }
            case TrackingEnemyState.Attacking:
                {
                    GetTargetDirectionOnce(GetTargetDirection(myPlayerRef));
                    Attack(myDirection);
                    break;
                }
        }
    }

    private Vector3 GetTargetDirection(GameObject aTarget)        // Takes the player's position and returns the direction-vector used for moving
    {
        Vector3 direction = aTarget.transform.position - transform.position;

        return direction;
    }
    private void Patrol()
    {
        if (!myEnemy)
        {
            return;
        }

        print(myPatrolLength);

        Vector3 sineVector = new Vector3(Mathf.Sin(Time.time), Mathf.Sin(Time.time * myBoppingHeight), 0f);
        Vector3 scalingVector = new Vector3(myPatrolLength / 4f, myBoppingFactor, 0f);
        Vector3 movement = Vector3.Scale(sineVector, scalingVector);
        Vector3 newPosition = new Vector3(0f, 0f, myEnemy.transform.localPosition.z) + movement;
        Quaternion newRotation = Quaternion.Euler(0, 90  *((newPosition.x > myEnemy.transform.localPosition.x) ? 1f : -1f), 0);

        myEnemy.transform.localRotation = Quaternion.Slerp(myEnemy.transform.localRotation, newRotation, Time.deltaTime * myTurnFactor);
        myEnemy.transform.localPosition = newPosition;
    }
    private void Attack(Vector3 aDirection)           // Takes the direction and move enemy towards it
    {
        transform.Translate(aDirection.normalized * mySpeed * Time.deltaTime, Space.World);
        Destroy(gameObject, myDestroyTimer);
    }

    private void GetTargetDirectionOnce(Vector3 aTarget)
    {
        if (!myFoundTarget)
        {
            print("found it!");
            myDirection = aTarget + myForwardOffset;
            myFoundTarget = true;
        }
    }
}