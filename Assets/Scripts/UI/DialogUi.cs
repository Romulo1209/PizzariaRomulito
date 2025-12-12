using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogUi : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnClicked;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke();
    }
}
