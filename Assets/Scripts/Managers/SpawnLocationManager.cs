using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnLocationManager : MonoSingleton<SpawnLocationManager>
{
    [Header("GameObject Prefabs")]
    [SerializeField] private GameObject villageCentrePrefab;
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private GameObject mountainPrefab;
    [SerializeField] private GameObject hillPrefab;
    [Header("Model Reference")]
    [SerializeField] private GameObject oikosModel;
    [Header("God Definitions")]
    [SerializeField] private GodDefinitions[] godsAndGoddesses;
    [Header("Minimum distance between spawn locations")]
    [SerializeField] private int minimumSpawnDistance = 5;

    private Renderer renderer;
    private Material[] materials;

    private List<HexTile> tiles = new List<HexTile>();
    private List<HexTile> selectedTiles = new List<HexTile>();
    private List<HexTile> occupiedTiles = new List<HexTile>();

    public void InitiateSpawnLocations()
    {
        FindTiles();
        AssignStartingLocations();
        AssignGodsToTiles();
        AssignTerrainToTiles();
    }

    private void FindTiles()
    {
        tiles = HexGridManager.Instance.GetAllTiles();
    }
    
    private void AssignStartingLocations()
    {
        List<HexTile> validTiles = GetValidStartingTiles();

        // Ensure there are enough valid tiles for all players
        if (validTiles.Count < GameManager.Instance.Players.Count)
        {
            Debug.LogError("Not enough valid tiles to assign starting locations!");
            return;
        }

        foreach (Player player in GameManager.Instance.Players)
        {
            if (validTiles.Count == 0)
            {
                return;
            }

            int randomIndex = Random.Range(0, validTiles.Count);

            HexTile chosenTile = validTiles[randomIndex];

            AssignPlayerToTile(player, chosenTile);

            selectedTiles.Add(chosenTile);

            RemoveTilesWithinRange(chosenTile, validTiles);
        }
    }

    private List<HexTile> GetValidStartingTiles()
    {
        List<HexTile> validTiles = new List<HexTile>();

        foreach (HexTile tile in tiles)
        {
            if (tile.IsValidStartingTile())
            {
                validTiles.Add(tile);
            }
        }

        return validTiles;
    }

    private void AssignPlayerToTile(Player player, HexTile tile)
    {
        Debug.Log($"Player {player.playerID} starts on {tile.name}");

        if (player == null)
        {
            Debug.LogError($"Could not find Player {player.playerID}");
        }

        if (tile == null)
        {
            Debug.LogError($"{player.playerID} was assigned a null starting tile");
        }

        if (player != null && tile != null)
        {
            occupiedTiles.Add(tile);

            player.startingTile = tile;

            occupiedTiles.AddRange(HexGridManager.Instance.GetNeighbours(tile));

            GameObject starterVillage = Instantiate(villageCentrePrefab, tile.transform.position, Quaternion.identity);

            starterVillage.transform.SetParent(tile.transform);

            starterVillage.transform.localScale = new Vector3(0.2f, 0.1f, 0.2f);

            starterVillage.name = $"{player.playerName} Starting Location";

            AssignPlayerColoursToPrefabs(player, starterVillage);
        }
    }

    public bool TileSurroundedByOcean(HexTile tile, out List<HexTile> neighbours)
    {
        neighbours = new List<HexTile>();

        neighbours.AddRange(HexGridManager.Instance.GetNeighbours(tile));

        foreach (HexTile neighbour in neighbours)
        {
            if (neighbour.Terrain != TerrainType.Ocean)
            {
                return false;
            }
        }

        return true;
    }

    private void RemoveTilesWithinRange(HexTile chosenTile, List<HexTile> validTiles)
    {
        for (int i = validTiles.Count - 1; i >= 0; i--)
        {
            if (HexGridManager.Instance.GetHexDistance(chosenTile, validTiles[i]) < minimumSpawnDistance)
            {
                validTiles.RemoveAt(i);
            }
        }
    }

    public void AssignGodsToTiles()
    {
        foreach (GodDefinitions god in godsAndGoddesses)
        {
            HexTile spawnTile = GetValidGodTile(god);

            if (spawnTile == null)
            {
                Debug.LogWarning($"Could not find a valid spawn location for {god.GodName}.");

                continue;
            }

            SpawnGod(god, spawnTile);
        }
    }

    private void SpawnGod(GodDefinitions god, HexTile tile)
    {
        if (god.Prefab != null && tile != null)
        {
            GameObject godObject = Instantiate(god.Prefab, tile.transform.position, Quaternion.identity, tile.transform);

            occupiedTiles.Add(tile);

            godObject.transform.localScale = new Vector3(0.5f, 0.25f, 0.5f);

            godObject.name = god.GodName;

            Debug.Log($"{god.GodName} spawned on {tile.name}");
        }
    }

    private HexTile GetValidGodTile(GodDefinitions god)
    {
        List<HexTile> validTiles = new List<HexTile>();

        foreach (HexTile tile in tiles)
        {
            if (!IsValidGodTile(tile, god) || occupiedTiles.Contains(tile))
            {
                continue;
            }

            validTiles.Add(tile);
        }

        if (validTiles.Count == 0)
        {
            return null;
        }

        return validTiles[Random.Range(0, validTiles.Count)];
    }

    private bool IsValidGodTile(HexTile tile, GodDefinitions god)
    {
        // Check terrain
        if (!HasSuitableTerrain(tile.Terrain, god.SuitableTerrain))
        {
            return false;
        }

        // Check adjacent terrain requirements
        if (!HasRequiredAdjacentTerrain(tile, god.RequiredAdjacentTerrain))
        {
            return false;
        }

        return true;
    }

    private void AssignTerrainToTiles()
    {
        float randomYAngle = Random.Range(0, 360f);
        Quaternion randomRotation = Quaternion.Euler(0f, randomYAngle, 0f);

        foreach (HexTile tile in tiles)
        {
            switch (tile.Terrain)
            {
                case TerrainType.Ocean:
                    continue;
                case TerrainType.Plains:
                    continue;
                case TerrainType.Forest:
                    if (treePrefab != null)
                    {
                        GameObject spawnedTrees = Instantiate(treePrefab, tile.transform.position, randomRotation, tile.transform);
                    }
                    continue;
                case TerrainType.Hills:
                    if (hillPrefab != null)
                    {
                        GameObject spawnedHill = Instantiate(hillPrefab, tile.transform.position, Quaternion.identity, tile.transform);

                        spawnedHill.transform.localScale = new Vector3(1.2f, 0.4f, 1.2f);
                    }
                    continue;
                case TerrainType.Mountain:
                    if (mountainPrefab != null)
                    {
                        GameObject spawnedMountain = Instantiate(mountainPrefab, tile.transform.position + new Vector3(0f, 9f, 0f), randomRotation, tile.transform);
                    }
                    continue;
            }
        }
    }

    private bool HasSuitableTerrain(TerrainType tileTerrain, TerrainType[] validTerrain)
    {
        foreach (TerrainType terrainType in validTerrain)
        {
            if (tileTerrain == terrainType)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasRequiredAdjacentTerrain(HexTile tile, TerrainType[] requiredAdjacentTerrain)
    {
        // No adjacent requirement
        if (requiredAdjacentTerrain == null || requiredAdjacentTerrain.Length == 0)
        {
            return true;
        }

        List<HexTile> neighbourTiles = HexGridManager.Instance.GetNeighbours(tile);

        foreach (HexTile neighbour in neighbourTiles)
        {
            if (HasSuitableTerrain(neighbour.Terrain, requiredAdjacentTerrain))
            {
                return true;
            }
        }

        return false;
    }

    private void AssignPlayerColoursToPrefabs(Player player, GameObject prefab)
    {
        renderer = prefab.transform.GetChild(0).GetComponent<Renderer>();

        materials = renderer.materials;

        materials[4] = GameManager.Instance.PlayerColours[player.playerID];

        renderer.materials = materials;
    }
}
