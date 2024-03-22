using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoadCreator))]
public class RoadEditor : Editor
{

    RoadCreator creator;
    Road Road
    {
        get { return creator.road; }
    }

    const float segmentSelectDistanceThreshold = .1f;
    int selectedSegmentIndex = -1;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        if (GUILayout.Button("Create new"))
        {
            Undo.RecordObject(creator, "Create new");
            creator.CreateRoad();
        }

        bool isClosed = GUILayout.Toggle(Road.IsClosed, "Closed");
        if (isClosed != Road.IsClosed)
        {
            Undo.RecordObject(creator, "Toggle closed");
            Road.IsClosed = isClosed;
        }

        bool autoSetControlPoints = GUILayout.Toggle(Road.AutoSetControlPoints, "Auto Set Control Points");
        if (autoSetControlPoints != Road.AutoSetControlPoints)
        {
            Undo.RecordObject(creator, "Toggle auto set controls");
            Road.AutoSetControlPoints = autoSetControlPoints;
        }

        if (EditorGUI.EndChangeCheck())
            SceneView.RepaintAll();
    }

    void OnSceneGUI()
    {
        Input();
        Draw();
    }

    void Input()
    {
        Event guiEvent = Event.current;
        Vector3 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            if (selectedSegmentIndex != -1)
            {
                Undo.RecordObject(creator, "Split segment");
                Road.SplitSegment(mousePos, selectedSegmentIndex);
            }
            else if (!Road.IsClosed)
            {
                Undo.RecordObject(creator, "Add segment");
                Road.AddSegment(mousePos);
            }
        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1)
        {
            float minDstToAnchor = creator.anchorDiameter * .5f;
            int closestAnchorIndex = -1;

            for (int i = 0; i < Road.NumPoints; i += 3)
            {
                float dst = Vector2.Distance(mousePos, Road[i]);
                if (dst < minDstToAnchor)
                {
                    minDstToAnchor = dst;
                    closestAnchorIndex = i;
                }
            }

            if (closestAnchorIndex != -1)
            {
                Undo.RecordObject(creator, "Delete segment");
                Road.DeleteSegment(closestAnchorIndex);
            }
        }

        if (guiEvent.type == EventType.MouseMove)
        {
            float minDstToSegment = segmentSelectDistanceThreshold;
            int newSelectedSegmentIndex = -1;

            for (int i = 0; i < Road.NumSegments; i++)
            {
                Vector3[] points = Road.GetPointsInSegment(i);
                float dst = HandleUtility.DistancePointBezier(mousePos, points[0], points[3], points[1], points[2]);
                if (dst < minDstToSegment)
                {
                    minDstToSegment = dst;
                    newSelectedSegmentIndex = i;
                }
            }

            if (newSelectedSegmentIndex != selectedSegmentIndex)
            {
                selectedSegmentIndex = newSelectedSegmentIndex;
                HandleUtility.Repaint();
            }
        }

        HandleUtility.AddDefaultControl(0);
    }

    void Draw()
    {
        for (int i = 0; i < Road.NumSegments; i++)
        {
            Vector3[] points = Road.GetPointsInSegment(i);
            if (creator.displayControlPoints)
            {
                Handles.color = Color.black;
                Handles.DrawLine(points[1], points[0]);
                Handles.DrawLine(points[2], points[3]);
            }
            Color segmentCol = (i == selectedSegmentIndex && Event.current.shift) ? creator.selectedSegmentCol : creator.segmentCol;
            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.red, null, 10f);
        }

        for (int i = 0; i < Road.NumPoints; i++)
        {
            if (i % 3 == 0 || creator.displayControlPoints)
            {
                Handles.color = (i % 3 == 0) ? creator.anchorCol : creator.controlCol;
                float handleSize = (i % 3 == 0) ? creator.anchorDiameter : creator.controlDiameter;
                Vector3 newPos = Handles.FreeMoveHandle(Road[i], .1f, Vector3.zero, Handles.CylinderHandleCap);
                if (Road[i] != newPos)
                {
                    Undo.RecordObject(creator, "Move point");
                    Road.MovePoint(i, newPos);
                }
            }
        }
    }

    void OnEnable()
    {
        creator = (RoadCreator)target;
        if (creator.road == null)
            creator.CreateRoad();
    }
}