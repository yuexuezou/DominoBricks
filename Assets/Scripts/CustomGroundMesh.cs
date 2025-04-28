using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class CustomGroundMesh : MonoBehaviour
{
    public int xSegments = 20;
    public int zSegments = 20;
    public float width = 10f;
    public float length = 10f;
    public float thickness = 1f;
    public float waveHeight = 1f;
    public float waveFrequency = 1f;

    public void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        int vertCount = (xSegments + 1) * (zSegments + 1) * 2;
        Vector3[] vertices = new Vector3[vertCount];
        Vector3[] normals = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int[] triangles = new int[xSegments * zSegments * 12];

        // 上表面
        for (int z = 0; z <= zSegments; z++)
        {
            for (int x = 0; x <= xSegments; x++)
            {
                float xPos = (x / (float)xSegments - 0.5f) * width;
                float zPos = (z / (float)zSegments - 0.5f) * length;
                float yPos = Mathf.Sin(xPos * waveFrequency) * waveHeight;
                vertices[z * (xSegments + 1) + x] = new Vector3(xPos, yPos, zPos);
                normals[z * (xSegments + 1) + x] = Vector3.up;
                uvs[z * (xSegments + 1) + x] = new Vector2(x / (float)xSegments, z / (float)zSegments);
            }
        }
        // 下表面
        int offset = (xSegments + 1) * (zSegments + 1);
        for (int z = 0; z <= zSegments; z++)
        {
            for (int x = 0; x <= xSegments; x++)
            {
                float xPos = (x / (float)xSegments - 0.5f) * width;
                float zPos = (z / (float)zSegments - 0.5f) * length;
                float yPos = Mathf.Sin(xPos * waveFrequency) * waveHeight - thickness;
                vertices[offset + z * (xSegments + 1) + x] = new Vector3(xPos, yPos, zPos);
                normals[offset + z * (xSegments + 1) + x] = Vector3.down;
                uvs[offset + z * (xSegments + 1) + x] = new Vector2(x / (float)xSegments, z / (float)zSegments);
            }
        }
        // 三角面
        int tri = 0;
        for (int z = 0; z < zSegments; z++)
        {
            for (int x = 0; x < xSegments; x++)
            {
                int i0 = z * (xSegments + 1) + x;
                int i1 = i0 + 1;
                int i2 = i0 + (xSegments + 1);
                int i3 = i2 + 1;
                int j0 = offset + i0;
                int j1 = offset + i1;
                int j2 = offset + i2;
                int j3 = offset + i3;
                // 上表面
                triangles[tri++] = i0; triangles[tri++] = i2; triangles[tri++] = i1;
                triangles[tri++] = i1; triangles[tri++] = i2; triangles[tri++] = i3;
                // 下表面
                triangles[tri++] = j0; triangles[tri++] = j1; triangles[tri++] = j2;
                triangles[tri++] = j1; triangles[tri++] = j3; triangles[tri++] = j2;
            }
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void OnValidate()
    {
        GenerateMesh();
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(CustomGroundMesh))]
public class CustomGroundMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CustomGroundMesh mesh = (CustomGroundMesh)target;
        if (GUILayout.Button("生成地面Mesh"))
        {
            mesh.GenerateMesh();
        }
    }
}
#endif
