using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RoadCreator : MonoBehaviour
{

    [HideInInspector]
    public Road road;

    public void CreatePath()
    {
        road = new Road(transform.position);
    }
}