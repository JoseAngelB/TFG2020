using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceEntityControllerGeneric : Bolt.EntityEventListener<IResourceState>
{


    bool nameInit = false;

    public ResourceManagerGeneric RM;

    public override void Attached()
    {

        state.AddCallback("Name", () =>
        {
            foreach (Terrain T in Terrain.activeTerrains)
            {
                if (T.name == state.Name)
                {
                    RM = T.GetComponent<ResourceManagerGeneric>();
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
                    RM = T.GetComponent<ResourceManagerGeneric>();
                    nameInit = true;
                    break;
                }
            }

        }

        

        int index = indices[0] + (state.index * RM.resourceEntityCap);

        // The changed property:
        // actorState.stats[index]

        bool a = state.ResourceDead[indices[0]] == 1;

        RM.resourceDatas[index].dead = a;

    }

    
    
}
