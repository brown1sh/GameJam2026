using UnityEngine;

[CreateAssetMenu(fileName = "UnitDefinition", menuName = "ScriptableObjects/UnitDefinition")]
public class UnitDefinitions : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string unitName;
    public string UnitName => unitName;
    [SerializeField] private GameObject prefab;
    public GameObject Prefab => prefab;

    [Header("Movement")]
    [SerializeField] private int movementRange = 1;
    public int MovementRange => movementRange;

    [Header("Transport")]
    [SerializeField] private bool canTransport;
    public bool CanTransport => canTransport;
    [SerializeField] private int transportCapacity;
    public int TransportCapacity => transportCapacity;
}
