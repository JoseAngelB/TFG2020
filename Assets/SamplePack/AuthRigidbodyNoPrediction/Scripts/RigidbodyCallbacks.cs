using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "AuthRigidbodyNoPredicition")]
public class RigidbodyCallbacks : Bolt.GlobalEventListener
{

    public override void SceneLoadLocalDone(string map)
    {
        BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.TankEntity);
        player.TakeControl();
    }


    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.TankEntity, new Vector3(0, 0, -5f), Quaternion.identity);
        player.AssignControl(connection);
    }
}
