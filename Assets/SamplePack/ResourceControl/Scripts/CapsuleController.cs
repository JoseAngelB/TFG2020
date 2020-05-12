using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CapsuleController : MonoBehaviour
{
    public bool kill = false;
    public int sector; //if -1 then this capsule needs to be reallocated
    public int index; //index within the sector
    public int treeIndex; //absolute index?


    public void Update()
    {


        if (BoltNetwork.IsServer)
            if (sector != -1)
                if (kill == true)
                {
                    Terrain tn = transform.parent.GetComponent<Terrain>();

                    ResourceManager RM = transform.parent.GetComponent<ResourceManager>();


                    int entityIndex = treeIndex / RM.resourceEntityCap;
                    int actualTreeIndex = treeIndex % RM.resourceEntityCap;

                    /*
                    treeIndex = 0, resourceEntityCap = 2, then entity Index = 0


                    */


                    /*
                    var evnt = treeDeadEvent.Create(Bolt.GlobalTargets.AllClients);
                    evnt.sector = sector;
                    evnt.index = index;

                    evnt.terrainName = tn.name;
                    evnt.Send();
                    */

                    kill = false;
                    RM.TerrainSector.resourceSectors[sector][index].dead = true;
                    RM.RECs[entityIndex].state.ResourceDead[actualTreeIndex] = 1;
                    //  transform.position = Vector3.zero;
                    sector = -1;



                    // Debug.Log("wew");
                    //   terrain.terrainData.treeInstances[i].position.y = 0.1f;

                    List<TreeInstance> trees = new List<TreeInstance>(tn.terrainData.treeInstances);

                    TreeInstance TI;
                    //trees[evnt.ID] = new TreeInstance();
                    TI = trees[treeIndex];
                    //Debug.Log(PogChamp.heightScale);
                    //Debug.Log(PogChamp.widthScale);
                    // Debug.Log("network tree");
                    BoltEntity newTree = BoltNetwork.Instantiate(BoltPrefabs.Fir05_NM, transform.position + new Vector3(0, 0f, 0), Quaternion.AngleAxis(TI.rotation * Mathf.Rad2Deg, Vector3.up));
                    newTree.GetComponent<TreeController>().state.scale = TI.widthScale;

                    newTree.gameObject.transform.localScale = new Vector3(TI.widthScale, TI.heightScale, TI.widthScale);
                    TI.position.y = 0f;
                    trees[treeIndex] = TI;
                    //trees.RemoveAt(this);
                    tn.terrainData.treeInstances = trees.ToArray();
                    //tn.Flush();
                    //            resourceSectors[tsrList[i].sector][tsrList[i].listIndex].dead


                    tn.GetComponent<ResourceManager>().poolNeedsUpdating = true;
                    // Debug.Log(TI.rotation);


                    //  GameObject myTree = GameObject.Find("test0");
                    // //  myTree.transform.position = transform.position + new Vector3(0, -1.9f, 0);
                    //   myTree.transform.rotation = Quaternion.AngleAxis(PogChamp.rotation * Mathf.Rad2Deg, Vector3.up);
                    //  Instantiate(tn.GetComponent<resourceManager>().treePrefab, transform.position + new Vector3(0, -1.9f, 0), Quaternion.AngleAxis(PogChamp.rotation * Mathf.Rad2Deg, Vector3.up));
                    // BoltNetwork.Instantiate(BoltPrefabs.Broadleaf_Desktop_Entity, transform.position + new Vector3(0, -1.9f, 0), Quaternion.AngleAxis(PogChamp.rotation * Mathf.Rad2Deg, Vector3.up));
                }
    }
}
