using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

//TEMP FILE JUST TO CHECK
class ReadCSV
{
    private const int EarthRadius = 6371;
    public static List<(float x, float y)> ReadCsvFile(string filePath)
    {
        List<(float x, float y)> coordinates = new();

        float startingX = 0;
        float startingY = 0;
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
                    float x = EarthRadius * longitude * (float)Math.Cos(latitude);
                    float y = EarthRadius * latitude;
                    if (coordinates.Count == 0)
                    {
                        startingX = x;
                        startingY = y;
                    }
                    coordinates.Add((x - startingX, y - startingY));
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
