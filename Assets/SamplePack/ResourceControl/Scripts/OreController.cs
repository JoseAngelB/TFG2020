using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreController : Bolt.EntityEventListener<IOreState>
{
    int deadPieces = 0;
    public List<OrePiece> orePiece = new List<OrePiece>();
    public override void Attached()
    {
        int i = 0;
        foreach (Transform t in transform)
        {
            t.GetComponent<OrePiece>().index = i;
            orePiece.Add(t.GetComponent<OrePiece>());
            i++;
        }

        state.AddCallback("pieces[]", OnChop);
    }
    public override void OnEvent(treePieceHit evnt)
    {
        state.pieces[evnt.index] = 1;
        deadPieces++;

        if (deadPieces > 10)
        {
            //state.chopped = true;
        }

        //choppedPieces
    }

    public void OnChop(Bolt.IState state2, string path, Bolt.ArrayIndices indices)
    {
        int index = indices[0];

        bool a = state.pieces[index] == 0;

        orePiece[index].gameObject.SetActive(a);


        //IActorState actorState = (IActorState)state;
        // The changed property:
        // actorState.stats[index]
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
