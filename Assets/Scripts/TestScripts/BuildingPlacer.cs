using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject buildingPrefab;
    [SerializeField] private Material canPlaceMat;
    [SerializeField] private Material cannotPlaceMat;
    [SerializeField] private LayerMask groundLayer;

    private GameObject preview;
    private Building building;
    private Renderer renderer;

    private void Start()
    {
        preview = Instantiate(buildingPrefab);
        building = preview.GetComponent<Building>();

        renderer = preview.GetComponent<Renderer>();
    }

    private void Update()
    {
        UpdatePreview();

        if (Input.GetMouseButtonDown(0))
        {
            TryPlace();
        }
    }

    private void UpdatePreview()
    {
        Ray ray = cam.ScreenPointToRay (Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            return;
        }

        Vector3 worldMousePos = hit.point;

        Vector2Int cell = GridManager.Instance.WorldToGrid(worldMousePos);

        Vector3 snappedPosition = GridManager.Instance.GridToWorld(cell);

        preview.transform.position = snappedPosition;

        bool canPlace = GridManager.Instance.CanPlace(cell, building.Size);

        SetPreviewColour(canPlace);
    }

    private void TryPlace()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            return;
        }

        Vector2Int cell = GridManager.Instance.WorldToGrid(hit.point);

        if (!GridManager.Instance.CanPlace(cell, building.Size))
        {
            return;
        }

        Vector3 position = GridManager.Instance.GridToWorld(cell);

        GameObject buildingObject = Instantiate(buildingPrefab, position, Quaternion.identity);

        GridManager.Instance.Place(cell, building.Size);
    }

    private void SetPreviewColour(bool valid)
    {
        Material mat = valid ? canPlaceMat : cannotPlaceMat;
    }
}
