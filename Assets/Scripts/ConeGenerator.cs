using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ConeGenerator : MonoBehaviour
{
    private Mesh coneMeshMain;

    public Material lineMaterial;

    public float lineWidth = 0.1f;
    public Color lineColor = Color.black;
    void Start()
    {
        // Create a new GameObject to hold the mesh
        GameObject coneObject = new GameObject("ProceduralCone");

        // Add a MeshFilter component to the GameObject
        MeshFilter meshFilter = coneObject.AddComponent<MeshFilter>();

        // Add a MeshRenderer component to the GameObject
        MeshRenderer meshRenderer = coneObject.AddComponent<MeshRenderer>();

        // Call the CreateConeMesh method to generate the cone mesh
        coneMeshMain = ConeMesh.CreateConeMesh(2.0f, 1.0f, 10);

        // Assign the generated mesh to the MeshFilter
        meshFilter.mesh = coneMeshMain;

        // Optionally, assign a material to the MeshRenderer
        // For this example, we'll use a default material
        meshRenderer.material = new Material(Shader.Find("Standard"));
        meshRenderer.material.color = new Color(Random.value, Random.value, Random.value);

        RenderLine(coneObject);
    }

    private void RenderLine(GameObject coneObject)
    {
        if (coneObject == null) return;

        Mesh coneMesh = coneObject.GetComponent<MeshFilter>().mesh;
        if (coneMesh == null) return;

        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = false;

        // Assuming the cone apex is the last vertex and the base starts at 0
        Vector3 apex = coneMesh.vertices[coneMesh.vertexCount - 1];
        int baseVertexCount = coneMesh.vertexCount - 1; // Excluding the apex

        lineRenderer.positionCount = baseVertexCount * 2 + 1;
        for (int i = 0; i < baseVertexCount; i++)
        {
            lineRenderer.SetPosition(i * 2, coneMesh.vertices[i]); // Base vertex
            lineRenderer.SetPosition(i * 2 + 1, apex); // Connect to apex

            if (i == baseVertexCount - 1) // Closing the base loop
            {
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, coneMesh.vertices[0]);
            }
            else // Base to base
            {
                lineRenderer.SetPosition(i * 2 + 2, coneMesh.vertices[i + 1]);
            }
        }

        // Set the material and colors
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }

    /*void OnRenderObject()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        if (mesh == null)
        {
            Debug.LogWarning("WireframeRenderer: Mesh is null.");
            return;
        }
        if (lineMaterial == null)
        {
            Debug.LogWarning("WireframeRenderer: Line Material is not set.");
            return;
        }

        lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(Color.black); // Consider making this a public variable for easy adjustment

        for (int i = 0; i < mesh.vertices.Length; i += 3)
        {
            if (i + 2 >= mesh.vertices.Length) break; // Safety check

            GL.Vertex(mesh.vertices[i]);
            GL.Vertex(mesh.vertices[i + 1]);

            GL.Vertex(mesh.vertices[i + 1]);
            GL.Vertex(mesh.vertices[i + 2]);

            GL.Vertex(mesh.vertices[i + 2]);
            GL.Vertex(mesh.vertices[i]);
        }

        GL.End();
        GL.PopMatrix();

        Debug.Log("WireframeRenderer: GL Lines should be rendered.");
    }*/
}
