using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CutterUi : MonoBehaviour, IPointerClickHandler
{
    public Camera worldCamera;
    public RawImage rawImage;
    public static event Action<GameObject> OnClicked;

    void OnValidate()
    {
        rawImage = GetComponent<RawImage>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject hitObj = Utils.RawImageToWorld(
            rawImage: rawImage,
            worldCamera: worldCamera,
            eventData: eventData
        );

        OnClicked?.Invoke(hitObj);
    }
}
