using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Utils
{
    public static GameObject RawImageToWorld(
        RawImage rawImage,
        Camera worldCamera,
        PointerEventData eventData,
        float worldPlaneZ = 0,
        float debugDistance = 0.25f,
        float debugDuration = 1f
    )
    {
        if (rawImage.texture == null || worldCamera == null)
            return null;

        RectTransform rt = rawImage.rectTransform;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rt, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
            return null;

        Vector2 normalized = new Vector2(
            (localPoint.x + rt.rect.width * 0.5f) / rt.rect.width,
            (localPoint.y + rt.rect.height * 0.5f) / rt.rect.height
        );

        Rect uvRect = rawImage.uvRect;
        Vector2 uv = new Vector2(
            uvRect.x + normalized.x * uvRect.width,
            uvRect.y + normalized.y * uvRect.height
        );

        float dist = Mathf.Abs(worldCamera.transform.position.z - worldPlaneZ);
        Vector3 worldPoint = worldCamera.ViewportToWorldPoint(new Vector3(uv.x, uv.y, dist));

        //Debug
        Debug.DrawLine(worldCamera.transform.position, worldPoint, Color.yellow, debugDuration);
        Debug.DrawRay(worldPoint, Vector3.up * debugDistance, Color.cyan, debugDuration);
        Debug.DrawRay(worldPoint, Vector3.right * debugDistance, Color.cyan, debugDuration);
        Debug.DrawRay(worldPoint, -Vector3.up * debugDistance, Color.cyan, debugDuration);
        Debug.DrawRay(worldPoint, -Vector3.right * debugDistance, Color.cyan, debugDuration);

        Collider2D hit = Physics2D.OverlapPoint(new Vector2(worldPoint.x, worldPoint.y));

        if (hit != null)
            return hit.gameObject;

        return null;
    }

    public static Vector3 GetWorldPositionFromRawImage(
        RawImage rawImage,
        Camera uiCamera,
        Camera worldCamera,
        Vector2 screenPosition
    )
    {
        if (rawImage == null || worldCamera == null)
        {
            Vector3 fallback = Camera.main.ScreenToWorldPoint(screenPosition);
            fallback.z = 0f;
            return fallback;
        }

        if (!RectTransformUtility.RectangleContainsScreenPoint(
            rawImage.rectTransform,
            screenPosition,
            uiCamera))
        {
            return Vector3.positiveInfinity;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rawImage.rectTransform,
            screenPosition,
            uiCamera,
            out Vector2 localPoint
        );

        Rect rect = rawImage.rectTransform.rect;
        float px = (localPoint.x - rect.x) / rect.width;
        float py = (localPoint.y - rect.y) / rect.height;

        Vector2 uv = new Vector2(
            Mathf.Clamp01(px),
            Mathf.Clamp01(py)
        );

        Vector3 viewportPoint = new Vector3(uv.x, uv.y, 0f);

        Ray ray = worldCamera.ViewportPointToRay(viewportPoint);

        if (worldCamera.orthographic)
        {
            Vector3 vpWithZ = new Vector3(uv.x, uv.y, Mathf.Abs(worldCamera.transform.position.z));
            Vector3 worldPosOrtho = worldCamera.ViewportToWorldPoint(vpWithZ);
            worldPosOrtho.z = 0f;
            return worldPosOrtho;
        }

        if (Mathf.Abs(ray.direction.z) < 1e-6f)
            return Vector3.positiveInfinity;

        float distance = -ray.origin.z / ray.direction.z;
        Vector3 worldPos = ray.origin + ray.direction * distance;
        worldPos.z = 0f;

        return worldPos;
    }

    public static SlicesCount GetRandomSlice()
    {
        var values = (SlicesCount[])System.Enum.GetValues(typeof(SlicesCount));
        return values[Random.Range(0, values.Length)];
    }
}
