using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerCallbacks : Bolt.GlobalEventListener
{
    //Callback script that instantiates players and gives control

    public override void SceneLoadLocalDone(string map)
    {
        if (BoltNetwork.IsServer)
        {
            BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.PlatformerPlayer);
            entity.TakeControl();
        }
    }

    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        if (BoltNetwork.IsServer)
        {
            BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.PlatformerPlayer);
            entity.AssignControl(connection);
        }
    }
}
