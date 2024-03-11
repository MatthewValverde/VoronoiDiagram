using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ConeWireframe : MonoBehaviour
{
    public GameObject coneObject; // Assign your cone object here
    public float lineWidth = 0.1f;
    public Color lineColor = Color.black;

    private void Start()
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
}
