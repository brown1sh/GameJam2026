using System.Collections;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera cam;

    [Header("Movement")]
    [SerializeField] private float panSpeed = 1f;
    [SerializeField] private float zoomSpeed = 5f;

    [Header("Zoom")]
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 30f;

    [Header("Map Bounds")]
    [SerializeField] private Bounds mapBounds;

    [Header("Origin Values")]
    [SerializeField] private float camDistance = 30f;
    [SerializeField] private float originHeight = 45f;

    private Vector3 lastMousePosition;

    private void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        ClampCamera();
    }

    private void Update()
    {
        HandleMouseInput();
        HandleTouchInput();

        ClampCamera();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePosition = Input.mousePosition;

            Vector3 movement = GetWorldMovement(lastMousePosition, currentMousePosition);

            transform.position -= movement * panSpeed;

            lastMousePosition = currentMousePosition;
        }

        float scroll = Input.mouseScrollDelta.y;

        if (scroll != 0)
        {
            Zoom(scroll * zoomSpeed);
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 movement = GetWorldMovement(touch.position - touch.deltaPosition, touch.position);

                transform.position -= movement * panSpeed;
            }
        }

        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 previousPos0 = touch0.position - touch0.deltaPosition;

            Vector2 previousPos1 = touch1.position - touch1.deltaPosition;

            float previousDistance = Vector2.Distance(previousPos0, previousPos1);

            float currentDistance = Vector2.Distance(touch0.position, touch1.position);

            float difference = currentDistance - previousDistance;

            Zoom(-difference * 0.01f * zoomSpeed);
        }
    }

    private Vector3 GetWorldMovement(Vector2 oldScreenPosition, Vector2 newScreenPosition)
    {
        Ray oldRay = cam.ScreenPointToRay(oldScreenPosition);
        Ray newRay = cam.ScreenPointToRay(newScreenPosition);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(oldRay, out float oldDistance) && groundPlane.Raycast(newRay, out float newDistance))
        {
            Vector3 oldWorldPosition = oldRay.GetPoint(oldDistance);
            Vector3 newWorldPosition = newRay.GetPoint(newDistance);

            return newWorldPosition - oldWorldPosition;
        }

        return Vector3.zero;
    }

    private void Zoom(float amount)
    {
        Vector3 forward = transform.forward;

        Vector3 newPosition = transform.position + forward * amount;

        // Calculate zoom based on distance from ground
        float height = newPosition.y;

        height = Mathf.Clamp(height, minZoom, maxZoom);

        newPosition.y = height;

        transform.position = newPosition;
    }

    private void ClampCamera()
    {
        Vector3 position = transform.position;

        float mapMinX = mapBounds.min.x;
        float mapMaxX = mapBounds.max.x;

        float mapMinZ = mapBounds.min.z;
        float mapMaxZ = mapBounds.max.z;

        // Approximate visible size based on camera angle
        float height = position.y;

        float verticalSize = height * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

        float horizontalSize = verticalSize * cam.aspect;

        position.x = Mathf.Clamp(position.x, mapMinX + horizontalSize, mapMaxX - horizontalSize);

        position.z = Mathf.Clamp(position.z, mapMinZ + verticalSize, mapMaxZ - verticalSize);

        transform.position = position;
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

    public void FocusOn(HexTile tile)
    {
        if (tile == null)
        {
            Debug.LogWarning("Cannot focus camera, tile is null!");
            return;
        }

        Vector3 target = tile.transform.position;

        Vector3 cameraForward = transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 offset = -cameraForward * camDistance;

        Vector3 targetPosition = target + offset;
        targetPosition.y = originHeight;

        StartCoroutine(MoveToPosition(targetPosition));

    }

    public void SetMapBounds(Bounds bounds)
    {
        mapBounds = bounds;
        ClampCamera();
    }
}
