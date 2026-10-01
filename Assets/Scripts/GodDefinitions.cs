using UnityEngine;

[CreateAssetMenu(fileName = "GodDefinition", menuName = "ScriptableObjects/GodDefinition")]
public class GodDefinitions : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string godName;
    public string GodName => godName;

    [SerializeField] private GameObject prefab;
    public GameObject Prefab => prefab;

    [Header("Spawn Rules")]
    [SerializeField] private TerrainType[] suitableTerrain;
    public TerrainType[] SuitableTerrain => suitableTerrain;

    [SerializeField] private TerrainType[] requiredAdjacentTerrain;
    public TerrainType[] RequiredAdjacentTerrain => requiredAdjacentTerrain;
}
