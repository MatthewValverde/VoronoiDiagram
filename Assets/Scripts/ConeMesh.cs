using System.Collections;
using System.Collections.Generic;
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

        // Create bottom circle triangles
        for (int i = 0; i < numVertices - 1; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        // Last triangle for the bottom circle
        triangles[(numVertices - 1) * 3] = 0;
        triangles[(numVertices - 1) * 3 + 1] = numVertices;
        triangles[(numVertices - 1) * 3 + 2] = 1;

        // Create side triangles
        for (int i = 0; i < numVertices - 1; i++)
        {
            triangles[(numVertices + i) * 3] = i + 1;
            triangles[(numVertices + i) * 3 + 1] = i + 2;
            triangles[(numVertices + i) * 3 + 2] = numVertices + 1;
        }

        // Last side triangle
        triangles[(numVertices * 2 - 1) * 3] = numVertices;
        triangles[(numVertices * 2 - 1) * 3 + 1] = 1;
        triangles[(numVertices * 2 - 1) * 3 + 2] = numVertices + 1;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;

        return mesh;
    }

}
