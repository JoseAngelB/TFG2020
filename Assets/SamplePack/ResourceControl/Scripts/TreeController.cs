using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : Bolt.EntityEventListener<ITreeState>
{
    public GameObject pieceParent;

    int deadPieces = 0;

    //public treePieceController[] treePiece = new treePieceController[0];
    public List<ChopTreePiece> treePiece = new List<ChopTreePiece>();

    bool gotScale = false;
    public GameObject trunk;
    public GameObject top;


    public override void OnEvent(treePieceHit evnt)
    {
        state.choppedPieces[evnt.index] = 1;
        deadPieces++;

        if (deadPieces > 10)
        {
            state.chopped = true;
        }

        //choppedPieces
    }

    public void OnChop(Bolt.IState state2, string path, Bolt.ArrayIndices indices)
    {
        int index = indices[0];

        bool a = state.choppedPieces[index] == 0;

        treePiece[index].gameObject.SetActive(a);


        //IActorState actorState = (IActorState)state;
        // The changed property:
        // actorState.stats[index]
    }

    public override void Attached()
    {
        int i = 0;

        foreach (ChopTreePiece child in pieceParent.transform.GetComponentsInChildren<ChopTreePiece>())
        {
            child.GetComponent<ChopTreePiece>().index = i;
            treePiece.Add(child.GetComponent<ChopTreePiece>());
            i++;
        }

        state.AddCallback("choppedPieces[]", OnChop);

        state.AddCallback("chopped", test0);
        //Invoke("test0", 0.1f);
    }

    void test0()
    {
        if (entity.IsOwner)
        {
            top.GetComponent<Rigidbody>().isKinematic = false;
            top.GetComponent<Rigidbody>().AddForceAtPosition(transform.position, transform.forward * 1000);
        }
        trunk.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (gotScale == false)
        {
            if (entity.IsAttached)
            {
                if (state.scale != 0)
                {
                    transform.localScale = new Vector3(state.scale, state.scale, state.scale);
                    gotScale = true;
                }
            }
        }

    }
    private void FixedUpdate()
    {
        if (entity.IsOwner == false)
        {
            if (state.position != Vector3.zero)
            {

                //Debug.Log(state.position);

                // if (count > 4)
                // {

                top.transform.localPosition = state.position;

                //  if (state.rotation != Quaternion.identity)
                top.transform.localRotation = state.rotation;

                //   }
                //   else count++;
            }
        }
        else
        {
            if (true)
            {
                state.position = top.transform.localPosition;


                state.rotation = top.transform.localRotation;
            }

        }
    }

}
