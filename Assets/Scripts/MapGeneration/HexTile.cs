using System.Collections.Generic;
using UnityEngine;

public enum TerrainType
{
    Ocean,
    Plains,
    Forest,
    Hills,
    Mountain,
    None
}

public class HexTile : MonoBehaviour
{
    [SerializeField] private GameObject highlight;

    [SerializeField] private GameObject tileModel;
    [SerializeField] private Material grassMaterial;

    [Header("Tile Offsets")]
    [SerializeField] private float oceanTileHeight = -1f;
    [SerializeField] private float landTileHeight = 0f;

    [Header("Resource Assignment")]
    [SerializeField] private int resourceQuality;
    public int ResourceQuality => resourceQuality;

    public int column;
    public int row;

    private TerrainType terrain;
    public TerrainType Terrain => terrain;

    public Vector2Int coordinate;

    private Renderer tileRenderer;
    private Material[] tileMaterials;

    private Renderer highlightRenderer;

    private List<Unit> units = new List<Unit>();
    public IReadOnlyList<Unit> Units => units;

    public bool IsOccupied => units.Count > 0;

    private bool isSelected;
    public bool IsSelected => isSelected;

    private void Awake()
    {
        tileRenderer = GetComponent<Renderer>();

        if (tileRenderer != null)
        {
            tileMaterials = tileRenderer.materials;
        }

        highlightRenderer = highlight.GetComponent<Renderer>();

        SetHoverHighlight(false);
    }

    public void SetTerrain(TerrainType newTerrain)
    {
        if (terrain == newTerrain)
        {
            return;
        }

        terrain = newTerrain;
        AssignTerrainCharacteristics();
        UpdateHeight();
    }

    private void AssignTerrainCharacteristics()
    {
        switch (terrain)
        {
            case TerrainType.Ocean:
                tileMaterials[1].color = Color.cornflowerBlue;
                AssignResource();
                break;
            case TerrainType.Plains:
                tileMaterials[1] = grassMaterial;
                AssignResource();
                break;
            case TerrainType.Forest:
                tileMaterials[1].color = Color.darkGreen;
                AssignResource();
                break;
            case TerrainType.Hills:
                tileMaterials[1].color = Color.sandyBrown;
                AssignResource();
                break;
            case TerrainType.Mountain:
                tileMaterials[1].color = Color.grey;
                AssignResource();
                break;
        }
    }

    private void AssignResource()
    {
        resourceQuality = Random.Range(1, 4);
    }

    private void UpdateHeight()
    {
        transform.localPosition = terrain == TerrainType.Ocean ? new Vector3(transform.localPosition.y, oceanTileHeight, transform.localPosition.z) :
            new Vector3(transform.localPosition.y, landTileHeight, transform.localPosition.z);
    }

    public void SetHoverHighlight(bool highlighted)
    {
        highlight.SetActive(highlighted);

        if (highlighted)
        {
            SetHighlightColour(Color.white);
        }    

    }

    public void SetSelectedHighlight(bool highlighted)
    {
        highlight.SetActive(highlighted);
        isSelected = highlighted;

        if (highlighted)
        {
            SetHighlightColour(Color.yellow);
        }
    }

    public void SetMoveHighlight(bool highlighted, bool validMove = false)
    {
        highlight.SetActive(highlighted);

        if (highlighted)
        {
            SetHighlightColour(validMove ? Color.green : Color.red);
        }
    }

    private void SetHighlightColour(Color highlightColour)
    {
        highlightRenderer.material.color = highlightColour;
    }

    public bool IsValidStartingTile()
    {
        return terrain == TerrainType.Plains || terrain == TerrainType.Forest || terrain == TerrainType.Hills;
    }

    public void AddUnit(Unit unit)
    {
        if (!units.Contains(unit))
        {
            units.Add(unit);
        }
    }

    public void RemoveUnit (Unit unit)
    {
        units.Remove(unit);
    }

    public Unit GetTransportUnit(Player player)
    {
        foreach (Unit unit in Units)
        {
            if (unit.Owner == player && unit.UnitDef.CanTransport)
            {
                return unit;
            }
        }

        return null;
    }
    //private void OnMouseDown()
    //{
    //    int nextTerrain = ((int)terrain + 1) % System.Enum.GetValues(typeof(TerrainType)).Length;
    //
    //    SetTerrain((TerrainType) nextTerrain);
    //
    //    Debug.Log($"Hex [{column}, {row}] = {terrain}");
    //}
}
