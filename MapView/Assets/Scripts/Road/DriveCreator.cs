using System;
using UnityEngine;

[RequireComponent(typeof(RoadCreator))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DriveCreator : MonoBehaviour
{
    [Range(.05f, 1.5f)]
    public float spacing = 1;
    public float roadWidth = 1;
    public bool autoUpdate;
    public float tiling = 1;
    public RoadState state = RoadState.Normal;

    public void UpdateRoad()
    {
        Road road = GetComponent<RoadCreator>().road;
        Vector3[] points = road.CalculateEvenlySpacedPoints(spacing);
        GetComponent<MeshFilter>().mesh = CreateRoadMesh(points, road.IsClosed);

        int textureRepeat = Mathf.RoundToInt(tiling * points.Length * spacing * .05f);
        GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(1, textureRepeat);

        GetComponent<Renderer>().sharedMaterial.color = StateToColor(state);
    }

    Mesh CreateRoadMesh(Vector3[] points, bool isClosed)
    {
        Vector3[] verts = new Vector3[points.Length * 2];
        Vector2[] uvs = new Vector2[verts.Length];
        int numTris = 2 * (points.Length - 1) + ((isClosed) ? 2 : 0);
        int[] tris = new int[numTris * 3];
        int vertIndex = 0;
        int triIndex = 0;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 forward = Vector3.zero;
            if (i < points.Length - 1 || isClosed)
                forward += points[(i + 1) % points.Length] - points[i];
            if (i > 0 || isClosed)
                forward += points[i] - points[(i - 1 + points.Length) % points.Length];

            forward.Normalize();
            Vector3 left = new(-forward.z, 0, forward.x);

            verts[vertIndex] = points[i] + .5f * roadWidth * left;
            verts[vertIndex + 1] = points[i] - .5f * roadWidth * left;

            float completionPercent = i / (float)(points.Length - 1);
            float v = 1 - Mathf.Abs(2 * completionPercent - 1);
            uvs[vertIndex] = new(0, v);
            uvs[vertIndex + 1] = new(1, v);

            if (i < points.Length - 1 || isClosed)
            {
                tris[triIndex] = vertIndex;
                tris[triIndex + 1] = (vertIndex + 2) % verts.Length;
                tris[triIndex + 2] = vertIndex + 1;

                tris[triIndex + 3] = vertIndex + 1;
                tris[triIndex + 4] = (vertIndex + 2) % verts.Length;
                tris[triIndex + 5] = (vertIndex + 3) % verts.Length;
            }

            vertIndex += 2;
            triIndex += 6;
        }

        Mesh mesh = new()
        {
            vertices = verts,
            triangles = tris,
            uv = uvs
        };

        return mesh;
    }

    public static Color StateToColor(RoadState state)
    {
        return state switch
        {
            RoadState.Good => Color.green,
            RoadState.Normal => Color.yellow,
            RoadState.Bad => Color.red,
            _ => Color.gray
        };
    }
}