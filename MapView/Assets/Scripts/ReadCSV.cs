using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

//TEMP FILE JUST TO CHECK
class ReadCSV
{
    private const int EarthRadius = 6371;
    public static Queue<(float x, float y)> ReadCsvFile(string filePath)
    {
        Queue<(float x, float y)> coordinates = new();

        (float x, float y) startingCoords = (0, 0);
        try
        {
            using StreamReader reader = new(filePath);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] fields = line.Split(',');

                if (fields.Length >= 2 && float.TryParse(fields[0], NumberStyles.Any, CultureInfo.InvariantCulture, out float longitude)
                    && float.TryParse(fields[1], NumberStyles.Any, CultureInfo.InvariantCulture, out float latitude))
                {
                    if (coordinates.Count == 0)
                    {
                        startingCoords = Gps.GeographicCoordsToXY(longitude, latitude);
                        coordinates.Enqueue(startingCoords);
                    }
                    else coordinates.Enqueue(Gps.GeographicCoordsToXY(longitude, latitude, startingCoords));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return coordinates;
    }
}
