using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SettingsLoader : MonoBehaviour
{
    public static string SettingsPath;
    public static bool RandomPoints;
    public static bool RandomColors;
    public static float Speed;
    public static float ConeRadiusMultiplier;
    public static int ConeSegments;
    public static int AmountOfRandomPoints;
    public static bool DrawVertices;
    public static bool Orthographic;
    public static bool ShowSeeds;
    public static bool Isometric;
    public static string Shader;
    public static List<Vector3> Points = new List<Vector3>();
    public static RandomRange RandomHeightRange;
    public static List<Color> Colors = new List<Color>();
    public static event Action OnSettingsLoaded;
    public static Color BackgroundColor;

    void Start()
    {
        LoadApplication();
    }

    void LoadApplication()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "application.json");
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            ApplicationData loadedData = JsonUtility.FromJson<ApplicationData>(dataAsJson);

            SettingsPath = loadedData.SettingsPath;

            LoadSettings(SettingsPath);
        }
        else
        {
            Debug.LogError("Cannot find settings file.");
        }
    }

    void LoadSettings(String settingsPath)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, settingsPath);
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            SettingsData loadedData = JsonUtility.FromJson<SettingsData>(dataAsJson);

            // Assigning the values to static variables
            RandomPoints = loadedData.RandomPoints;
            RandomColors = loadedData.RandomColors;
            Speed = loadedData.Speed;
            Orthographic = loadedData.Orthographic;
            ShowSeeds = loadedData.ShowSeeds;
            Isometric = loadedData.Isometric;
            Shader = loadedData.Shader;
            ConeRadiusMultiplier = loadedData.ConeRadiusMultiplier;
            ConeSegments = loadedData.ConeSegments;
            DrawVertices = loadedData.DrawVertices;
            AmountOfRandomPoints = loadedData.AmountOfRandomPoints;
            RandomHeightRange = loadedData.RandomHeightRange;
            Color backgroundColor;
            if (ColorUtility.TryParseHtmlString(loadedData.BackgroundColor, out backgroundColor))
            {
                BackgroundColor = backgroundColor;
            }

            // Points
            foreach (var point in loadedData.Points)
            {
                Points.Add(new Vector3(point.x, point.y, point.z));
            }

            // Colors
            foreach (var hex in loadedData.Colors)
            {
                Color color;
                if (ColorUtility.TryParseHtmlString(hex, out color))
                {
                    Colors.Add(color);
                }
            }

            OnSettingsLoaded?.Invoke();
        }
        else
        {
            Debug.LogError("Cannot find settings file.");
        }
    }

    [System.Serializable]
    public class ApplicationData
    {
        public string SettingsPath;
    }

    [System.Serializable]
    public class SettingsData
    {
        public bool RandomPoints;
        public bool RandomColors;
        public float Speed;
        public float ConeRadiusMultiplier;
        public int ConeSegments;
        public int AmountOfRandomPoints;
        public bool DrawVertices;
        public bool Orthographic;
        public bool ShowSeeds;
        public bool Isometric;
        public string Shader;
        public RandomRange RandomHeightRange;
        public List<Point> Points;
        public List<string> Colors;
        public string BackgroundColor;
    }

    [System.Serializable]
    public class Point
    {
        public float x, y, z;
    }

    [System.Serializable]
    public class RandomRange
    {
        public float min, max;
    }

    public static Vector3[] GetPointsArray()
    {
        return Points.ToArray();
    }
}
