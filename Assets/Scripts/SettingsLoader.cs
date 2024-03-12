using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SettingsLoader : MonoBehaviour
{
    public static bool RandomPoints;
    public static bool RandomColors;
    public static float Speed;
    public static float ConeRadius;
    public static int ConeSegments;
    public static int AmountOfRandomPoints;
    public static bool DrawVertices;
    public static bool Orthographic;
    public static bool ShowSeeds;
    public static bool Isometric;
    public static string Shader;
    public static List<Vector3> Points = new List<Vector3>();
    public static List<Color> Colors = new List<Color>();
    public static event Action OnSettingsLoaded;

    void Start()
    {
        LoadSettings();
    }

    void LoadSettings()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "settings.json");
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
            ConeRadius = loadedData.ConeRadius;
            ConeSegments = loadedData.ConeSegments;
            DrawVertices = loadedData.DrawVertices;
            AmountOfRandomPoints = loadedData.AmountOfRandomPoints;

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
    public class SettingsData
    {
        public bool RandomPoints;
        public bool RandomColors;
        public float Speed;
        public float ConeRadius;
        public int ConeSegments;
        public int AmountOfRandomPoints;
        public bool DrawVertices;
        public bool Orthographic;
        public bool ShowSeeds;
        public bool Isometric;
        public string Shader;
        public List<Point> Points;
        public List<string> Colors;
    }

    [System.Serializable]
    public class Point
    {
        public float x, y, z;
    }

    public static Vector3[] GetPointsArray()
    {
        return Points.ToArray();
    }
}
