using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MIConvexHull;

public class VoronoiDiagramHull : MonoBehaviour
{
    [SerializeField]
    private Material cellMaterial; // Assign a default material in the inspector

    [SerializeField]
    private Material seedMaterial; // Assign another material for the seeds in the inspector

    [SerializeField]
    private float seedScale = 0.1f; // The scale for the seed visual representation

    void Start()
    {
        // Generate random points and store them
        List<VoronoiVertex> seeds = GenerateRandomPoints(10, new Rect(0, 0, 10, 10)).ToList();

        // Compute Voronoi diagram from the points
        var voronoiMesh = VoronoiMesh.Create(seeds);

        // Create Unity GameObjects from the Voronoi diagram
        foreach (var cell in voronoiMesh.Vertices)
        {
            var cellVertices = cell.Vertices.Select(v => new Vector3((float)v.Position[0], (float)v.Position[1])).ToList();
            CreateVoronoiCell(cellVertices);
        }

        // Create visual representation for the seeds
        foreach (var seed in seeds)
        {
            CreateSeedVisual(seed);
        }
    }
    void CreateSeedVisual(VoronoiVertex seed)
    {
        // Create a small sphere GameObject to represent the seed
        var seedObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        // Set the position, scale, and material of the sphere
        seedObject.transform.position = new Vector3((float)seed.Position[0], (float)seed.Position[1], 0f);
        seedObject.transform.localScale = new Vector3(seedScale, seedScale, seedScale);
        seedObject.GetComponent<Renderer>().material = seedMaterial;
    }

    public void DuplicateAndRecolorMaterial(Material originalMaterial, int duplicatesCount)
    {
        for (int i = 0; i < duplicatesCount; i++)
        {
            Material newMaterial = new Material(originalMaterial);
            newMaterial.color = new Color(Random.value, Random.value, Random.value);
            // Use newMaterial here, for example, assign it to a renderer
        }
    }

    public Material DuplicateAndRecolorMaterial(Material originalMaterial)
    {
        Material newMaterial = new Material(originalMaterial);
        newMaterial.color = new Color(Random.value, Random.value, Random.value);
        return newMaterial;
    }

    VoronoiVertex[] GenerateRandomPoints(int count, Rect bounds)
    {
        VoronoiVertex[] vertices = new VoronoiVertex[count];
        for (int i = 0; i < count; i++)
        {
            float x = UnityEngine.Random.Range(bounds.xMin, bounds.xMax);
            float y = UnityEngine.Random.Range(bounds.yMin, bounds.yMax);
            vertices[i] = new VoronoiVertex(x, y);
        }
        return vertices;
    }

    void CreateVoronoiCell(List<Vector3> cellVertices)
    {
        // Triangulate the cell's vertices to create triangles
        List<int> triangles = TriangulateConvexPolygon(cellVertices);

        // Create Unity mesh
        Mesh mesh = new Mesh();
        mesh.vertices = cellVertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        // Create mesh GameObject
        GameObject meshObject = new GameObject("VoronoiCell");
        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        meshRenderer.material = DuplicateAndRecolorMaterial(cellMaterial);
    }

    List<int> TriangulateConvexPolygon(List<Vector3> cellVertices)
    {
        List<int> indices = new List<int>();
        for (int i = 1; i < cellVertices.Count - 1; i++)
        {
            indices.Add(0);
            indices.Add(i);
            indices.Add(i + 1);
        }
        return indices;
    }

    // Voronoi vertex class
    public class VoronoiVertex : IVertex
    {
        public double[] Position { get; set; }

        public VoronoiVertex(double x, double y)
        {
            Position = new double[] { x, y };
        }
    }
}

