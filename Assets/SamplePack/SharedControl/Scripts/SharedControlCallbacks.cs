using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "SharedControl")]
public class SharedControlCallbacks : Bolt.GlobalEventListener
{

    public override void SceneLoadLocalDone(string map)
    {
        BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.SharedPlayer);
        player.TakeControl();
    }

}
