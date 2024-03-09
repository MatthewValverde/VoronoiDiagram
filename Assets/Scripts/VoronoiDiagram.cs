using UnityEngine;
using UnityEngine.UI;

public class VoronoiDiagram : MonoBehaviour
{
    private Color[] possibleColors;
    [SerializeField] private int gridSize = 10;
    private int imgSize;
    private int pixelsPerCell;
    private RawImage image;
    private Vector2Int[,] pointPositions;
    private Color[,] colors;

    private void Awake()
    {
        image = GetComponent<RawImage>();
        print(image);
        imgSize = Mathf.RoundToInt(image.GetComponent<RectTransform>().sizeDelta.x);
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
        Texture2D texture = new Texture2D(imgSize, imgSize);
        texture.filterMode = FilterMode.Point;
        pixelsPerCell = imgSize / gridSize;

        possibleColors = new Color[]
        { new Color(Random.Range(0, 1f),Random.Range(0, 1f), Random.Range(0, 1f)),
          new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f)),
          new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f)),
          new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f))
        };

        GeneratePoints();

        for (int i = 0; i < imgSize; i++)
        {
            for (int j = 0; j < imgSize; j++)
            {

                int gridX = i / pixelsPerCell;
                int gridY = j / pixelsPerCell;

                float nearestDistance = Mathf.Infinity;
                Vector2Int nearestPoint = new();


                for (int a = -1; a < 2; a++)
                {
                    for (int b = -1; b < 2; b++)
                    {

                        int X = gridX + a;
                        int Y = gridY + b;

                        if (X < 0 || Y < 0 || X >= gridSize || Y >= gridSize) continue;

                        float distance = Vector2Int.Distance(new Vector2Int(i, j), pointPositions[X, Y]);
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestPoint = new Vector2Int(X, Y);
                        }

                    }
                }

                texture.SetPixel(i, j, colors[nearestPoint.x, nearestPoint.y]);
            }
        }

        texture.Apply();
        image.texture = texture;
    }

    private void GeneratePoints()
    {
        pointPositions = new Vector2Int[gridSize, gridSize];
        colors = new Color[gridSize, gridSize];

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                pointPositions[i, j] = new Vector2Int(i * pixelsPerCell + Random.Range(0, pixelsPerCell), j * pixelsPerCell + Random.Range(0, pixelsPerCell));
                colors[i, j] = possibleColors[Random.Range(0, possibleColors.Length)];

            }
        }
    }
}
