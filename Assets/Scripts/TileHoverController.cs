using UnityEngine;

public class TileHoverController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask tileLayer;

    private HexTile hoveredTile;
    private HexTile selectedTile;
    private Unit selectedUnit;
    private Player currentPlayer;

    private void Update()
    {
        HandleMouseInput();

        if (UnitSelectionManager.Instance.SelectedUnit != null)
        {
            MoveTileHoverSelection();
        }
        else
        {
            TileHoverHighlight();
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }

        if (Input.GetMouseButtonDown(1))
        {
            HandleRightClick();
        }
    }

    private void HandleLeftClick()
    {
        HexTile tile = GetTileUnderMouse();

        if (tile == null || !tile.IsOccupied)
        {
            return;
        }

        UnitSelectionManager.Instance.SelectUnitFromTile(tile);

        if (UnitSelectionManager.Instance.SelectedUnit != null)
        {
            Unit unit = UnitSelectionManager.Instance.SelectedUnit;

            currentPlayer = unit.Owner;
        }

        selectedTile = UnitSelectionManager.Instance.SelectedTile;
    }

    private void HandleRightClick()
    {
        selectedUnit = UnitSelectionManager.Instance.SelectedUnit;

        if (selectedUnit == null)
        {
            return;
        }

        if (hoveredTile == null)
        {
            return;
        }

        if (hoveredTile == selectedUnit.CurrentTile)
        {
            hoveredTile.SetSelectedHighlight(false);
            hoveredTile.SetMoveHighlight(false);
            hoveredTile.SetHoverHighlight(true);

            UnitSelectionManager.Instance.ClearSelection();
            selectedTile = null;

            hoveredTile = null;
        }

        HexTile oldTile = selectedUnit.CurrentTile;

        if (selectedUnit != null)
        {
            Unit boat = hoveredTile.GetTransportUnit(selectedUnit.Owner);

            // Boarding boat
            if (boat != null && selectedUnit.CanBoardBoat(boat))
            {
                selectedUnit.BoardBoat(boat);
                oldTile.SetSelectedHighlight(false);
                hoveredTile.SetMoveHighlight(false);
                hoveredTile.SetHoverHighlight(true);
                UnitSelectionManager.Instance.ClearSelection();

                hoveredTile = null;
                selectedTile = null;
            }
        }

        if (!selectedUnit.CanMoveTo(hoveredTile))
        {
            return;
        }

        if (selectedUnit.MoveTo(hoveredTile))
        {
            oldTile.SetSelectedHighlight(false);
            hoveredTile.SetMoveHighlight(false);
            hoveredTile.SetHoverHighlight(true);

            UnitSelectionManager.Instance.ClearSelection();
            selectedTile = null;

            hoveredTile = null;
        }
    }

    private void TileHoverHighlight()
    {
        HexTile tile = GetTileUnderMouse();

        if (selectedTile == tile)
        {
            return;
        }

        // Remove hover from previous tile
        if (hoveredTile != null && hoveredTile != selectedTile)
        {
            hoveredTile.SetHoverHighlight(false);
        }

        hoveredTile = tile;

        if (hoveredTile == null)
        {
            return;
        }

        // Don't overwrite the selected highlight
        if (hoveredTile.IsSelected)
        {
            return;
        }

        hoveredTile.SetHoverHighlight(true);
    }

    private void MoveTileHoverSelection()
    {
        HexTile tile = GetTileUnderMouse();


        if (currentPlayer != TurnManager.Instance.CurrentPlayer)
        {
            selectedTile.SetSelectedHighlight(false);
            hoveredTile.SetMoveHighlight(false);
            hoveredTile.SetHoverHighlight(true);

            UnitSelectionManager.Instance.ClearSelection();
            selectedTile = null;

            hoveredTile = null;

            currentPlayer = TurnManager.Instance.CurrentPlayer;
        }

        // Mouse isn't over a tile
        if (tile == null)
        {
            if (hoveredTile != null)
            {
                hoveredTile.SetMoveHighlight(false);
            }

            hoveredTile = null;
            return;
        }

        // Mouse is still over the same tile
        if (tile == hoveredTile)
        {
            UpdateMoveHighlight();
            return;
        }

        // Remove movement highlight from old tile
        if (hoveredTile != null && hoveredTile != selectedTile)
        {
            hoveredTile.SetMoveHighlight(false);
        }

        hoveredTile = tile;

        if (hoveredTile != selectedTile)
        {
            hoveredTile.SetMoveHighlight(true);

            UpdateMoveHighlight();
        }
    }

    public void UpdateMoveHighlight()
    {
        selectedUnit = UnitSelectionManager.Instance.SelectedUnit;

        if (selectedUnit == null || hoveredTile == null || hoveredTile.IsSelected)
        {
            return;
        }

        bool validMove = selectedUnit.CanMoveTo(hoveredTile);

        hoveredTile.SetMoveHighlight(true, validMove);
    }

    private HexTile GetTileUnderMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, tileLayer))
        {
            return null;
        }

        return hit.collider.GetComponent<HexTile>();
    }
}
