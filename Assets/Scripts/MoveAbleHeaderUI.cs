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
    public bool itemDrag;

    private void Awake()
    {
        if(targetUI == null)
            targetUI = transform;
    }

    void IPointerDownHandler.OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        beginPoint = targetUI.position;
        MovePoint = eventData.position;
    }
    
    void IDragHandler.OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if(!itemDrag)
            targetUI.position = beginPoint+(eventData.position-MovePoint);
    }
}
