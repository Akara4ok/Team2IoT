using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

//TEMP FILE JUST TO CHECK
class ReadCSV
{
    private const int EarthRadius = 6371;
    public static List<(double x, double y)> ReadCsvFile(string filePath)
    {
        List<(double x, double y)> coordinates = new();

        double startingX = 0;
        double startingY = 0;
        try
        {
            using StreamReader reader = new(filePath);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] fields = line.Split(',');

                if (fields.Length >= 2 && double.TryParse(fields[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double longitude)
                    && double.TryParse(fields[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double latitude))
                {
                    double x = EarthRadius * longitude * Math.Cos(latitude);
                    double y = EarthRadius * latitude;
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
