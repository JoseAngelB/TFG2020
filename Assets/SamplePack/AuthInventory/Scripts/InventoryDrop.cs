using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class InventoryDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int slot = -1;

    public void selectSlot()
    {
        if (GetComponentInParent<InventoryController>().selectedSlot == null && transform.childCount != 0)
            GetComponentInParent<InventoryController>().selectedSlot = gameObject;
        else if (GetComponentInParent<InventoryController>().selectedSlot == gameObject)
            GetComponentInParent<InventoryController>().selectedSlot = null;
        else
        {
            if (GetComponentInParent<InventoryController>().selectedSlot)
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

    }

    public void OnDrop(PointerEventData eventData)
    {
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
            EventSystem.current.SetSelectedGameObject(null);
        }     
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
     
    }
}
