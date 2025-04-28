using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierPath))]
public class BezierPathEditor : Editor
{
    private void OnSceneGUI()
    {
        BezierPath path = (BezierPath)target;
        if (path.controlPoints.Count < 4) return;

        // 控制点可拖拽
        for (int i = 0; i < path.controlPoints.Count; i++)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 worldPos = path.transform.TransformPoint(path.controlPoints[i]);
            Vector3 newWorldPos = Handles.PositionHandle(worldPos, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(path, "Move Bezier Control Point");
                path.controlPoints[i] = path.transform.InverseTransformPoint(newWorldPos);
            }
        }

        // 绘制曲线
        Handles.color = Color.green;
        Vector3 prev = path.transform.TransformPoint(path.GetPoint(0));
        for (int i = 1; i <= path.CurveResolution; i++)
        {
            float t = i / (float)path.CurveResolution;
            Vector3 point = path.transform.TransformPoint(path.GetPoint(t));
            Handles.DrawLine(prev, point);
            prev = point;
        }

        // 绘制控制点连线
        Handles.color = Color.gray;
        for (int i = 0; i < path.controlPoints.Count - 1; i++)
        {
            Handles.DrawDottedLine(
                path.transform.TransformPoint(path.controlPoints[i]),
                path.transform.TransformPoint(path.controlPoints[i + 1]), 4f);
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BezierPath path = (BezierPath)target;
        if (GUILayout.Button("添加控制点"))
        {
            Undo.RecordObject(path, "Add Control Point");
            Vector3 last = path.controlPoints[path.controlPoints.Count - 1];
            path.controlPoints.Add(last + Vector3.right);
        }
        if (path.controlPoints.Count > 4 && GUILayout.Button("删除最后一个控制点"))
        {
            Undo.RecordObject(path, "Remove Control Point");
            path.controlPoints.RemoveAt(path.controlPoints.Count - 1);
        }
    }
}