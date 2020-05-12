using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManyAgentCallbacks : Bolt.GlobalEventListener
{
    public int agentCount = 10;


    public override void SceneLoadLocalDone(string map)
    {
        if (BoltNetwork.IsServer)
        {
             for (int i = 0; i < agentCount; i++)
                  BoltNetwork.Instantiate(BoltPrefabs.ManyAgentEntity, new Vector3(50f, 1, 20f), Quaternion.identity);
       

        }
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
