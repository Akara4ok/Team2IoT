using UnityEngine;
using System.Collections.Generic;

public class Moving : MonoBehaviour
{
    [SerializeField]
    List<(double x, double y)> coordinates;
    [SerializeField]
    float speed = 10;
    [SerializeField]
    RoadCreator roadCreator;
    [SerializeField]
    DriveCreator driveCreator;
    [SerializeField]
    private int index = 0;
    private Vector3 target;
    private Vector3 nextTarget;

    void Start()
    {
        string filePath = "Assets/testData/gps.csv";
        coordinates = ReadCSV.ReadCsvFile(filePath);
        if (coordinates.Count > 1)
        {
            target = new Vector3((float)coordinates[index].x, 0, (float)coordinates[index].y);
            nextTarget = new Vector3((float)coordinates[index + 1].x, 0, (float)coordinates[index + 1].y);
            roadCreator.road.MovePoint(2, target);
            roadCreator.road.MovePoint(3, target);
            roadCreator.road.AddSegment(nextTarget);
            index++;
        }
    }

    private void Update()
    {
        Move();
    }

    void Move()
    {
        if (index < coordinates.Count)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.x, .25f, target.z), speed * Time.deltaTime);
            if (transform.position.x == target.x && transform.position.z == target.z)
            {
                target = nextTarget;
                if (index + 1 < coordinates.Count)
                {
                    nextTarget = new Vector3((float)coordinates[index + 1].x, 0, (float)coordinates[index + 1].y);
                    roadCreator.road.AddSegment(nextTarget);
                    driveCreator.UpdateRoad();
                }
                if (index - 1> 0)
                {
                    roadCreator.road.DeleteSegment(0);
                    driveCreator.UpdateRoad();
                }
                index++;
            }
        }
    }
}
