using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopTreeController : Bolt.EntityEventListener<ITreeState>
{
    bool chopped = false;

    int deadPieces = 0;

    //public treePieceController[] treePiece = new treePieceController[0];
    public List<ChopTreePiece> treePiece = new List<ChopTreePiece>();

    public Rigidbody topRigidbody;
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
        foreach (Transform t in transform.GetChild(0).GetChild(0))
        {
            t.GetComponent<ChopTreePiece>().index = i;
            treePiece.Add(t.GetComponent<ChopTreePiece>());
            i++;
        }


        state.SetTransforms(state.transform, transform);
        state.AddCallback("choppedPieces[]", OnChop);
    }



    private void FixedUpdate()
    {
        if (entity.IsOwner == false)
        {
            if (state.position != Vector3.zero)
            {
                topRigidbody.position = state.position;
                topRigidbody.rotation = state.rotation;

            }
        }
        else
        {
            if (chopped)
            {
                state.position = topRigidbody.position;

                
                state.rotation = topRigidbody.rotation;
            }

        }
    }

    public void pieceChopped(GameObject go)
    {
        state.choppedPieces[go.GetComponent<ChopTreePiece>().index] = 1;

        // GameObject test = treePiece.Find(item => item == go).gameObject;
        Debug.Log(go.name + " " + go.transform.GetSiblingIndex());

        //treePiece.Remove(go);
        //go.SetActive(false);
        deadPieces++;
        if (deadPieces > 10)
        {
            chopped = true;
            topRigidbody.isKinematic = false;
            test0();
        }
    }

    void test0()
    {
        topRigidbody.AddForce(Vector3.forward * 100f);
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
