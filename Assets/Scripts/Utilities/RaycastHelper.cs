using UnityEngine;

public static class RaycastHelper
{
    // Raycasts from a screen position into the world
    public static bool ScreenPoint(Camera camera, Vector2 screenPosition, out RaycastHit hit, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers)
    {
        Ray ray = camera.ScreenPointToRay(screenPosition);

        return Physics.Raycast(ray, out hit, maxDistance, layerMask);
    }

    // Raycasts from a screen position and returns the hit gameobject
    public static bool ScreenPoint(Camera camera, Vector2 screenPosition, out GameObject hitObject, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers)
    {
        hitObject = null;

        if (!ScreenPoint(camera, screenPosition, out RaycastHit hit, maxDistance, layerMask))
        {
            return false;
        }

        hitObject = hit.collider.gameObject;
        return true;
    }
}
