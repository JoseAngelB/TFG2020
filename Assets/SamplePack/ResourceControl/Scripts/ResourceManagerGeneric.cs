using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManagerGeneric : MonoBehaviour
{

    float timer = 0;
    public int resourceEntityCap = 1000;


    [System.Serializable]
    public class resourceData
    {


        public GameObject gameObject;
        public bool dead;
        public float distance;
    }

    public int numberEntitiesNeeded;
    public Transform resourceRoot;

    public List<ResourceEntityControllerGeneric> RECs = new List<ResourceEntityControllerGeneric>();

    public List<resourceData> resourceDatas = new List<resourceData>();
    public List<resourceData> resourceDataNearby = new List<resourceData>();

    List<targetData> targets = new List<targetData>();
    public Terrain terrain;


    public class targetData
    {
        public GameObject gameobject;
        public Vector3 position;
        public CapsulePool capsulePool;

        public targetData(GameObject go, Vector3 v3, CapsulePool tpc)
        {
            gameobject = go;
            position = v3;
            capsulePool = tpc;
        }
    }

    // Use this for initialization
    void Start()
    {
        numberEntitiesNeeded = (resourceRoot.childCount - 1) / resourceEntityCap + 1;


        terrain = GetComponent<Terrain>();

        foreach (Transform child in resourceRoot)
        {
            resourceData RD = new resourceData();
            RD.gameObject = child.gameObject;
            resourceDatas.Add(RD);
        }

        Invoke("test0", 1.0f);
    }


    public class CapsulePool
    {

        public int numCapsulesInPool;       // initialized by constructor
        public Vector3 hidingPlace;        // initialized by constructor
        private Transform terrainTransform; // initialized by constructor
        public GameObject[] capsules;       // Created by makeCapsules; invoked by constructor

        private void makeCapsules() // Used by constructor
        {
            for (int i = 0; i < numCapsulesInPool; i++)
            {
                GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                capsule.AddComponent<CapsuleControllerGeneric>();
                capsule.layer = 11;

                CapsuleCollider capsuleCollider = capsule.GetComponent<Collider>() as CapsuleCollider;

                capsuleCollider.center = new Vector3(0, 5, 0);
                capsuleCollider.height = 10;

                // DestroyableTree tree = capsule.AddComponent<DestroyableTree>();
                // tree.terrainIndex = -1;                     // Needs to be set when moved  TODO
                capsule.transform.position = hidingPlace;   // Needs to be set when moved  
                                                            // capsule.tag = "Tree";
                capsule.transform.parent = terrainTransform;
                // capsule.GetComponent<Renderer>().enabled = false;
                capsules[i] = capsule;
            }
        }

        public CapsulePool(Vector3 _hidingPlace, Transform _terrainTransform, int _numCapsulesInPool = 10)
        {
            numCapsulesInPool = _numCapsulesInPool;
            terrainTransform = _terrainTransform;
            hidingPlace = new Vector3(_hidingPlace.x, _hidingPlace.y, _hidingPlace.z);
            capsules = new GameObject[numCapsulesInPool];
            makeCapsules();
        }

    }

    public targetData addTarget(GameObject player)
    {
        foreach (targetData TD in targets)
        {
            if (TD.gameobject == player)
            {
                Debug.Log("ree");
                return TD;
            }
        }

        targetData targetData = new targetData(player, player.transform.position, new CapsulePool(new Vector3(0, -100, 0), terrain.transform, 5));
        Debug.Log(targetData.position);
        targets.Add(targetData);
        return targetData;
    }

    public void removeTarget(targetData TD)
    {
        foreach (GameObject GO in TD.capsulePool.capsules)
        {
            GameObject.Destroy(GO);
        }

        targets.Remove(TD);

    }



    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;

        if (timer > 1f)
        {
            test0();
            timer = 0;

        }

    }


    public void test0()
    {
        //Debug.Log("a");


        foreach (targetData TD in targets)
        {
            //if (Vector3.Distance(TD.position, TD.gameobject.transform.position) > 2f)
            if (true)
            {
                TD.position = TD.gameobject.transform.position;


                float furthestDistance = 0;
                int furthestIndex = -1;

                resourceDataNearby.Clear();

                foreach (resourceData RD in resourceDatas)
                {
                    if (RD.dead == false)
                    {

                        /*    
                    if there is still empty space, then add resource then, 
                    if more distant than current furthest resource,
                    then update furthestDistance and furthestIndex 

                    if there is not empty space, compare distance, if closer than the further distance, 
                    replace the resource referenced in the furthest index, 
                    then find which is the new furthest and update distance and index                     
                       */


                        if (resourceDataNearby.Count < TD.capsulePool.numCapsulesInPool)
                        {
                            float dist = Vector3.Distance(TD.position, RD.gameObject.transform.position);
                            if (dist > furthestDistance)
                            {
                                furthestIndex = resourceDataNearby.Count;
                                furthestDistance = dist;

                            }
                            RD.distance = dist;
                            resourceDataNearby.Add(RD);
                        }
                        else
                        {
                            float dist = Vector3.Distance(TD.position, RD.gameObject.transform.position);
                            if (dist < furthestDistance)
                            {
                                RD.distance = dist;
                                resourceDataNearby[furthestIndex] = RD;

                                furthestDistance = 0;
                                int count2 = 0;
                                foreach (resourceData myRD in resourceDataNearby)
                                {
                                    if (furthestDistance < myRD.distance)
                                    {
                                        furthestDistance = myRD.distance;
                                        furthestIndex = count2;

                                    }
                                    count2++;
                                }
                            }
                        }
                    }
                }

                int count = 0;
                foreach (GameObject go in TD.capsulePool.capsules)
                {
                    if (count > (resourceDataNearby.Count - 1))
                        break;

                    if (count > TD.capsulePool.numCapsulesInPool)
                    {
                        go.transform.position = TD.capsulePool.hidingPlace;
                    }
                    else
                    {
                        go.transform.position = resourceDataNearby[count].gameObject.transform.position;
                        go.GetComponent<CapsuleControllerGeneric>().kill = false;
                        go.GetComponent<CapsuleControllerGeneric>().index = resourceDataNearby[count].gameObject.transform.GetSiblingIndex();
                    }
                    count++;
                }

            }
        }  
    }
}
