using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Moving : MonoBehaviour
{
    [SerializeField]
    List<(double x, double y)> coordinates;
    [SerializeField]
    float speed;
    [SerializeField]
    private int index = 0;
    [SerializeField]
    private Vector3 target;

    [SerializeField]
    float rotationSpeed = 1;

    void Start()
    {
        string filePath = "Assets/testData/gps.csv";
        coordinates = ReadCSV.ReadCsvFile(filePath);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (index < coordinates.Count)
        {
            target = new Vector3((float)coordinates[index].x, 0, (float)coordinates[index].y);
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (target != Vector3.zero)
            {
                Vector3 dirMovement = new Vector3(target.x, target.y, target.z); 
                dirMovement.Normalize();
                Quaternion quaternion = Quaternion.LookRotation(dirMovement, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, quaternion, rotationSpeed * Time.deltaTime);
            }

            if (transform.position == target)
                index++;
        }
}
    }
