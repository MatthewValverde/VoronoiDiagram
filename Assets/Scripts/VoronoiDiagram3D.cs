using System.Collections.Generic;
using UnityEngine;
using MIConvexHull;

[RequireComponent(typeof(MeshRenderer))]
public class VoronoiDiagram3D : MonoBehaviour
{
    public bool animate = false;
    public bool useRandomPoints = false;
    public bool withRandomZ = false;
    public int totalRandomPoints = 50;
    [SerializeField] private int gridSize = 10;
    private MeshRenderer meshRenderer;
    private Material material;
    public Vector3[] pointPositions;
    public Color[] colors;
    public float speed = 5f;
    public Rect boundary;
    private Vector3[] directions; // Array to hold directions corresponding to points
    // Data structure for Voronoi corners to be calculated
    private List<List<Vector2>> voronoiCorners;
    private GameObject[] cones;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material; // Ensure this material uses your custom shader
    }
    void OnEnable()
    {
        SettingsLoader.OnSettingsLoaded += SettingsLoaded;
    }

    void OnDisable()
    {
        SettingsLoader.OnSettingsLoaded -= SettingsLoaded;
    }

    private void Start()
    {

    }

    private void SettingsLoaded()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.orthographic = SettingsLoader.Orthographic;
            if (SettingsLoader.Isometric)
            {
                mainCamera.transform.position = new Vector3(-3.271372f, 4.229227f, 4.293624f);
                mainCamera.transform.eulerAngles = new Vector3(33, 144, 0);
            }
        }

        if (SettingsLoader.RandomPoints)
        {
            pointPositions = GenerateRandomPoints(SettingsLoader.AmountOfRandomPoints);
        }
        else
        {
            pointPositions = SettingsLoader.GetPointsArray();
        }

        directions = new Vector3[pointPositions.Length];
        cones = new GameObject[pointPositions.Length];

        for (int i = 0; i < pointPositions.Length; i++)
        {
            directions[i] = GetRandomDirection();
        }

        GenerateDiagram();
        GenerateAndRenderCones();
    }

    public Vector3[] GenerateRandomPoints(int numberOfPoints)
    {
        Vector3[] points = new Vector3[numberOfPoints];

        for (int i = 0; i < numberOfPoints; i++)
        {
            float z = 2.0f;
            float x = Random.Range(0.0f, 1.0f);
            float y = Random.Range(0.0f, 1.0f);
            if (withRandomZ) z = Random.Range(2.0f, 3.0f);
            points[i] = (new Vector3(x, y, z));
        }

        return points;
    }

    private Vector3 GetRandomDirection()
    {
        Vector3 temp = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
        ; return temp;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // Toggle the bool value
            animate = !animate;
        }

        if (animate) UpdatePointPositions();
    }

    private void GenerateDiagram()
    {
        int numPoints = pointPositions.Length;
        float[] pointData = new float[numPoints * 3];
        for (int i = 0; i < numPoints; i++)
        {
            pointData[i * 3] = pointPositions[i].x;
            pointData[i * 3 + 1] = pointPositions[i].y;
            pointData[i * 3 + 2] = pointPositions[i].z;
        }

        voronoiCorners = CalculateVoronoiCorners(pointPositions);

        /*var materialProperty = new MaterialPropertyBlock();
        materialProperty.SetFloatArray("_Points", pointData);
        materialProperty.SetInt("_NumPoints", numPoints);
        material.SetColorArray("_Colors", colors);
        GetComponent<Renderer>().SetPropertyBlock(materialProperty);*/
    }

    public float To3D(float input)
    {
        return 0 - (input - 0.5f) * gridSize;
    }

    void GenerateAndRenderCones()
    {
        for (int i = 0; i < pointPositions.Length; i++)
        {
            Vector3 position = new Vector3(To3D(pointPositions[i].x), 0, To3D(pointPositions[i].y));
            Color color;
            if (SettingsLoader.RandomColors)
            {
                color = new Color(Random.value, Random.value, Random.value);
            }
            else
            {
                try
                {
                    color = SettingsLoader.Colors[i];
                }
                catch (System.ArgumentOutOfRangeException e)
                {
                    color = new Color(Random.value, Random.value, Random.value);
                }
                catch (System.Exception e)
                {
                    color = new Color(Random.value, Random.value, Random.value);
                }
               
            }
            GameObject cone = GenerateCone("Cone_" + i, pointPositions[i].z, SettingsLoader.ConeRadius, SettingsLoader.ConeSegments, position, color);
            cones[i] = cone;
        }
    }
    void UpdateConesPosition()
    {
        for (int i = 0; i < pointPositions.Length; i++)
        {
            Vector3 position = new Vector3(To3D(pointPositions[i].x), 0, To3D(pointPositions[i].y));
            if (cones[i] != null) cones[i].transform.position = position;
        }
    }

    GameObject GenerateCone(string name, float height, float bottomRadius, int numVertices, Vector3 position, Color color)
    {
        GameObject coneObject = new GameObject(name);

        // Add a MeshFilter component to the GameObject
        MeshFilter meshFilter = coneObject.AddComponent<MeshFilter>();

        // Add a MeshRenderer component to the GameObject
        MeshRenderer meshRenderer = coneObject.AddComponent<MeshRenderer>();

        // Call the CreateConeMesh method to generate the cone mesh
        Mesh coneMesh = ConeMesh.CreateConeMesh(height, bottomRadius, numVertices);

        // Assign the generated mesh to the MeshFilter
        meshFilter.mesh = coneMesh;

        // Optionally, assign a material to the MeshRenderer
        // For this example, we'll use a default material
        meshRenderer.material = new Material(Shader.Find(SettingsLoader.Shader));
        meshRenderer.material.color = color;

        // Set the position of the cone
        coneObject.transform.position = position;

        // Add a MeshRenderer component to the GameObject
        if (SettingsLoader.DrawVertices)
        {
            LineRenderer lineRenderer = coneObject.AddComponent<LineRenderer>();
            ConeMesh.RenderLine(coneObject, lineRenderer, Color.black);
        }

        if (SettingsLoader.ShowSeeds)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            float sphereSize = 0.03f;
            sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
            sphere.transform.position = new Vector3(position.x, height, position.z);
            Material sphereMaterial = new Material(Shader.Find("Unlit/Color"));
            sphereMaterial.color = Color.black;
            sphere.GetComponent<Renderer>().material = sphereMaterial;
            sphere.transform.parent = coneObject.transform;
        }

        return coneObject;
    }

    private void UpdatePointPositions()
    {
        for (int i = 0; i < pointPositions.Length; i++)
        {
            // Normalize the direction to ensure uniform speed
            Vector3 normalizedDirection = directions[i].normalized;

            // Move point
            pointPositions[i] += normalizedDirection * SettingsLoader.Speed * Time.deltaTime;

            // Check for boundaries and reflect direction if needed
            if (pointPositions[i].x <= boundary.xMin || pointPositions[i].x >= boundary.xMax)
            {
                directions[i] = new Vector3(-directions[i].x, directions[i].y, 0).normalized; // Normalize the new direction as well
            }
            if (pointPositions[i].y <= boundary.yMin || pointPositions[i].y >= boundary.yMax)
            {
                directions[i] = new Vector3(directions[i].x, -directions[i].y, 0).normalized; // Normalize the new direction as well
            }
        }
        GenerateDiagram();
        UpdateConesPosition();
    }

    // since the Voronoi calculator only deals with 2D points
    private List<Vector2> ConvertTo2DPoints(Vector3[] threeDPoints)
    {
        List<Vector2> twoDPoints = new List<Vector2>();
        foreach (var point in threeDPoints)
        {
            twoDPoints.Add(new Vector2(point.x, point.y)); // Assuming the Z-axis is ignored for 2D Voronoi
        }
        return twoDPoints;
    }

    // Update the CalculateVoronoiCorners method signatures to fit your context
    private List<List<Vector2>> CalculateVoronoiCorners(Vector3[] points3D)
    {
        var points2D = ConvertTo2DPoints(points3D);
        // Your existing CalculateVoronoiCorners logic with the 2D points
        return voronoiCorners;
    }
}
