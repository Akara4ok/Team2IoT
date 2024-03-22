using System;
using UnityEngine;
using System.Linq;
using WebSocketSharp;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class Gps : MonoBehaviour
{
    private const int EarthRadius = 6371;
    private bool _haveStarting;
    private (float x, float y) startingCoords;

    [SerializeField] bool fromCSV = false;
    const string CsvFilePath = "Assets/testData/gps.csv";

    private WebSocket ws;
    const string WebSocketPath = "store://localhost:8000";

    private Queue<(float x, float y)> coordinates = new();
    private Queue<RoadState> states = new();

    void Awake()
    {
        if (fromCSV)
            coordinates = ReadCSV.ReadCsvFile(CsvFilePath);
        else
        {
            ws = new WebSocket(WebSocketPath);
            ws.OnMessage += (sender, e) =>
            {
                List<string> data = e.Data.Split(' ').ToList();
                if (data.Count < 3)
                    return;
                if (!float.TryParse(data[0] ,out float longitude) || !float.TryParse(data[1], out float latitude))
                    return;
                if (!Enum.TryParse(data[2], out RoadState state))
                    return;
                coordinates.Enqueue(GetCoords(longitude, latitude));
                states.Enqueue(state);
            };
            ws.Connect();
        }
    }

    public bool GetNext(out Vector3 nextCoords, out RoadState state, float defaultY = 0)
    {
        nextCoords = new(0, defaultY, 0);
        state = RoadState.Normal;
        if (coordinates.Count == 0)
            return false;
        var (x, y) = coordinates.Dequeue();
        nextCoords.x = x;
        nextCoords.z = y;
        if (fromCSV)
            state = (RoadState)Random.Range(0, 2);
        else
            state = states.Dequeue();
        return true;
    }

    private (float x, float y) GetCoords(float longitude, float latitude)
    {
        (float x, float y) coords;
        if (!_haveStarting)
        {
            coords = GeographicCoordsToXY(longitude, latitude);
            startingCoords = coords;
            _haveStarting = true;
        }
        else coords = GeographicCoordsToXY(longitude, latitude, startingCoords);
        return coords;
    }

    public static (float x, float y) GeographicCoordsToXY(float longitude, float latitude, (float x, float y) startingCoords)
    {
        (float x, float y) = GeographicCoordsToXY(longitude, latitude);
        return (x - startingCoords.x, y - startingCoords.y);
    }

    public static (float x, float y) GeographicCoordsToXY(float longitude, float latitude)
    {
        float x = EarthRadius * longitude * (float)Mathf.Cos(latitude);
        float y = EarthRadius * latitude;
        return (x, y);
    }
}
