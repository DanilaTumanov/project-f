using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonControlView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action OnButtonDown; 
    
    public bool IsPressed { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        OnButtonDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
    }
}