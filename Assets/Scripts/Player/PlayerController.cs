using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //[SerializeField] private float mySpeedMultiplier;
    [SerializeField] private bool myInvertedControls;

    private Animator myAnimator;
    private static float globalGravity = -20f;

    private float myVertical;
    private float myHorizontal;

    private bool myIsOutOfStamina;


    public Rigidbody myRigidbody;
    public StaminaBar myStaminaBar;
    public bool myIsAlive { get; set; } = true;
    public bool myIsInGoal = false;


    public float myBaseZForce = 15f;
    public float myUpwardsForce = 200f;
    public float myDownwardsForce = 150f;
    public float myGravityScale = 1f;
    public float mySideForce = 1f;
    public float myMaxZSpeed = 60f;
    public float myMinZSpeed = 40f;
    public float myCurrentZSpeed;
    public float myStartVelocity = 40f;
    public float myDivingAccelerationForce = 20f;
    public float myMaxTiltAngle = 45f;
    private Component[] myTrailRenderers;

    [Header("Movement Constraints")]
    [SerializeField] float myXLimit;
    [SerializeField] float myMaxY;
    [SerializeField] float myMinY;
    public float[,] myMaxCoordinates;

    [SerializeField] bool ragdollTest = true;

    public Vector3 vel;

    //rail stuff
    [Header("Running Settings")]
    [SerializeField] float myRunningSpeed = 40f;
    [SerializeField] bool myIsRunning = false;
    bool myIsPreparingToRun = false;
    BezierCurveGenerator myCurrentCurve;
    float myRunningDistanceTraveled = 0;
    float myTotalDistanceOfRunningSection;
    float myDistanceToCurveLastFrame;
    List<GameObject> myUsedRunningSections = new List<GameObject>();
    Vector3 myDefaultRotation;
    [SerializeField] float myDistanceToRunningSectionThreshhold = .5f;
    [SerializeField] float mySpeedToTransitionToRun = 20f;
    float myInputDelayTimer = 1.0f;


    private void Awake()
    {
        myMaxCoordinates = new float[,] { { -myXLimit, myXLimit }, { myMinY, myMaxY } };
        myTrailRenderers = GetComponentsInChildren<TrailRenderer>();
    }

    void Start()
    {
        SetRigidbodyState(true, false);
        SetColliderState(false, true);

        myDefaultRotation = transform.rotation.eulerAngles;
        myRigidbody.useGravity = false;
        myAnimator = GetComponentInChildren<Animator>();
        StartVelocity();

        //mySpeedMultiplier = GameManager.ourInstance.mySpeedMultiplier;
        myInvertedControls = GameManager.ourInstance.myInvertedControls;
    }

    private void StartVelocity()
    {
        myRigidbody.AddForce(Vector3.forward * myStartVelocity, ForceMode.Impulse);
    }

    void SetRigidbodyState(bool aState, bool anotherState)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = aState;
        }

        GetComponent<Rigidbody>().isKinematic = anotherState;
    }

    void SetColliderState(bool aState, bool anotherState)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider c in colliders)
        {
            c.enabled = aState;
        }

        GetComponent<Collider>().enabled = anotherState;
    }

    public bool GetInverted()
    {
        return myInvertedControls;
    }


    void Update()
    {
        HandleAnimation();
    }

    private void FixedUpdate()
    {
        myInputDelayTimer -= Time.fixedDeltaTime;
        if (myIsAlive)
        {
            if (!myIsRunning && !myIsPreparingToRun)
            {
                ApplyGravity();

                ApplyDrag();

                ShowVelocityInInspector();

                GetAxises();

                LimitMovementWhenStaminaDepleted();

                FlyVertical();

                FlySideways();

                FlyForward();

                CheckIfInsideBounds();

                TiltPlayer();
            }
            else if (myIsPreparingToRun)
            {
                FlyTowardsPath();
            }
            else if (myIsRunning)
            {
                RunAlongPath();
            }
        }
    }

    float IsOutOfStamina(float vertical)
    {
        if (vertical <= 0)
        {
            return vertical;
        }
        else if (vertical > 0 && myIsOutOfStamina)
        {
            vertical = 0;
        }
        return vertical;
    }

    void LimitMovementWhenStaminaDepleted()
    {

        if (myStaminaBar.GetStaminaValue() <= 1)
        {
            myIsOutOfStamina = true;
            Debug.Log("myIsOutOfStamina = " + myIsOutOfStamina);
        }
        else myIsOutOfStamina = false;
    }

    public float[,] GetMaxCoordinates()
    {
        return myMaxCoordinates;
    }

    public float GetBaseSpeed()
    {
        return myMinZSpeed;
    }

    public float GetSpeed()
    {
        return myCurrentZSpeed;
    }

    private void CheckIfInsideBounds()
    {
        if (transform.position.x < myMaxCoordinates[0, 0])
        {
            transform.position = new Vector3(myMaxCoordinates[0, 0], transform.position.y, transform.position.z);
        }
        if (transform.position.x > myMaxCoordinates[0, 1])
        {
            transform.position = new Vector3(myMaxCoordinates[0, 1], transform.position.y, transform.position.z);
        }
        if (transform.position.y < myMaxCoordinates[1, 0])
        {
            transform.position = new Vector3(transform.position.x, myMaxCoordinates[1, 0], transform.position.z);
        }
        if (transform.position.y > myMaxCoordinates[1, 1])
        {
            transform.position = new Vector3(transform.position.x, myMaxCoordinates[1, 1], transform.position.z);
        }
    }

    private void ShowVelocityInInspector()
    {
        myCurrentZSpeed = myRigidbody.velocity.z;
    }

    private void GetAxises()
    {
        if (myInputDelayTimer <= 0)
        {
            myInputDelayTimer = 0;
            myVertical = Input.GetAxisRaw("Vertical");
            myHorizontal = Input.GetAxisRaw("Horizontal");
        }
    }

    private void FlyVertical()
    {
        if (!myInvertedControls)
        {
            if (myVertical > 0 && !myIsOutOfStamina)
            {
                myRigidbody.AddForce(Vector3.up * myUpwardsForce);
            }

            if (myVertical < 0)
            {
                myRigidbody.AddForce(Vector3.down * myDownwardsForce);

            }
        }
        else if (myInvertedControls)
        {
            if (myVertical > 0 && !myIsOutOfStamina)
            {
                myRigidbody.AddForce(Vector3.down * myUpwardsForce);
            }

            if (myVertical < 0)
            {
                myRigidbody.AddForce(Vector3.up * myDownwardsForce);

            }
        }
    }

    private void FlySideways()
    {
        if (myHorizontal < 0)
        {
            myRigidbody.AddForce(Vector3.left * mySideForce);

        }
        if (myHorizontal > 0)
        {
            myRigidbody.AddForce(Vector3.right * mySideForce);
        }
    }

    private void FlyForward()
    {
        if (myRigidbody.velocity.z < myMinZSpeed)
        {
            myRigidbody.AddForce(Vector3.forward * myBaseZForce);

        }
        if (!myInvertedControls)
        {
            //Accelerate speed when diving
            if (myVertical < 0)
            {
                myRigidbody.AddForce(Vector3.forward * myDivingAccelerationForce);
            }
            //Decelerate when going up by adding force in opposite direction until lowest speed allowed is reached.
            else if (myVertical > 0 && myRigidbody.velocity.z > myMinZSpeed)
            {
                myRigidbody.AddForce(Vector3.back * (myRigidbody.velocity.z * 1.5f - myMaxZSpeed), ForceMode.Acceleration);
            }
        }
        else
        {
            //Accelerate speed when diving
            if (myVertical > 0)
            {
                myRigidbody.AddForce(Vector3.forward * myDivingAccelerationForce);
            }
            //Decelerate when going up by adding force in opposite direction until lowest speed allowed is reached.
            else if (myVertical < 0 && myRigidbody.velocity.z > myMinZSpeed)
            {
                myRigidbody.AddForce(Vector3.back * (myRigidbody.velocity.z * 1.5f - myMaxZSpeed), ForceMode.Acceleration);
            }
        }

        //Lock speed at 60
        if (myRigidbody.velocity.z > myMaxZSpeed)
        {
            myRigidbody.AddForce(Vector3.back * (myRigidbody.velocity.z * 2 - myMaxZSpeed));
        }
    }

    private void ApplyDrag()
    {
        vel = GetComponent<Rigidbody>().velocity;
        if (myVertical < 0)
        {
            if (!myIsOutOfStamina)
            {
                myGravityScale = 1;
            }
            vel.y *= 0.97f;
        }
        else if (myVertical > 0 && !myIsOutOfStamina)
        {
            if (!myIsOutOfStamina)
            {
                myGravityScale = 1;
                vel.y *= 0.95f;
            }
            else
            {
                vel.y *= 0.85f;
            }


        }
        else
        {
            if (!myIsRunning)
            {
                FallingFasterWhenGliding();
            }


        }
        //sidedrag
        vel.x *= 0.97f;

        GetComponent<Rigidbody>().velocity = vel;
    }

    private void FallingFasterWhenGliding()
    {
        float fallingFasterWhenGlidingMulti = 1.007f;
        myGravityScale *= fallingFasterWhenGlidingMulti;
        vel.y *= 0.85f;
    }

    //Unitys own gravity is set to -9.81, so to change the value of gravity, i disabled it and created my own. To change gravity, edit GravityScale in the 
    //Unity-Editor
    private void ApplyGravity()
    {
        Vector3 gravity = globalGravity * myGravityScale * Vector3.up;
        myRigidbody.AddForce(gravity, ForceMode.Acceleration);
    }

    private void FlyTowardsPath()
    {
        //myRigidbody.velocity = Vector3.zero;
        Vector3 closestPosOnCurve = myCurrentCurve.GetClosestPositonOnCurve(transform.position);
        Vector3 nextClosestPosOnCurve = myCurrentCurve.GetNextClosestPositonOnCurve(transform.position);

        float distanceToCurve = Vector3.Distance(transform.position, closestPosOnCurve);

        if (distanceToCurve < myDistanceToRunningSectionThreshhold)
        {
            myRunningSpeed = myCurrentZSpeed;
            myIsRunning = true;
            myAnimator.SetBool("isRunning", true);
            myIsPreparingToRun = false;
            myRunningDistanceTraveled = myCurrentCurve.GetDistanceToPos(myCurrentCurve.GetClosestPositonOnCurve(transform.position));
        }
        else if (transform.position.z > myCurrentCurve.GetPointFarthestInZ().z)
        {
            myIsPreparingToRun = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, nextClosestPosOnCurve, mySpeedToTransitionToRun * Time.deltaTime);
            mySpeedToTransitionToRun *= 1.1f;
        }
        myDistanceToCurveLastFrame = distanceToCurve;
    }

    private void RunAlongPath()
    {
        if (myRunningDistanceTraveled >= myTotalDistanceOfRunningSection)
        {
            myRigidbody.velocity = Vector3.zero;

            myRigidbody.velocity = (Vector3.forward * myRunningSpeed);

            myAnimator.SetBool("isRunning", false);
            myIsRunning = false;

            if (Camera.main.GetComponent<CameraScript>())
            {
                CameraScript cameraScript = Camera.main.GetComponent<CameraScript>();
                cameraScript.SetOffset(cameraScript.GetDefaultOffset());
            }
            foreach (TrailRenderer trail in myTrailRenderers)
            {
                trail.enabled = true;
            }

            Jump();
        }
        else
        {
            if (myRunningSpeed <= myMaxZSpeed)
            {
                myRunningSpeed *= 1.01f;
            }
            myRunningDistanceTraveled += Time.deltaTime * myRunningSpeed;
            Vector3[] newPosAndRot = myCurrentCurve.TravelOnPath(myRunningDistanceTraveled);

            if (newPosAndRot[0] != Vector3.zero && newPosAndRot[1] != Vector3.zero)
            {
                transform.position = newPosAndRot[0];
                transform.rotation = Quaternion.LookRotation(-newPosAndRot[1]);
            }
        }
        Debug.DrawRay(transform.position, myCurrentCurve.TravelOnPath(myRunningDistanceTraveled)[1] * myRunningSpeed * 100, Color.red);
        Debug.DrawRay(transform.position, new Vector3(0, 1, 0) * 100, Color.cyan);
    }

    private void Jump()
    {
        myRigidbody.velocity = myCurrentCurve.TravelOnPath(myRunningDistanceTraveled)[1].normalized * myRunningSpeed;

        myRigidbody.AddForce(Vector3.up * 10, ForceMode.VelocityChange);
        transform.rotation = Quaternion.Euler(myDefaultRotation);
    }

    private void HandleAnimation()
    {
        myAnimator.SetFloat("velX", myRigidbody.velocity.x);
        myAnimator.SetFloat("velY", myRigidbody.velocity.y);

    }

    void TiltPlayer()
    {
        float smoothing = 5;

        if (!myInvertedControls)
        {
            Quaternion target = Quaternion.Euler((ReturnLessVerticalOnlyWhenNotStrafing() * ReturnMoreTiltAngleWhenDiving()), (myHorizontal * 20) + 180, (myHorizontal * myMaxTiltAngle));
            myRigidbody.transform.rotation = Quaternion.Slerp(myRigidbody.transform.rotation, target, Time.fixedDeltaTime * smoothing);
        }
        else if (myInvertedControls)
        {
            Quaternion target = Quaternion.Euler((ReturnLessVerticalOnlyWhenNotStrafing() * -ReturnMoreTiltAngleWhenDiving()), (myHorizontal * 20) + 180, (myHorizontal * myMaxTiltAngle));
            myRigidbody.transform.rotation = Quaternion.Slerp(myRigidbody.transform.rotation, target, Time.fixedDeltaTime * smoothing);
        }


    }

    float ReturnPositiveVerticalOnlyWhenStaminaNotEmpty()
    {
        if (myIsOutOfStamina && myVertical > 0)
        {
            return 0;

        }
        else
        {
            return myVertical;
        }
    }


    float ReturnLessVerticalOnlyWhenNotStrafing()
    {
        if (myHorizontal != 0)
        {
            return myVertical / 3;
        }
        else
        {
            return ReturnPositiveVerticalOnlyWhenStaminaNotEmpty();
        }
    }

    float ReturnMoreTiltAngleWhenDiving()
    {
        if (myVertical > 0)
        {
            return myMaxTiltAngle;
        }
        else
        {
            return myMaxTiltAngle + 10;
        }
    }

    private void OnTriggerEnter(Collider someCollider)
    {
        if (someCollider.CompareTag("Goal"))
        {
            myIsInGoal = true;
        }
        else if (someCollider.transform.parent)
        {
            if (someCollider.transform.parent.CompareTag("RunningSection"))
            {
                if (!myUsedRunningSections.Contains(someCollider.transform.parent.gameObject))
                {
                    mySpeedToTransitionToRun = myCurrentZSpeed;
                    myCurrentCurve = someCollider.transform.parent.GetComponentInChildren<BezierCurveGenerator>();
                    myTotalDistanceOfRunningSection = myCurrentCurve.GetLengthOfCurve();
                    myIsPreparingToRun = true;
                    myDistanceToCurveLastFrame = Vector3.Distance(transform.position, myCurrentCurve.GetClosestPositonOnCurve(transform.position));
                    myUsedRunningSections.Add(myCurrentCurve.transform.parent.gameObject);

                    if (Camera.main.GetComponent<CameraScript>())
                    {
                        CameraScript cameraScript = Camera.main.GetComponent<CameraScript>();
                        cameraScript.SetOffset(cameraScript.GetDefaultOffset() + new Vector3(0f, 1f, 0f));
                    }
                    foreach (TrailRenderer trail in myTrailRenderers)
                    {
                        trail.enabled = false;
                    }
                }
            }
        }
    }

    public bool GetIsRunning()
    {
        return myIsRunning;
    }

    private void OnCollisionEnter(Collision someCollision)
    {
        if (someCollision.gameObject.tag != "Player")
        {
            if (myIsRunning || myIsPreparingToRun)
            {
                Collider[] colliders = GetComponentsInChildren<Collider>();
                for (int i = 0; i < colliders.Length; i++)
                {
                    Physics.IgnoreCollision(someCollision.collider, colliders[i]);
                }
            }
            else
            {
                StartCoroutine(Camera.main.GetComponent<CameraScript>().Shake());
                GetComponent<AudioSource>().Play();
                myIsAlive = false;
                GetComponentInChildren<Animator>().enabled = false;
                SetRigidbodyState(false, false);
                SetColliderState(true, false);
            }
        }
    }

    private void OnCollisionStay(Collision someCollision)
    {
        if (someCollision.gameObject.tag != "Player")
        {
            if (myIsRunning || myIsPreparingToRun)
            {
                Collider[] colliders = GetComponentsInChildren<Collider>();
                for (int i = 0; i < colliders.Length; i++)
                {
                    Physics.IgnoreCollision(someCollision.collider, colliders[i]);
                }
            }
            else
            {
                StartCoroutine(Camera.main.GetComponent<CameraScript>().Shake());
                GetComponent<AudioSource>().Play();
                myIsAlive = false;
                GetComponentInChildren<Animator>().enabled = false;
                SetRigidbodyState(false, false);
                SetColliderState(true, false);
            }
        }
    }
}
