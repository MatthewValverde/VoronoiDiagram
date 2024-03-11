using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WireframeRenderer : MonoBehaviour
{
    public Material lineMaterial;

    private void OnRenderObject()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        if (mesh == null || lineMaterial == null)
        {
            return;
        }

        lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(Color.black);

        for (int i = 0; i < mesh.vertices.Length; i += 3)
        {
            // Draw lines between each set of three vertices (assuming triangles)
            GL.Vertex(mesh.vertices[i]);
            GL.Vertex(mesh.vertices[i + 1]);

            GL.Vertex(mesh.vertices[i + 1]);
            GL.Vertex(mesh.vertices[i + 2]);

            GL.Vertex(mesh.vertices[i + 2]);
            GL.Vertex(mesh.vertices[i]);
        }

        GL.End();
        GL.PopMatrix();
    }
}

