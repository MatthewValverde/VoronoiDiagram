using UnityEngine;

public class ConeMesh : MonoBehaviour
{
    public static Mesh CreateConeMesh(float height, float bottomRadius, int numVertices)
    {
        Mesh mesh = new Mesh();
        // After defining vertices and triangles


        Vector3[] vertices = new Vector3[numVertices + 2];
        Vector3[] normals = new Vector3[numVertices + 2];
        int[] triangles = new int[(numVertices * 2) * 3];

        // Calculate normals for the top vertex and its adjacent vertices
        Vector3 topNormal = Vector3.up; // Assuming the top vertex is straight up
        normals[normals.Length - 1] = topNormal;

        // Create bottom circle vertices and top vertex
        vertices[0] = Vector3.zero; // Bottom center vertex
        normals[0] = -Vector3.up; // Normal pointing downwards for the bottom center vertex

        float angleStep = 360.0f / numVertices;
        for (int i = 1; i <= numVertices; i++)
        {
            float angle = Mathf.Deg2Rad * angleStep * i;
            vertices[i] = new Vector3(Mathf.Cos(angle) * bottomRadius, 0, Mathf.Sin(angle) * bottomRadius);
            normals[i] = Vector3.up; // Assuming flat shading for the bottom, normals point up
        }
        vertices[numVertices + 1] = new Vector3(0, height, 0); // Top vertex
        normals[numVertices + 1] = Vector3.up; // This might not be visually correct for the sides but works for the top

        // Adjust the normals for the sides of the cone
        Vector3 sideNormal = Vector3.Cross(vertices[1] - vertices[numVertices + 1], vertices[2] - vertices[numVertices + 1]).normalized;
        for (int i = 1; i <= numVertices; i++)
        {
            normals[i] = sideNormal; // Adjust normals to point outwards for the sides
        }

        // Adjusted bottom circle triangles
        for (int i = 0; i < numVertices - 1; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 2] = i + 1; // Swapped
            triangles[i * 3 + 1] = i + 2; // Swapped
        }

        // Adjusted last triangle for the bottom circle
        triangles[(numVertices - 1) * 3] = 0;
        triangles[(numVertices - 1) * 3 + 2] = numVertices; // Swapped
        triangles[(numVertices - 1) * 3 + 1] = 1; // Swapped

        // Adjusted side triangles
        for (int i = 0; i < numVertices - 1; i++)
        {
            triangles[(numVertices + i) * 3] = i + 1;
            triangles[(numVertices + i) * 3 + 2] = i + 2; // Swapped
            triangles[(numVertices + i) * 3 + 1] = numVertices + 1; // Swapped
        }

        // Adjusted last side triangle
        triangles[(numVertices * 2 - 1) * 3] = numVertices;
        triangles[(numVertices * 2 - 1) * 3 + 2] = 1; // Swapped
        triangles[(numVertices * 2 - 1) * 3 + 1] = numVertices + 1; // Swapped


        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;

        return mesh;
    }

    public static void RenderLine(GameObject coneObject, LineRenderer lineRenderer, Color lineColor, float lineWidth = 0.01f)
    {
        if (coneObject == null) return;

        Mesh coneMesh = coneObject.GetComponent<MeshFilter>().mesh;
        if (coneMesh == null) return;

        //LineRenderer lineRenderer = GetComponent<LineRenderer>();
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
        lineRenderer.material.color = lineColor;
    }

}
