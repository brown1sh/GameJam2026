using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Unit Type")]
    [SerializeField] private UnitDefinitions unitDef;
    public UnitDefinitions UnitDef => unitDef;

    public Player Owner { get; private set; }
    public HexTile CurrentTile { get; private set; }

    private bool hasMovedThisTurn = false;
    public bool HasMovedThisTurn => hasMovedThisTurn;

    public bool IsSelected { get; private set; }

    private Material outlineMat;
    private int materialIndex = 1;

    private readonly List<Unit> passengers = new List<Unit>();
    public IReadOnlyList<Unit> Passengers => passengers;

    private Unit transport;
    public Unit Transport => transport;

    public bool IsTransported => transport != null;

    public bool HasPassengers => passengers.Count > 0;

    private void Awake()
    {
        outlineMat = gameObject.GetComponent<Renderer>().materials[materialIndex];
    }

    public void InitialiseUnit(UnitDefinitions unit, Player owner, HexTile startingTile)
    {
        this.unitDef = unit;
        Owner = owner;
        CurrentTile = startingTile;

        startingTile.AddUnit(this);

        transform.position = startingTile.transform.position;
    }

    public void StartTurn()
    {
        hasMovedThisTurn = false;
    }

    public void Select()
    {
        IsSelected = true;
        outlineMat.SetFloat("_ShowOutline", 1);
    }

    public void DeSelect()
    {
        IsSelected = false;
        outlineMat.SetFloat("_ShowOutline", 0);
    }

    public bool CanMoveTo(HexTile targetTile)
    {
        if (hasMovedThisTurn)
        {
            return false;
        }

        if (targetTile == null)
        {
            return false;
        }

        if (TurnManager.Instance.CurrentPlayer != Owner)
        {
            return false;
        }

        if (IsTransported)
        {
            return false;
        }

        int distance = HexGridManager.Instance.GetHexDistance(CurrentTile, targetTile);

        if (HexGridManager.Instance.AreNeighbours(CurrentTile, targetTile))
        {
            Unit boat = targetTile.GetTransportUnit(Owner);

            if (boat != null)
            {
                return distance <= unitDef.MovementRange;
            }
        }

        if (!unitDef.CanTransport && targetTile.Terrain == TerrainType.Ocean || targetTile.Terrain == TerrainType.Mountain)
        {
            return false;
        }

        if (unitDef.CanTransport && targetTile.Terrain != TerrainType.Ocean)
        {
            return false;
        }

        return distance <= unitDef.MovementRange;
    }

    public bool MoveTo(HexTile targetTile)
    {
        if (!CanMoveTo(targetTile))
        {
            return false;
        }

        CurrentTile.RemoveUnit(this);
        targetTile.AddUnit(this);

        CurrentTile = targetTile;

        StartCoroutine(MoveToPosition(targetTile.transform.position));

        foreach (Unit passenger in passengers)
        {
            passenger.SetCurrentTile(targetTile);
        }

        hasMovedThisTurn = true;

        return true;
    }

    public bool CanBoardBoat(Unit boat)
    {
        if (boat == null)
        {
            return false;
        }

        if (IsTransported)
        {
            return false;
        }

        if (!boat.unitDef.CanTransport)
        {
            return false;
        }

        if (!boat.CanLoadUnit(this))
        {
            return false;
        }

        return HexGridManager.Instance.AreNeighbours(CurrentTile, boat.CurrentTile);
    }

    public bool BoardBoat(Unit boat)
    {
        if (!CanBoardBoat(boat))
        {
            return false;
        }

        // Remove unit from land tile
        CurrentTile.RemoveUnit(this);

        //Add unit to boat
        boat.LoadUnit(this);

        return true;
    }

    public bool CanLoadUnit(Unit unit)
    {
        if (!unitDef.CanTransport)
        {
            return false;
        }

        if (passengers.Count >= unitDef.TransportCapacity)
        {
            return false;
        }

        return unit != this;
    }

    public bool LoadUnit(Unit unit)
    {
        if (!CanLoadUnit(unit))
        {
            return false;
        }

        passengers.Add(unit);

        unit.SetCurrentTile(CurrentTile);
        unit.SetTransport(this);

        return true;
    }

    public bool CanUnloadUnit(Unit unit, HexTile targetTile)
    {
        if (!passengers.Contains(unit))
        {
            return false;
        }

        if (targetTile == null)
        {
            return false;
        }

        if (!HexGridManager.Instance.AreNeighbours(CurrentTile, targetTile))
        {
            return false;
        }

        return targetTile.Terrain != TerrainType.Ocean || targetTile.Terrain != TerrainType.Mountain;
    }

    public bool UnloadUnit(Unit unit, HexTile targetTile)
    {
        if (!passengers.Remove(unit))
        {
            return false;
        }

        passengers.Remove(unit);

        unit.SetTransport(null);

        unit.SetCurrentTile(targetTile);

        targetTile.AddUnit(unit);

        unit.gameObject.SetActive(true);
        unit.transform.position = targetTile.transform.position;

        return true;
    }

    public void SetTransport(Unit transport)
    {
        this.transport = transport;

        if (transport != null)
        {
            // Hide or disable normal movement representation
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }    
    }

    internal void SetCurrentTile(HexTile tile)
    {
        CurrentTile = tile;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;

        float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        transform.position = targetPosition;
    }
}
