using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AnchorPoint : MonoBehaviour
{
    public ControlPoint c1;
    public ControlPoint c2;

    Vector3 c1PosLastFrame;
    Vector3 c2PosLastFrame;

    private void Update()
    {
        if (c1PosLastFrame!=c1.transform.position)
        {
            MirrorControlPoints(c2, c1);
        }
        else if (c2PosLastFrame != c2.transform.position)
        {
            MirrorControlPoints(c1, c2);
        }

        c1PosLastFrame = c1.transform.position;
        c2PosLastFrame = c2.transform.position;
    }

    public void MirrorControlPoints(ControlPoint aPointToMove, ControlPoint aPointToMirror)
    {
        Vector3 vectorToC1 = transform.position - aPointToMirror.transform.position;

        aPointToMove.transform.position = transform.position + vectorToC1;

        //aPointToMirror.transform.rotation = Quaternion.LookRotation(aPointToMirror.transform.position - transform.position);
        //aPointToMove.transform.rotation = Quaternion.LookRotation(aPointToMove.transform.position - transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(c1.transform.position, c2.transform.position);
    }
}
