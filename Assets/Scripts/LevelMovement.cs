using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMovement : MonoBehaviour
{
    [SerializeField] float myCurrentSpeed;
    float myBaseSpeed;
    float myMaxSpeed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(0, 0, -myCurrentSpeed * Time.deltaTime);
    }

    public void SetInitialSpeeds(float aStartingBaseSpeed, float aStartingMaxSpeed)
    {
        myBaseSpeed = aStartingBaseSpeed;
        myMaxSpeed = aStartingMaxSpeed;
        myCurrentSpeed = myBaseSpeed;
    }
    public void SetSpeed(float aModifier)
    {
        myCurrentSpeed += aModifier;

        if (myCurrentSpeed < myBaseSpeed)
        {
            myCurrentSpeed = myBaseSpeed;
        }
        if (myCurrentSpeed > myMaxSpeed)
        {
            myCurrentSpeed = myMaxSpeed;
        }
    }

    public float GetSpeed()
    {
        return myCurrentSpeed;
    }
}
