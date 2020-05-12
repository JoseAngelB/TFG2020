using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceEntityController : Bolt.EntityEventListener<IResourceState>
{
    bool nameInit = false;

    public GameObject stump;
    public ResourceManager RM;

    public override void Attached()
    {

        state.AddCallback("Name", () =>
        {
            foreach (Terrain T in Terrain.activeTerrains)
            {
                if (T.name == state.Name)
                {
                    RM = T.GetComponent<ResourceManager>();
                    nameInit = true;
                    break;
                }
            }
        });


        state.AddCallback("ResourceDead[]", ResourceDead);



    }
    public void ResourceDead(Bolt.IState myState, string path, Bolt.ArrayIndices indices)
    {
        if (nameInit == false)
        {
            foreach (Terrain T in Terrain.activeTerrains)
            {
                if (T.name == state.Name)
                {
                    RM = T.GetComponent<ResourceManager>();
                    nameInit = true;
                    break;
                }
            }

        }


        BoltConsole.Write("a");

        int index = indices[0] + (state.index * RM.resourceEntityCap);

        // The changed property:
        // actorState.stats[index]

        bool a = state.ResourceDead[indices[0]] == 1;
        
        RM.TerrainSector.resourceOrdered[index].ResourceSectorItem.dead = a;





        if (a == false)
        {
            List<TreeInstance> trees = new List<TreeInstance>(RM.terrain.terrainData.treeInstances);

            TreeInstance PogChamp = trees[index];
            PogChamp.position.y = 0.166666666666666666666666666f;
            RM.terrain.terrainData.treeInstances[index] = PogChamp;
            trees[index] = PogChamp;
            //trees.RemoveAt(this);
            RM.terrain.terrainData.treeInstances = trees.ToArray();
            //tn.Flush();
            //            resourceSectors[tsrList[i].sector][tsrList[i].listIndex].dead
            RM.poolNeedsUpdating = true;


        }
        else
        {
            List<TreeInstance> trees = new List<TreeInstance>(RM.terrain.terrainData.treeInstances);

            TreeInstance PogChamp = trees[index];

            ResourceManager.resourceSectorItem RSI = RM.TerrainSector.resourceOrdered[index].ResourceSectorItem;
            GameObject yeet = GameObject.Instantiate(stump, RSI.position, Quaternion.AngleAxis(PogChamp.rotation * Mathf.Rad2Deg, Vector3.up));
            yeet.gameObject.transform.localScale = new Vector3(PogChamp.widthScale, PogChamp.heightScale, PogChamp.widthScale);
            PogChamp.position.y = 0;
            RM.terrain.terrainData.treeInstances[index] = PogChamp;
            trees[index] = PogChamp;
            //trees.RemoveAt(this);
            RM.terrain.terrainData.treeInstances = trees.ToArray();
            //tn.Flush();
            //            resourceSectors[tsrList[i].sector][tsrList[i].listIndex].dead
            RM.poolNeedsUpdating = true;

        }
    }

    
}
