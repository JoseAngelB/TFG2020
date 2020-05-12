using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "ShipScene")]
public class ShipCallbacks : Bolt.GlobalEventListener
{

    //Spawn Server Player
    public override void SceneLoadLocalDone(string map)
    {
        BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.ShipPlayer);
        player.TakeControl();
    }

    //Spawn Client Player
    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.ShipPlayer);
        player.AssignControl(connection);
    }
}
