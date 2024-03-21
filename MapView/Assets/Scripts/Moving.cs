using UnityEngine;
using System.Collections.Generic;

public class Moving : MonoBehaviour
{
    [SerializeField]
    private List<(double x, double y)> coordinates;
    [field: SerializeField]
    public float Speed { private set; get; }
    [field: SerializeField]
    public float RotationSpeed { private set; get; }

    [SerializeField]
    private RoadCreator roadCreator;
    [SerializeField]
    private DriveCreator driveCreator;

    [field: SerializeField]
    public int index = 0;

    public Vector3 Target { private set; get; }
    private Vector3 nextTarget;

   
    void Start()
    {
        string filePath = "Assets/testData/gps.csv";
        coordinates = ReadCSV.ReadCsvFile(filePath);
        if (coordinates.Count > 1)
        {
            Target = new Vector3((float)coordinates[index].x, 0, (float)coordinates[index].y);
            nextTarget = new Vector3((float)coordinates[index + 1].x, 0, (float)coordinates[index + 1].y);
            roadCreator.road.MovePoint(2, Target);
            roadCreator.road.MovePoint(3, Target);
            roadCreator.road.AddSegment(nextTarget);
            index++;
        }
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (index <= coordinates.Count)
        {
            transform.position = Vector3.MoveTowards(transform.position, Target, Speed * Time.deltaTime);

            Rotate();

            if (transform.position.x == Target.x && transform.position.z == Target.z)
            {
                Target = nextTarget;
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

    private void Rotate()
    {
        Vector3 dirMovement = Target - transform.position;
        if (dirMovement != Vector3.zero)
        {
            dirMovement.Normalize();
            Quaternion quaternion = Quaternion.LookRotation(dirMovement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, quaternion, RotationSpeed * Speed * Time.deltaTime);
        }
    }
}
