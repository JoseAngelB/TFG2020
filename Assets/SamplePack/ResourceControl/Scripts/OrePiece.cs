using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrePiece : MonoBehaviour
{

    public int index = -1;

    void OnMouseDown()
    {
        var evnt = treePieceHit.Create(transform.root.GetComponent<OreController>().entity, Bolt.EntityTargets.OnlyOwner);
        evnt.index = index;
        evnt.Send();
    }
}
