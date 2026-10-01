using UnityEngine;

public class GridManager : MonoSingleton<GridManager>
{
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private int width = 100;
    [SerializeField] private int height = 100;

    private bool[,] occupied;

    private void Awake()
    {
        occupied = new bool[width, height];
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / cellSize);
        int y = Mathf.FloorToInt(worldPosition.y / cellSize);

        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(
            (gridPosition.x + 0.5f) * cellSize,
            0f,
            (gridPosition.y + 0.5f) * cellSize
            );
    }

    public bool IsOccupied(Vector2Int position)
    {
        if (!IsInsideGrid(position))
        {
            return true;
        }

        return occupied[position.x, position.y];
    }

    public bool IsInsideGrid(Vector2Int position)
    {
        return position.x >= 0 &&
            position.x < width &&
            position.y >= 0 &&
            position.y < height;
    }

    public void SetOccupied(Vector2Int position, bool value)
    {
        if (!IsInsideGrid(position))
        {
            return;
        }

        occupied[position.x, position.y] = value;
    }

    public bool CanPlace(Vector2Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int cell = origin + new Vector2Int(x, y);

                if (!IsInsideGrid(cell) || IsOccupied(cell))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void Place(Vector2Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int cell = origin + new Vector2Int(x, y);
                SetOccupied(cell, true);
            }
        }
    }
}
