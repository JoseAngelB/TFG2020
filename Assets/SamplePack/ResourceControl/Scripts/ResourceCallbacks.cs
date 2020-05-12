using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[BoltGlobalBehaviour(BoltNetworkModes.Client, "resourceTest0")]
public class ResourceCallbacks : Bolt.GlobalEventListener
{
    //respawn test code

    /*
    public override void OnEvent(treeRespawn evnt)
    {
        Terrain myTerrain = Terrain.activeTerrain;
        myTerrain.GetComponent<resourceManager>().TerrainSector.resourceSectors[evnt.sector][evnt.index].dead = false;

        List<TreeInstance> trees = new List<TreeInstance>(myTerrain.terrainData.treeInstances);
        resourceManager.resourceSectorItem RSI = myTerrain.GetComponent<resourceManager>().TerrainSector.resourceSectors[evnt.sector][evnt.index];
        //int treeIndex = myTerrain.GetComponent<resourceManager>().TerrainSector.resourceSectors[evnt.sector][evnt.index].treeIndexInTerrain;
        TreeInstance PogChamp;
        //trees[evnt.ID] = new TreeInstance();
        PogChamp = trees[RSI.treeIndexInTerrain];
        //Debug.Log(PogChamp.heightScale);
        //Debug.Log(PogChamp.widthScale);
        // BoltEntity newTree = BoltNetwork.Instantiate(BoltPrefabs.Broadleaf_Desktop_Entity, transform.position + new Vector3(0, 0f, 0), Quaternion.AngleAxis(PogChamp.rotation * Mathf.Rad2Deg, Vector3.up));
        // newTree.gameObject.transform.localScale = new Vector3(PogChamp.widthScale, PogChamp.heightScale, PogChamp.widthScale);
        PogChamp.position.y = 0.166666666666666666666666666f;
        trees[RSI.treeIndexInTerrain] = PogChamp;
        //trees.RemoveAt(this);
        myTerrain.terrainData.treeInstances = trees.ToArray();
        //tn.Flush();
        //            resourceSectors[tsrList[i].sector][tsrList[i].listIndex].dead


        myTerrain.GetComponent<resourceManager>().poolNeedsUpdating = true;


    }
    */

}

