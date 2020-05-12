using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCallbacks : Bolt.GlobalEventListener
{
    public bool skip;

    CursorLockMode wantedMode;

    public override void SceneLoadLocalDone(string map)
    {
        if (BoltNetwork.IsServer)
        {
            if (skip == false)
                foreach (Terrain terrain in Terrain.activeTerrains)
                {
                    ResourceManager RM = terrain.GetComponent<ResourceManager>();

                    //int numberOfTreeResourceEntitiesNeeded = (terrain.terrainData.treeInstanceCount - 1) / RM.resourceEntityCap + 1;

                    if (RM != null)
                    {
                        for (int i = 0; i < RM.numberOfTreeResourceEntitiesNeeded; i++)
                        {


                            BoltEntity BE = BoltNetwork.Instantiate(BoltPrefabs.ResourceEntity);
                            ResourceEntityController REC = BE.GetComponent<ResourceEntityController>();

                            REC.state.index = i;
                            REC.state.Name = terrain.name;

                            //REC.RM = terrain.GetComponent<resourceManager>();
                            RM.RECs.Add(REC);
                        }
                    }

                    ResourceManagerGeneric RM2 = terrain.GetComponent<ResourceManagerGeneric>();

                    //int numberOfTreeResourceEntitiesNeeded = (terrain.terrainData.treeInstanceCount - 1) / RM.resourceEntityCap + 1;

                    if (RM2 != null)
                    {
                        for (int i = 0; i < RM2.numberEntitiesNeeded; i++)
                        {


                            BoltEntity BE = BoltNetwork.Instantiate(BoltPrefabs.ResourceEntityGeneric);
                            ResourceEntityControllerGeneric REC = BE.GetComponent<ResourceEntityControllerGeneric>();

                            REC.state.index = i;
                            REC.state.Name = terrain.name;

                            //REC.RM = terrain.GetComponent<resourceManager>();
                            RM2.RECs.Add(REC);


                        }
                    }


                    //  BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.Player, new Vector3(1289.76f, 36.81f, 1610.84f), Quaternion.identity);
                    BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.Player, new Vector3(0, 110f, 0), Quaternion.identity);
                    player.TakeControl();
                }
        }
    }

    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        if (BoltNetwork.IsServer)
        {
            BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.Player, new Vector3(0, 110f, 1), Quaternion.identity);
            // BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.Player, new Vector3(1289.76f, 36.81f, 1610.84f), Quaternion.identity);
            player.AssignControl(connection);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            if (wantedMode == CursorLockMode.Locked)
                wantedMode = CursorLockMode.None;
            else wantedMode = CursorLockMode.Locked;

        Cursor.lockState = wantedMode;
    }
}
