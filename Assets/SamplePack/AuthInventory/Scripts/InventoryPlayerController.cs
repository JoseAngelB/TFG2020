﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPlayerController : Bolt.EntityEventListener<IInventoryState>
{
    public InventoryController myInventory;
   

    public void swapSlots(swapSlots evnt)
    {
        //step1: check if match
        //step2: check is stackable
        //step3: check if "to" is full, if full then just swap
        //check if they add up to beyond stack cap
        //if they not beyond cap, add to "to" and make "from" empty
        //otherwise add to "to" until full and subtract from "from"

        if (state.items[evnt.to].ID == state.items[evnt.from].ID)
        {
            if (myInventory.lookUpID(state.items[evnt.to].ID).stackable)
            {
                if (myInventory.lookUpID(state.items[evnt.to].ID).stackSize > state.items[evnt.to].quantity)
                {
                    int moveQuanity = 0;
                    //how many item units are moving

                    int moveSpace = myInventory.lookUpID(state.items[evnt.to].ID).stackSize - state.items[evnt.to].quantity;
                    //how much room is available in "to"

                    int fromQuantity = state.items[evnt.from].quantity;

                    if (moveSpace >= fromQuantity)
                    {
                        moveQuanity = fromQuantity;
                    }
                    else
                    {
                        moveQuanity = moveSpace;
                    }

                    state.items[evnt.to].quantity += moveQuanity;
                    state.items[evnt.from].quantity -= moveQuanity;

                    if (state.items[evnt.from].quantity == 0)
                        state.items[evnt.from].ID = 0;

                    return;
                }
            }
        }

        int oldItem = state.items[evnt.to].ID;
        state.items[evnt.to].ID = state.items[evnt.from].ID;
        state.items[evnt.from].ID = oldItem;

        int oldQuantity = state.items[evnt.to].quantity;
        state.items[evnt.to].quantity = state.items[evnt.from].quantity;
        state.items[evnt.from].quantity = oldQuantity;
    }

    public void OnStatsChanged(Bolt.IState myState, string path, Bolt.ArrayIndices indices)
    {
        if (state.items[indices[0]].ID == 0)
        {
            myInventory.clearItem(indices[0]);
        }
        else
        {
            myInventory.refreshItem(indices[0]);

        }
      
    }

    public override void Attached()
    {

        myInventory = GameObject.Find("Canvas").transform.GetChild(1).GetComponent<InventoryController>();
        myInventory.PlayerController = this;
        state.AddCallback("slots", () => { myInventory.instantiateSlots(state.slots); });
        state.AddCallback("items[]", (x, y, z) => { OnStatsChanged((IInventoryState)x, (string)y, (Bolt.ArrayIndices)z); });

        
        
        if (BoltNetwork.IsServer)
        {
            //0 = unitialized
            //-1 = empty
            //

            state.slots = 5;          
        }       

    }


    public void addItem(int ID, int quantity)
    {
        //if stackable try adding to non full stacks
        ItemData ItemData = myInventory.lookUpID(ID);
        if (ItemData.stackable)
        {
            int currentQuantity = quantity;


            for (int i = 0; i < state.slots; i++)
            {
                if (state.items[i].ID == ID)
                {
                    if (state.items[i].quantity < ItemData.stackSize)
                    {
                        int moveQuanity;

                        int moveSpace = ItemData.stackSize - state.items[i].quantity;

                        if (moveSpace >= quantity)
                        {
                            moveQuanity = quantity;
                        }
                        else
                        {
                            moveQuanity = moveSpace;
                        }

                        state.items[i].quantity += moveQuanity;
                        currentQuantity -= moveQuanity;
                        if (currentQuantity == 0)
                        {
                            return;
                        }

                    }
                }

            }
        }




        for (int i = 0; i < state.slots; i++)
        {
            if (state.items[i].ID == 0)
            {
                state.items[i].ID = ID;
                state.items[i].quantity = quantity;
                return;
            }

        }


        Debug.Log("no space");
    }

    // Update is called once per frame
    void Update()
    {
        if (BoltNetwork.IsServer)
        {
            if (entity.IsAttached)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    addItem(1, 1);
                }
                if (Input.GetKeyDown(KeyCode.R))
                    addItem(2, 1);


                if (Input.GetKeyDown(KeyCode.Q))
                    state.slots--;
                if (Input.GetKeyDown(KeyCode.W))
                    state.slots++;
                if (Input.GetKeyDown(KeyCode.T))
                {
                    state.items[0].ID = 2;
                    state.items[0].quantity = 11;
                    //int oldItem = state.items[0].ID;
                    // state.items[0].ID = state.items[1].ID;
                    //state.items[1].ID = oldItem;


                }


            }

        }
    }
}
