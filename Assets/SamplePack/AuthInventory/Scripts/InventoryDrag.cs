using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class InventoryDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler
{
    public int slot = -1;

#pragma warning disable 0414    
    Transform startParent;
    Vector3 startPosition;
#pragma warning restore 0414

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Debug.Log("Left click");
        else if (eventData.button == PointerEventData.InputButton.Middle)
            Debug.Log("Middle click");
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right click");
            var evnt = destroySlot.Create(Bolt.GlobalTargets.OnlyServer);
            evnt.slot = eventData.pointerDrag.GetComponent<InventoryDrag>().slot;
            evnt.Send();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = false;
        startParent = transform.parent;
        startPosition = transform.position;

        if (GetComponentInParent<InventoryController>().selectedSlot == null)
            GetComponentInParent<InventoryController>().selectedSlot = gameObject;
        else if (GetComponentInParent<InventoryController>().selectedSlot == gameObject)
            GetComponentInParent<InventoryController>().selectedSlot = null;
        else
        {
            var evnt = swapSlots.Create(Bolt.GlobalTargets.OnlyServer);
            if (GetComponentInParent<InventoryController>().selectedSlot.GetComponent<InventoryDrop>())
                evnt.from = GetComponentInParent<InventoryController>().selectedSlot.GetComponent<InventoryDrop>().slot;
            else evnt.from = GetComponentInParent<InventoryController>().selectedSlot.GetComponent<InventoryDrag>().slot;
            evnt.to = slot;
            evnt.Send();
            GetComponentInParent<InventoryController>().selectedSlot = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponentInParent<InventoryController>().selectedSlot = null;
        GetComponent<Image>().raycastTarget = true;
        transform.localPosition = Vector3.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }
}
