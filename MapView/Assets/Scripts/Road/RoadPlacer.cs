using UnityEngine;

public class PathPlacer : MonoBehaviour
{

    public float spacing = .1f;
    public float resolution = 1;


    void Start()
    {
        Vector3[] points = FindObjectOfType<RoadCreator>().road.CalculateEvenlySpacedPoints(spacing, resolution);
        foreach (Vector2 p in points)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.position = p;
            g.transform.localScale = .5f * spacing * Vector3.one;
        }
    }


}