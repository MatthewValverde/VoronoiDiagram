using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class VoronoiDiagram3D : MonoBehaviour
{
    private Color[] possibleColors;
    [SerializeField] private int gridSize = 10;
    private MeshRenderer meshRenderer;
    private Material material;
    // Assume these are defined somewhere in your script
    private Vector3[] pointPositions;
    private Color[] colors;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material; // Ensure this material uses your custom shader
    }

    private void Start()
    {
        GenerateDiagram();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateDiagram();
        }
    }

    private void GenerateDiagram()
    {
        // Setup your points and colors here as before, but consider the plane's size and position

        // Example of setting arrays to the shader

        //material.SetVectorArray("_Points", pointPositions);
        //material.SetColorArray("_Colors", colors);
    }

    private void GeneratePoints()
    {
        // Generate your points and colors here
        // Remember to adjust this method for 3D space based on your Plane's dimensions and orientation
    }
}
