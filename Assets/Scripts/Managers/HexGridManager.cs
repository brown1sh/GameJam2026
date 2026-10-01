using System.Collections.Generic;
using UnityEngine;

public class HexGridManager : MonoSingleton<HexGridManager>
{
    [Header("Grid Size")]
    [Min(1)]
    [SerializeField] private int columns = 29;
    public int Columns => columns;

    [Min(1)]
    [SerializeField] private int rows = 26;
    public int Rows => rows;

    [Header("HexTile Settings")]
    [SerializeField] private float hexRadius = 10f;
    public float HexRadius => hexRadius;
    [SerializeField] private float hexHeight = 25f;

    [Header("Tile Prefab")]
    [SerializeField] private GameObject hexPrefab;

    [Header("Aesthetics")]
    [SerializeField] private GameObject oceanWaterPrefab;

    [Space]
    [SerializeField] private MapCameraController mapCameraController;
    [SerializeField] private Transform generatedMapParent;

    private Dictionary<Vector2Int, HexTile> tiles = new Dictionary<Vector2Int, HexTile>();

    public void StartGeneration()
    {
        GenerateGrid();
        CreateOceanPlane();
    }

    private void CreateOceanPlane()
    {
        GameObject waterPlane = Instantiate(oceanWaterPrefab, generatedMapParent);
    }

    private void GenerateGrid()
    {
        // Remove existing Tiles
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        tiles.Clear();

        float width = Mathf.Sqrt(3f) * hexRadius;
        float height = 2f * hexRadius;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                float x = column * width;

                // Offset every other row
                if (row % 2 == 1)
                {
                    x += width * 0.5f;
                }
                float z = row * (height * 0.75f);

                Vector3 position = new Vector3(x, 0f, z);

                GameObject tile = Instantiate(hexPrefab, position, Quaternion.identity, generatedMapParent);

                tile.transform.localScale = new Vector3(hexRadius, hexHeight, hexRadius);

                tile.name = $"Hex {column}_{row}";

                HexTile hexTile = tile.GetComponentInChildren<HexTile>();

                hexTile.column = column;
                hexTile.row = row;

                // Default terrain
                hexTile.SetTerrain(TerrainType.None);

                Vector2Int coordinate = new Vector2Int(column, row);

                tiles.Add(coordinate, hexTile);

                MapData savedMap = MapSaveSystem.LoadMap();

                if (savedMap != null)
                {
                    ApplySavedMap(savedMap);
                }
            }
        }

        Bounds bounds = GetGridBounds();
        mapCameraController.SetMapBounds(bounds);
    }

    private void ApplySavedMap(MapData mapData)
    {
        columns = mapData.columns;
        rows = mapData.rows;

        foreach (HexData data in mapData.tiles)
        {
            Vector2Int coordinate = new Vector2Int(data.column, data.row);

            if (tiles.TryGetValue(coordinate, out HexTile tile))
            {
                tile.SetTerrain(data.terrain);
            }
        }
    }

    public HexTile GetTile(int column, int row)
    {
        Vector2Int coordinate = new Vector2Int(column, row);

        if (tiles.TryGetValue(coordinate, out HexTile tile))
        {
            return tile;
        }

        return null;
    }

    public void Save()
    {
        MapSaveSystem.SaveMap(this);
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        Save();
    //    }
    //}

    public Bounds GetGridBounds()
    {
        Bounds bounds = new Bounds();

        bool firstTile = true;

        foreach (KeyValuePair<Vector2Int, HexTile> kvp in tiles)
        {
            Renderer renderer = kvp.Value.gameObject.GetComponent<Renderer>();

            if (renderer == null)
            {
                continue;
            }

            if (firstTile)
            {
                bounds = renderer.bounds;
                firstTile = false;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        return bounds;
    }

    public bool AreNeighbours(HexTile tileA, HexTile tileB)
    {
        // Determine whether the two tiles share an edge
        if (tileA == null || tileB == null)
        {
            return false;
        }

        if (GetNeighbours(tileA).Contains(tileB))
        {
            return true;
        }

        return false;
    }

    public List<HexTile> GetNeighbours(HexTile tile)
    {
        List<HexTile> neighbourTiles = new List<HexTile>();

        int row = tile.row;
        int column = tile.column;

        int[,] directions;

        if (row % 2 == 0)
        {
            directions = new int[,]
            {
                { 1, 0 },
                { -1, 0 },
                { 0, -1 },
                { -1, -1 },
                { 0, 1 },
                { -1, 1 }
            };
        }
        else
        {
            directions = new int[,]
            {
                { 1, 0 },
                { -1, 0 },
                { 1, -1 },
                { 0, -1 },
                { 1, 1 },
                { 0, 1 }
            };
        }

        for (int i = 0; i < 6; i++)
        {
            int neighbourColumn = column + directions[i, 0];
            int neighbourRow = row + directions[i, 1];

            HexTile neighbour = HexGridManager.Instance.GetTile(neighbourColumn, neighbourRow);

            if (neighbour != null)
            {
                neighbourTiles.Add(neighbour);
            }
        }

        return neighbourTiles;
    }
    private Vector3Int OffsetToCube(int column, int row)
    {
        int x = column - (row - (row & 1)) / 2;
        int z = row;
        int y = -x - z;

        return new Vector3Int(x, y, z);
    }

    public int GetHexDistance(HexTile a, HexTile b)
    {
        Vector3Int aCube = OffsetToCube(a.column, a.row);
        Vector3Int bCube = OffsetToCube(b.column, b.row);

        return Mathf.Max(Mathf.Abs(aCube.x - bCube.x),
            Mathf.Abs(aCube.y - bCube.y),
            Mathf.Abs(aCube.z - bCube.z));
    }

    public List <HexTile> GetAllTiles()
    {
        return new List<HexTile>(tiles.Values);
    }
}
