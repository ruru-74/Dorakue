using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    [HideInInspector] public GameObject nowFrame_Obj;
    [HideInInspector] public Item item;
    [HideInInspector] public InventoryController inventoryController;
    private Vector2 prePosition;
    public void OnBeginDrag(PointerEventData eventData)
    {
        prePosition = transform.position;
        transform.SetParent(Singleton.MainCanvas.transform, true);//親子関係解除
        InventoryController.isDrag = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        inventoryController.ItemOnEndDrag(this, eventData, nowFrame_Obj, item, prePosition);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        inventoryController.ItemClick();
    }
}