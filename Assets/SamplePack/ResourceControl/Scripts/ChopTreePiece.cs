using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopTreePiece : MonoBehaviour
{
    public int index = -1;
    public void Test0()
    {
        transform.root.GetComponent<ChopTreeController>().pieceChopped(gameObject);
    }

    void OnMouseDown()
    {
        var evnt = treePieceHit.Create(transform.root.GetComponent<TreeController>().entity, Bolt.EntityTargets.OnlyOwner);
        evnt.index = index;
        evnt.Send();       
    }
}