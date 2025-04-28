using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BrickPlacer : MonoBehaviour
{
    public BezierPath bezierPath;
    public GameObject brickPrefab;
    public int brickCount = 10;
    public float brickSpacing = 1.0f;
    public bool autoUpdate = true;

    public void PlaceBricks()
    {
        if (bezierPath == null || brickPrefab == null || brickCount < 1) return;
        // 清理旧砖头
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            #if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(i).gameObject);
            #else
            Destroy(transform.GetChild(i).gameObject);
            #endif
        }
        // 沿曲线生成砖头
        for (int i = 0; i < brickCount; i++)
        {
            float t = brickCount == 1 ? 0.5f : i / (float)(brickCount - 1);
            Vector3 pos = bezierPath.GetPoint(t);
            GameObject brick =
                #if UNITY_EDITOR
                (GameObject)PrefabUtility.InstantiatePrefab(brickPrefab, transform);
                #else
                Instantiate(brickPrefab, transform);
                #endif
            brick.transform.localPosition = pos;
            // 可根据曲线切线调整砖头朝向
            if (i < brickCount - 1)
            {
                Vector3 nextPos = bezierPath.GetPoint(Mathf.Min(1f, t + 1f / (brickCount - 1)));
                brick.transform.forward = (nextPos - pos).normalized;
            }
        }
    }

    private void OnValidate()
    {
        if (autoUpdate) PlaceBricks();
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(BrickPlacer))]
public class BrickPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BrickPlacer placer = (BrickPlacer)target;
        if (GUILayout.Button("生成砖头"))
        {
            placer.PlaceBricks();
        }
    }
}
#endif
