#if false
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vrPlayerController : Bolt.EntityEventListener<IvrPlayerState>
{


    public override void Attached()
    {
        if (entity.IsOwner)
        {

            state.SetTransforms(state.head, GameObject.Find("VRPLAYER").GetComponent<VRTK.VRTK_SDKManager>().actualHeadset.transform);
            state.SetTransforms(state.leftHand, GameObject.Find("VRPLAYER").GetComponent<VRTK.VRTK_SDKManager>().actualLeftController.transform);
            state.SetTransforms(state.rightHand, GameObject.Find("VRPLAYER").GetComponent<VRTK.VRTK_SDKManager>().actualRightController.transform);
        }
        else
        {
            state.SetTransforms(state.head, transform.GetChild(0));
            state.SetTransforms(state.leftHand, transform.GetChild(1));
            state.SetTransforms(state.rightHand, transform.GetChild(2));
        }
    }

}
#endif