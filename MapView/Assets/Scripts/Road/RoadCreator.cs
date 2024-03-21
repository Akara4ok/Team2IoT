using UnityEngine;

public class RoadCreator : MonoBehaviour
{

    [HideInInspector]
    public Road road;

    public Color anchorCol = Color.red;
    public Color controlCol = Color.white;
    public Color segmentCol = Color.green;
    public Color selectedSegmentCol = Color.yellow;
    public float anchorDiameter = .1f;
    public float controlDiameter = .075f;
    public bool displayControlPoints = true;

    public void CreateRoad() => road = new Road(transform.position);
}