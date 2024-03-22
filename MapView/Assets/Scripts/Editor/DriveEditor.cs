using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DriveCreator))]
public class DriveEditor : Editor
{

    DriveCreator creator;

    void OnSceneGUI()
    {
        if (creator.autoUpdate && Event.current.type == EventType.Repaint)
            creator.UpdateRoad();
    }

    void OnEnable()
    {
        creator = (DriveCreator)target;
    }
}