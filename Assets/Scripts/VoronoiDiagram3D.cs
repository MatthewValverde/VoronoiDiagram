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

    private void Start()
    {
        if (useRandomPoints)
        {
            pointPositions = GenerateRandomPoints(totalRandomPoints);
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
            if(withRandomZ) z = Random.Range(2.0f, 3.0f);
            points[i] = (new Vector3(x, y, z));
        }

        return points;
    }

    private Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(0, 2) * 2 - 1, Random.Range(0, 2) * 2 - 1, 0.1f);
    }

    private void Update()
    {
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
            GameObject cone = GenerateCone("Cone_" + i, pointPositions[i].z, 1f, 18, position);
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

    GameObject GenerateCone(string name, float height, float bottomRadius, int numVertices, Vector3 position)
    {
        GameObject coneObject = new GameObject(name);

        // Add a MeshFilter component to the GameObject
        MeshFilter meshFilter = coneObject.AddComponent<MeshFilter>();

        // Add a MeshRenderer component to the GameObject
        MeshRenderer meshRenderer = coneObject.AddComponent<MeshRenderer>();

        // Add a MeshRenderer component to the GameObject
       // LineRenderer lineRenderer = coneObject.AddComponent<LineRenderer>();

        // Call the CreateConeMesh method to generate the cone mesh
        Mesh coneMesh = ConeMesh.CreateConeMesh(height, bottomRadius, numVertices);

        // Assign the generated mesh to the MeshFilter
        meshFilter.mesh = coneMesh;

        // Optionally, assign a material to the MeshRenderer
        // For this example, we'll use a default material
        meshRenderer.material = new Material(Shader.Find("Unlit/Color"));
        meshRenderer.material.color = new Color(Random.value, Random.value, Random.value);

        // Set the position of the cone
        coneObject.transform.position = position;

       // ConeMesh.RenderLine(coneObject, lineRenderer, Color.black);

        // Create a sphere GameObject and position it at the apex of the cone
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        // Set the sphere size and position
        float sphereSize = 0.03f; // Adjust the size of the sphere as needed
        sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
        sphere.transform.position = new Vector3(position.x, height, position.z); // Position at the cone's apex

        // Optionally, parent the sphere to the cone for better scene organization

        // Create or assign a material to the sphere
        // Create or assign a material to the sphere
        Material sphereMaterial = new Material(Shader.Find("Unlit/Color"));
        sphereMaterial.color = Color.black; // Change the color to whatever you prefer

        // Assign the material to the sphere's renderer
        sphere.GetComponent<Renderer>().material = sphereMaterial;

        sphere.transform.parent = coneObject.transform;

        return coneObject;
    }

    private void UpdatePointPositions()
    {
        for (int i = 0; i < pointPositions.Length; i++)
        {
            // Move point
            pointPositions[i] += directions[i] * speed * Time.deltaTime;

            // Check for boundaries and reflect direction if needed
            if (pointPositions[i].x <= boundary.xMin || pointPositions[i].x >= boundary.xMax)
            {
                directions[i] = new Vector2(-directions[i].x, directions[i].y);
            }
            if (pointPositions[i].y <= boundary.yMin || pointPositions[i].y >= boundary.yMax)
            {
                directions[i] = new Vector2(directions[i].x, -directions[i].y);
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
