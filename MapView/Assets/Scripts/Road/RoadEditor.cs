using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(RoadCreator))]
public class RoadEditor : Editor
{

    RoadCreator creator;
    Road road;

    void OnSceneGUI()
    {
        Input();
        Draw();
    }

    void Input()
    {
        Event guiEvent = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            Undo.RecordObject(creator, "Add segment");
            road.AddSegment(mousePos);
        }
    }

    void Draw()
    {
        for (int i = 0; i < road.NumSegments; i++)
        {
            Vector2[] points = road.GetPointsInSegment(i);
            Handles.color = Color.black;
            Handles.DrawLine(points[1], points[0]);
            Handles.DrawLine(points[2], points[3]);
            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2);
        }

        Handles.color = Color.red;
        for (int i = 0; i < road.NumPoints; i++)
        {
            var fmh_47_62_638465681363664261 = Quaternion.identity; Vector2 newPos = Handles.FreeMoveHandle(road[i], .1f, Vector2.zero, Handles.CylinderHandleCap);
            if (road[i] != newPos)
            {
                Undo.RecordObject(creator, "Move point");
                road.MovePoint(i, newPos);
            }
        }
    }

    void OnEnable()
    {
        creator = (RoadCreator)target;
        if (creator.road == null)
        {
            creator.CreatePath();
        }
        road = creator.road;
    }
}