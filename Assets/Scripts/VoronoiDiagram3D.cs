using System.Collections.Generic;
using UnityEngine;
using MIConvexHull;

[RequireComponent(typeof(MeshRenderer))]
public class VoronoiDiagram3D : MonoBehaviour
{
    public bool animate = false;
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
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material; // Ensure this material uses your custom shader
    }

    private void Start()
    {
        directions = new Vector3[pointPositions.Length];
        for (int i = 0; i < pointPositions.Length; i++)
        {
            directions[i] = GetRandomDirection();
        }
        voronoiCorners = CalculateVoronoiCorners(pointPositions);

        GenerateDiagram();
    }

    private Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(0, 2) * 2 - 1, Random.Range(0, 2) * 2 - 1, 0.1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateDiagram();
        }
      if(animate) UpdatePointPositions();
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

        var materialProperty = new MaterialPropertyBlock();
        materialProperty.SetFloatArray("_Points", pointData);
        materialProperty.SetInt("_NumPoints", numPoints);
        material.SetColorArray("_Colors", colors);
        GetComponent<Renderer>().SetPropertyBlock(materialProperty);
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
