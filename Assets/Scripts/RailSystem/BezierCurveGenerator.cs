using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class BezierCurveGenerator : MonoBehaviour
{
    [SerializeField] ControlPoint myControlPointPrefab;
    [SerializeField] AnchorPoint myAnchorPointPrefab;

    [Range(2, 20)]
    [SerializeField] int myNumberOfAnchorPoints = 2;
    
    float myDistanceToNewAnchor = 3;

    [SerializeField] List<AnchorPoint> myAnchorPoints = new List<AnchorPoint>();
 
    [SerializeField] bool myViewHelpLines;
    [SerializeField] bool myViewPosSpheres;

    [Range(2, 80)]
    [SerializeField] int myNumberOfPositionsPerSegment;
    int numberOfPositions;
    int numberOfSegments;

    Vector3[] positions;

    [SerializeField] float length;

    void Update()
    {
        //create/destroy points to be correct amount
        if (myAnchorPoints.Count!=myNumberOfAnchorPoints) // create/remove anchor and control points
        {
            if (myNumberOfAnchorPoints > myAnchorPoints.Count) //create new points
            {
                AnchorPoint newAnchor;
                if (myAnchorPoints.Count==0)
                {
                    newAnchor = Instantiate(myAnchorPointPrefab, transform.position,Quaternion.identity);
                    AssignNewAnchor(newAnchor);
                }
                else 
                {
                    int numberOfNewAnchors = myNumberOfAnchorPoints - myAnchorPoints.Count;
                    for (int i = 0; i < numberOfNewAnchors; i++)
                    {
                        newAnchor = Instantiate(myAnchorPointPrefab, myAnchorPoints[myAnchorPoints.Count - 1].transform.position + Vector3.forward * myDistanceToNewAnchor, Quaternion.identity);
                        AssignNewAnchor(newAnchor);
                    }                  
                }               
            }

            else //destroy old points
            {
                try
                {
                    int numberOfAnchorsToDestroy = myAnchorPoints.Count - myNumberOfAnchorPoints;
                    for (int i = 0; i < numberOfAnchorsToDestroy; i++)
                    {
                        AnchorPoint anchorToDestroy = myAnchorPoints[myAnchorPoints.Count - 1];
                        myAnchorPoints.RemoveAt(myAnchorPoints.Count - 1);

                        foreach (Transform child in anchorToDestroy.transform)
                        {
                            DestroyImmediate(child.gameObject);
                        }
                        DestroyImmediate(anchorToDestroy.gameObject);
                    }
                } catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
    
        numberOfSegments = myNumberOfAnchorPoints - 1;

        numberOfPositions = myNumberOfPositionsPerSegment * numberOfSegments;

        positions = new Vector3[numberOfPositions];

        for (int i = 0; i < numberOfSegments; i++)
        {
            Transform[] pointsForCurve = GetControlPointsFromSegment(i);

            for (int j = 0; j < myNumberOfPositionsPerSegment; j++)
            {               
                positions[j+i*myNumberOfPositionsPerSegment] = GetPosOnCurve((float)j / (float)myNumberOfPositionsPerSegment, pointsForCurve);
            }
        }

        length = GetLengthOfCurve();
    }

    void AssignNewAnchor(AnchorPoint aNewAnchor)
    {
        aNewAnchor.transform.SetParent(this.transform);
        myAnchorPoints.Add(aNewAnchor);

        ControlPoint newControlPoint1 = Instantiate(myControlPointPrefab, aNewAnchor.transform.position + Vector3.back, Quaternion.identity);
        ControlPoint newControlPoint2 = Instantiate(myControlPointPrefab);

        aNewAnchor.c1 = newControlPoint1;
        aNewAnchor.c2 = newControlPoint2;

        aNewAnchor.MirrorControlPoints(newControlPoint2, newControlPoint1);

        newControlPoint1.transform.SetParent(aNewAnchor.transform);
        newControlPoint2.transform.SetParent(aNewAnchor.transform);
    }

    public Vector3 GetPosOnCurve(float t, Transform[] somePositions)
    {
        Vector3 newPos = Mathf.Pow(1 - t, 3) * somePositions[0].position + 3 * Mathf.Pow(1 - t, 2) * t * somePositions[1].position + 3 * (1 - t) * Mathf.Pow(t, 2) * somePositions[2].position + Mathf.Pow(t, 3) * somePositions[3].position;

        return newPos;
    }

    public float GetTOnCurve(float z, Transform[] somePositions)
    {
        float p0 = somePositions[0].position.z;
        float p1 = somePositions[1].position.z;
        float p2 = somePositions[3].position.z;
        float p3 = somePositions[3].position.z;

        float a = p3 - 3 * p2 + 3 * p1 - p0;
        float b = 3 * (p2 - 2 * p1 + p0);
        float c = 3 * (p1 - p0);
        float d = p0 - z;

        float t = Mathf.Pow(((-Mathf.Pow(b, 3) / (27 * Mathf.Pow(a, 3)) + ((b * c) / 6 * Mathf.Pow(a, 2)) - d / (2 * a)) + Mathf.Sqrt((-Mathf.Pow(b, 3) / (27 * Mathf.Pow(a, 3)) + ((b * c) / 6 * Mathf.Pow(a, 2)) - d / (2 * a)) + Mathf.Pow((c / (3 * a) - (Mathf.Pow(b, 2) / (9 * Mathf.Pow(a, 2)))), 3))), 1f / 3f)
                + Mathf.Pow(((-Mathf.Pow(b, 3) / (27 * Mathf.Pow(a, 3)) + ((b * c) / 6 * Mathf.Pow(a, 2)) - d / (2 * a)) - Mathf.Sqrt((-Mathf.Pow(b, 3) / (27 * Mathf.Pow(a, 3)) + ((b * c) / 6 * Mathf.Pow(a, 2)) - d / (2 * a)) + Mathf.Pow((c / (3 * a) - (Mathf.Pow(b, 2) / (9 * Mathf.Pow(a, 2)))), 3))), 1f / 3f)
                - b / (3 * a);

        return t;
    }

    public Vector3 GetClosestPositionAlongCurve(Vector3 aPlayerPosition)
    {
        Vector3 closestPos = GetClosestPositonOnCurve(aPlayerPosition);

        Debug.Log(closestPos);
        Debug.Log(Vector3.Distance(aPlayerPosition, closestPos));

        int closestSegment = GetSegmentFromPos(closestPos);

        Transform[] controlPointsForSegment = GetControlPointsFromSegment(closestSegment);

        float t = GetTOnCurve(aPlayerPosition.z, controlPointsForSegment);

        Vector3 posOnCurve = GetPosOnCurve(t, controlPointsForSegment);

        return posOnCurve;
    }

    public Vector3[] TravelOnPath(float aDistanceTraveled)
    {
        float trackedDistance = 0;

        Vector3 posOnPath = Vector3.zero;

        Vector3 rotOnPath = Vector3.zero;

        bool positionFound = false;

        for (int i = 0; i < numberOfSegments; i++)
        {
            trackedDistance += GetLengthOfSegment(i);
            

            if ((aDistanceTraveled < trackedDistance && !positionFound)||aDistanceTraveled>GetLengthOfCurve()) //on this segment
            {
                Transform[] pointsForCurve = GetControlPointsFromSegment(i);

                float t = (aDistanceTraveled-(trackedDistance-GetLengthOfSegment(i)))/ GetLengthOfSegment(i);

                posOnPath = GetPosOnCurve(t, pointsForCurve);
                rotOnPath = GetRotOnCurve(t, pointsForCurve);
                positionFound = true;
            }          
        }
        return new Vector3[] { posOnPath, rotOnPath };
    }

    public float GetDistanceToPos(Vector3 aPosition)
    {
        float distanceToPos = 0;
        bool posIsFound = false;

        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i] != aPosition && !posIsFound)
            {
                distanceToPos += Vector3.Distance(positions[i], positions[i + 1]);
                
            }
            else if (positions[i] == aPosition)
            {
                posIsFound = true;
            }
        }
        return distanceToPos;
    }

    int GetSegmentFromPos(Vector3 aPositionToCheck)
    {
        float distanceToPos = 0;
        bool posIsFound = false;

        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i] != aPositionToCheck && !posIsFound)
            {
                distanceToPos += Vector3.Distance(positions[i], positions[i + 1]);

            }
            else if (positions[i] == aPositionToCheck)
            {
                posIsFound = true;
            }
        }

        float trackedDistance = 0;
        posIsFound = false;
        int segment = 0;

        for (int i = 0; i < numberOfSegments; i++)
        {
            trackedDistance += GetLengthOfSegment(i);

            if (distanceToPos < trackedDistance && !posIsFound)
            {
                segment = i;
                posIsFound = true;
            }
        }

        return segment;
    }

    Transform[] GetControlPointsFromSegment(int aSegment)
    {
        Transform[] controlPoints = new Transform[4];
        controlPoints[0] = myAnchorPoints[aSegment].transform;
        controlPoints[1] = myAnchorPoints[aSegment].c2.transform;
        controlPoints[2] = myAnchorPoints[aSegment + 1].c1.transform;
        controlPoints[3] = myAnchorPoints[aSegment + 1].transform;

        return controlPoints;
    }

    public Vector3 GetClosestPositonOnCurve(Vector3 aPositionToCompare)
    {
        Vector3 pos = Vector3.zero;

        float currentShortestDistance = Vector3.Distance(aPositionToCompare, positions[0]);

        for (int i = 0; i < positions.Length; i++)
        {
            if (Vector3.Distance(aPositionToCompare,positions[i])<=currentShortestDistance)
            {
                currentShortestDistance = Vector3.Distance(aPositionToCompare, positions[i]);
                pos = positions[i];
            }
        }
        return pos;
    }
    public Vector3 GetNextClosestPositonOnCurve(Vector3 aPositionToCompare)
    {
        Vector3 pos = Vector3.zero;

        float currentShortestDistance = Vector3.Distance(aPositionToCompare, positions[0]);

        for (int i = 0; i < positions.Length; i++)
        {
            if (Vector3.Distance(aPositionToCompare, positions[i]) <= currentShortestDistance && i<positions.Length-1)
            {
                currentShortestDistance = Vector3.Distance(aPositionToCompare, positions[i]);
                pos = positions[i+1];
            }
        }
        return pos;
    }

    public Vector3 GetPointFarthestInZ()
    {
        Vector3 pos = positions[0];
        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i].z>pos.z)
            {
                pos = positions[i];
            }
        }
        return pos;
    }

    public Vector3 GetRotOnCurve(float t, Transform[] somePositions)
    {
        Vector3 derivative = -3 * Mathf.Pow(1 - t, 2) * somePositions[0].position + 3 * Mathf.Pow(1 - t, 2) * somePositions[1].position - 6 * t * (1 - t) * somePositions[1].position - 3 * Mathf.Pow(t, 2) * somePositions[2].position + 6 * t * (1 - t) * somePositions[2].position + 3 * Mathf.Pow(t, 2) * somePositions[3].position;

        return derivative;
    }

    public float GetLengthOfCurve()
    {
        float length = 0;

        for (int i = 0; i < positions.Length - 1; i++)
        {
            length += Vector3.Distance(positions[i], positions[i + 1]);
        }
        return length;
    }
    
    float GetLengthOfSegment(int aSegment)
    {
        float length = 0;

        int startingPos = aSegment * myNumberOfPositionsPerSegment;

        for (int i = startingPos; i < startingPos+myNumberOfPositionsPerSegment; i++)
        {
            if (i!=positions.Length-1)
            {
                length += Vector3.Distance(positions[i], positions[i + 1]);
            }
            
        }
        return length;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < numberOfPositions; i++)
        {
            if (i != numberOfPositions - 1)
            {
                Gizmos.DrawLine(positions[i], positions[i + 1]);
            }

        }

        if (myViewPosSpheres)
        {
            Gizmos.color = Color.black;
            for (int i = 0; i < numberOfPositions; i++)
            {
                Gizmos.DrawSphere(positions[i], .05f);
            }
        }

    }
}
