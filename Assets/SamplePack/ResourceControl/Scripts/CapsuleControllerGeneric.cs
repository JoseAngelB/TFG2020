using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleControllerGeneric : MonoBehaviour
{

    public bool kill = false;

    public int index;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (BoltNetwork.IsServer)
        {
            if (kill == true)
            {
              

                ResourceManagerGeneric RM = transform.parent.GetComponent<ResourceManagerGeneric>();

                int entityIndex = index / RM.resourceEntityCap;
                kill = false;
                RM.RECs[entityIndex].state.ResourceDead[index] = 1;

                index = -1;


                transform.localPosition = new Vector3(0, -100f, 0);

                // tn.GetComponent<resourceManager2>().poolNeedsUpdating = true;

                RM.test0();

            }
        }
    }
}