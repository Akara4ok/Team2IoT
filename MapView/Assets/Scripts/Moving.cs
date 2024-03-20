using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Moving : MonoBehaviour
{
    [SerializeField]
    List<(double x, double y)> coordinates;
    [SerializeField]
    float speed = 10;


    private int index = 0;
    private Vector3 target;
    private Vector3 nextTarget;

    void Start()
    {
        string filePath = "Assets/testData/gps.csv";
        coordinates = ReadCSV.ReadCsvFile(filePath);
        if (coordinates.Count > 0 )
            target = new Vector3((float)coordinates[index].x, 0, (float)coordinates[index].y);
    }

    private void Update()
    {
        Move();
    }

    void Move()
    {
        if (index < coordinates.Count)
        {
            target = new Vector3((float)coordinates[index].x, 0, (float)coordinates[index].y);
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if (transform.position == target)
                index++;
        }
}
    }
