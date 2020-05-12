using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour]
public class NetworkCallbacks : Bolt.GlobalEventListener
{
    public override void SceneLoadLocalDone(string scene)
    {
        // aquí ponemos lo que pasa localmente cada vez que se carga una nueva escena

        //var spawnPosition = new Vector3(Random.Range(-16, 16), 0, Random.Range(-16, 16));
        //BoltNetwork.Instantiate(BoltPrefabs.Carta, spawnPosition, Quaternion.identity);
    }
}