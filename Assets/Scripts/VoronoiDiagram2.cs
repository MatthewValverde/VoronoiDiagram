using UnityEngine;
using System.Collections.Generic;

using csDelaunay;

public class VoronoiDiagram2 : MonoBehaviour
{

    // The number of polygons/sites we want
    public int polygonNumber = 200;

    // This is where we will store the resulting data
    private Dictionary<System.Numerics.Vector2, Site> sites;
    private List<Edge> edges;

    void Start()
    {
        // Create your sites (lets call that the center of your polygons)
        List<System.Numerics.Vector2> points = CreateRandomPoint();

        // Create the bounds of the voronoi diagram
        // Use Rectf instead of Rect; it's a struct just like Rect and does pretty much the same,
        // but like that it allows you to run the delaunay library outside of unity (which mean also in another tread)
        Rectf bounds = new Rectf(0, 0, 512, 512);

        // There is a two ways you can create the voronoi diagram: with or without the lloyd relaxation
        // Here I used it with 2 iterations of the lloyd relaxation
        Voronoi voronoi = new Voronoi(points, bounds, 5);

        // But you could also create it without lloyd relaxtion and call that function later if you want
        //Voronoi voronoi = new Voronoi(points,bounds);
        //voronoi.LloydRelaxation(5);

        // Now retreive the edges from it, and the new sites position if you used lloyd relaxtion
        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges;

        DisplayVoronoiDiagram();
    }

    private List<System.Numerics.Vector2> CreateRandomPoint()
    {
        // Use System.Numerics.Vector2f, instead of System.Numerics.Vector2
        // System.Numerics.Vector2f is pretty much the same than System.Numerics.Vector2, but like you could run Voronoi in another thread
        List<System.Numerics.Vector2> points = new List<System.Numerics.Vector2>();
        for (int i = 0; i < polygonNumber; i++)
        {
            points.Add(new System.Numerics.Vector2(Random.Range(0, 512), Random.Range(0, 512)));
        }

        return points;
    }

    // Here is a very simple way to display the result using a simple bresenham line algorithm
    // Just attach this script to a quad
    private void DisplayVoronoiDiagram()
    {
        Texture2D tx = new Texture2D(512, 512);
        foreach (KeyValuePair<System.Numerics.Vector2, Site> kv in sites)
        {
            tx.SetPixel((int)kv.Key.X, (int)kv.Key.Y, Color.red);
        }
        foreach (Edge edge in edges)
        {
            // if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
            if (edge.ClippedEnds == null) continue;

            DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, Color.black);
        }
        tx.Apply();

        GetComponent<Renderer>().material.mainTexture = tx;
    }

    // Bresenham line algorithm
    private void DrawLine(System.Numerics.Vector2 p0, System.Numerics.Vector2 p1, Texture2D tx, Color c, int offset = 0)
    {
        
        int x0 = (int)p0.X;
        int y0 = (int)p0.Y;
        int x1 = (int)p1.X;
        int y1 = (int)p1.Y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            tx.SetPixel(x0 + offset, y0 + offset, c);

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
}