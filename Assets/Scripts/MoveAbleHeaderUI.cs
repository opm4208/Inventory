using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveAbleHeaderUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField]
    private Transform targetUI;

    private Vector2 beginPoint;
    private Vector2 MovePoint;

    private void Awake()
    {
        if(targetUI == null)
            targetUI = transform;
    }

    void IPointerDownHandler.OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        Debug.Log("down");
        beginPoint = targetUI.position;
        MovePoint = eventData.position;
    }
    
    void IDragHandler.OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        Debug.Log("drag");
        targetUI.position = beginPoint+(eventData.position-MovePoint);
    }
}
