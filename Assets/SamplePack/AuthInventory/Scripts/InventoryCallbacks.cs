using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCallbacks : Bolt.GlobalEventListener
{
    public InventoryPlayerController inventoryPlayerController;

    public void Start()
    {
        // instantiate Player entity in respect to inventory
        inventoryPlayerController = GameObject.Find("InventoryEntity").GetComponent<InventoryPlayerController>();
    }

    public override void OnEvent(swapSlots evnt)
    {
        //swap the contents of two items slots
        inventoryPlayerController.swapSlots(evnt);
    }
    public override void OnEvent(destroySlot evnt)
    {
        //wipe the specificed item slot 
        inventoryPlayerController.state.items[evnt.slot].ID = 0;
        inventoryPlayerController.myInventory.refreshItem(evnt.slot);
    }
}
