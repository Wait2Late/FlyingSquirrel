using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    /* VISIBLE IN EDITOR */
    [Header("References")]
    [SerializeField] private GameObject myPlayerReference;
    [SerializeField] private GameObject myReferencePoint;

    [Header("Values")]
    [SerializeField] private Vector3 myDefaultCameraOffset = new Vector3(0f, -.3f, -3f);
    private Vector3 myCameraOffset;
    [SerializeField] private float myFollowEase = .3f;
    [SerializeField] private float myDefaultFieldOfView = 60f;
    [SerializeField] private float myFieldOfViewFactor = 1.75f;
    [SerializeField] private float myMaximumCameraDistanceToWallRelativeToView = 7f;
    [Header("Camera Shake")]
    [SerializeField] private float myShakeMagnitude = .075f;
    [SerializeField] private float myShakeDuration = .5f;

    /* PRIVATE */
    /* Values */
    private Vector3 myVelocity = Vector3.zero;
    /* References */
    private Collider myPlayerCapsuleCollider;
    private Renderer myPlayerRenderer;
    private Quaternion myOriginalRotation;
    private bool myIsFrozen = false;
    PlayerController myPlayerScript;

    /* Methods */
    private void Awake()
    {
        myCameraOffset = myDefaultCameraOffset;
        if (!myPlayerReference)
        {
            return;
        }

        myPlayerCapsuleCollider = myPlayerReference.GetComponentInChildren<CapsuleCollider>();
        myPlayerRenderer = myPlayerReference.GetComponentInChildren<Renderer>();
        myPlayerScript = myPlayerReference.GetComponent<PlayerController>();

        myOriginalRotation = transform.rotation;
    }
    private void Start()
    {
        if (!myPlayerReference)
        {
            return;
        }

        transform.position = myPlayerReference.transform.position + myCameraOffset;
    }
    private void FixedUpdate()
    {
        if (!myPlayerReference || myIsFrozen)
        {
            return;
        }

        /* Values and Calculations */
        Vector3 playerPositionInViewport = Camera.main.WorldToViewportPoint(myPlayerReference.transform.position);
        float[,] playerMaxCoordinates = myPlayerReference.GetComponent<PlayerController>().GetMaxCoordinates();
        Vector3 newPosition = myPlayerReference.transform.position + myCameraOffset;
        float cameraViewHeight = 2.0f * (myReferencePoint.transform.position - myPlayerReference.transform.position).magnitude * Mathf.Tan(Camera.main.fieldOfView * .5f * Mathf.Deg2Rad);
        float cameraViewWidth = Camera.main.aspect * cameraViewHeight;
        float XYPlaneDistanceToPlayer = Mathf.Sqrt(Mathf.Pow(myPlayerReference.transform.position.x - transform.position.x, 2f) + Mathf.Pow(myPlayerReference.transform.position.y - transform.position.y, 2f));

        myReferencePoint.transform.position = newPosition;

        /* Rotation */
        Quaternion newRotation = Quaternion.LookRotation(myPlayerReference.transform.position - transform.position);
        newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * Mathf.Sqrt(XYPlaneDistanceToPlayer) * 2f);

        /* Bounds */
        if (myPlayerReference.transform.position.x < playerMaxCoordinates[0, 0] + (cameraViewWidth / myMaximumCameraDistanceToWallRelativeToView) + (myPlayerCapsuleCollider.bounds.extents.x / 2))
        {
            newPosition.x = playerMaxCoordinates[0, 0] + (cameraViewWidth / myMaximumCameraDistanceToWallRelativeToView) + (myPlayerCapsuleCollider.bounds.extents.x / 2);
        }
        else if (myPlayerReference.transform.position.x > playerMaxCoordinates[0, 1] - (cameraViewWidth / myMaximumCameraDistanceToWallRelativeToView) - (myPlayerCapsuleCollider.bounds.extents.x / 2))
        {
            newPosition.x = playerMaxCoordinates[0, 1] - (cameraViewWidth / myMaximumCameraDistanceToWallRelativeToView) - (myPlayerCapsuleCollider.bounds.extents.x / 2);
        }
        if (myPlayerReference.transform.position.y < playerMaxCoordinates[1, 0] + cameraViewHeight / myMaximumCameraDistanceToWallRelativeToView)
        {
            newPosition.y = playerMaxCoordinates[1, 0] + cameraViewHeight / myMaximumCameraDistanceToWallRelativeToView;
        }
        else if (myPlayerReference.transform.position.y > playerMaxCoordinates[1, 1] - cameraViewHeight / myMaximumCameraDistanceToWallRelativeToView)
        {
            newPosition.y = playerMaxCoordinates[1, 1] - cameraViewHeight / myMaximumCameraDistanceToWallRelativeToView;
        }

        /* Movement */
        Vector3 newLerpPosition = Vector3.SmoothDamp(transform.position, newPosition, ref myVelocity, myFollowEase);
        newLerpPosition.z = newPosition.z;

        transform.position = newLerpPosition;

        /* Field of View */
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, (myDefaultFieldOfView - (myPlayerReference.GetComponent<PlayerController>().GetBaseSpeed() * myFieldOfViewFactor) + (myPlayerReference.GetComponent<PlayerController>().GetSpeed() * myFieldOfViewFactor)), .8f);

    }
    public void SetOffset(Vector3 anOffset)
    {
        myCameraOffset = anOffset;
    }
    public Vector3 GetOffset()
    {
        return myCameraOffset;
    }
    public Vector3 GetDefaultOffset()
    {
        return myDefaultCameraOffset;
    }

    public void Freeze()
    {
        myIsFrozen = true;
    }

    public void UnFreeze()
    {
        myIsFrozen = false;
    }

    public void ToggleFreeze()
    {
        myIsFrozen = !myIsFrozen;
    }

    public IEnumerator Shake()
    {
        Vector3 position = transform.localPosition;

        float elapsedTime = 0f;
        Freeze();

        while (elapsedTime < myShakeDuration)
        {
            float newXPosition = Random.Range(-1f, 1f) * myShakeMagnitude;
            float newYPosition = Random.Range(-1f, 1f) * myShakeMagnitude;

            transform.localPosition = new Vector3(newXPosition + position.x, newYPosition + position.y, position.z);

            elapsedTime += Time.unscaledDeltaTime;

            yield return null;
        }

        transform.localPosition = position;
    }
}
