using UnityEngine;

public class UnitSelectionManager : MonoSingleton<UnitSelectionManager>
{
    private HexTile selectedTile;
    public HexTile SelectedTile => selectedTile;
    private int selectedUnitIndex;
    private Unit selectedUnit;
    public Unit SelectedUnit => selectedUnit;

    public void SelectUnitFromTile(HexTile tile)
    {
        if (tile == null || !tile.IsOccupied)
        {
            return;
        }

        Unit unit = tile.Units[0];

        if (unit.Owner != TurnManager.Instance.CurrentPlayer || unit.HasMovedThisTurn)
        {
            return;
        }

        // Deselect previous unit
        if (selectedUnit != null)
        {
            selectedUnit.DeSelect();
        }

        selectedTile = tile;
        selectedUnit = unit;

        selectedUnit.Select();

        // Persistent selected highlight
        if (!selectedUnit.HasMovedThisTurn)
        {
            selectedTile.SetSelectedHighlight(true);
        }
    }

    public void ClearSelection()
    {
        if (selectedUnit != null)
        {
            selectedUnit.DeSelect();
        }

        selectedUnit = null;
        selectedTile = null;
    }

    public void SelectPassenger(int index)
    {
        if (index < 0 || index >= selectedUnit.Passengers.Count)
        {
            return;
        }

        Unit passenger = selectedUnit.Passengers[index];
    }
}
