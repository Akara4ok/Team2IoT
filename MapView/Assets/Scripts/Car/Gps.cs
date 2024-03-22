using UnityEngine;
using System.Collections.Generic;

public class Gps : MonoBehaviour
{
    [SerializeField] bool fromCSV = false;
    const string CsvFilePath = "Assets/testData/gps.csv";

    private List<(float x, float y)> coordinates;
    [SerializeField] int index = 0;


    void Awake()
    {
        if (fromCSV)
            coordinates = ReadCSV.ReadCsvFile(CsvFilePath);
    }

    public bool GetNext(out Vector3 nextCoords, out RoadState state, float defaultY = 0)
    {
        nextCoords = new(0, defaultY, 0);
        state = RoadState.Normal;
        if (fromCSV)
        {
            if (index == coordinates.Count)
                return false;
            nextCoords.x = coordinates[index].x;
            nextCoords.z = coordinates[index].y;
            state = (RoadState)Random.Range(0, 2);
        }
        else
        {
            //WEBSOCKET CONNECTION HERE
            return false;
        }
        index++;
        return true;
    }
}
