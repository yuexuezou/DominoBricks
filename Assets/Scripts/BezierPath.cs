using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BezierPath : MonoBehaviour
{
    public List<Vector3> controlPoints = new List<Vector3>()
    {
        new Vector3(-2, 0, 0),
        new Vector3(-1, 2, 0),
        new Vector3(1, -2, 0),
        new Vector3(2, 0, 0)
    };

    public int CurveResolution = 30;

    public Vector3 GetPoint(float t)
    {
        if (controlPoints.Count < 4) return Vector3.zero;
        return CalculateCubicBezierPoint(t, controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3]);
    }

    public static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;
        return p;
    }
}