using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ResourceManager : MonoBehaviour
{

    public int numberOfTreeResourceEntitiesNeeded;

    public int resourceEntityCap = 22;

    bool vegetationStudio;

    bool notTree;


    public List<ResourceEntityController> RECs = new List<ResourceEntityController>();



    public bool poolNeedsUpdating;
    public Terrain terrain;

    public GameObject nodeMaster;

    public terrainSector TerrainSector;
    private TreeInstance[] _originalTrees;      // Initialized in Start()

#pragma warning disable 0414
    List<GameObject> nodes = new List<GameObject>();
#pragma warning restore 0414
    List<targetData> targets = new List<targetData>();

    public class resourceSectorRanger
    {
        public int sector;
        public int listIndex;
        public float distance;

        public resourceSectorRanger(int _sector, int _listIndex, float _distance)
        {
            sector = _sector;
            listIndex = _listIndex;
            distance = _distance;
        }
    }

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


    public class resourceSectorItem
    {
        public Vector3 position;
        public int treeIndexInTerrain;
        public bool dead;

        public resourceSectorItem(Vector3 v, int terrainTreeIndex)
        {
            dead = false;
            position = new Vector3(v.x, v.y, v.z);
            treeIndexInTerrain = terrainTreeIndex;
        }
    }

    public class resourceOrderedItem
    {
        public resourceSectorItem ResourceSectorItem;

        public resourceOrderedItem(resourceSectorItem RSI)
        {
            ResourceSectorItem = RSI;
        }
    }

    public class terrainSector
    {
        // Define the grid size we will use to divide the playing space. This can be uneven
        private int sectorSlicesX;
        private int sectorSlicesZ;
        public List<resourceSectorItem>[] resourceSectors;
        public List<resourceOrderedItem> resourceOrdered;
        public Vector3 position;
        public float X_min, X_max, X_range;
        public float Z_min, Z_max, Z_range;

        public terrainSector(Vector3 corner1, Vector3 corner2, int _sectorSlicesX, int _sectorSlicesZ, Vector3 pos)
        {
            resourceOrdered = new List<resourceOrderedItem>();
            resourceSectors = new List<resourceSectorItem>[_sectorSlicesX * _sectorSlicesZ];
            for (int i = 0; i < _sectorSlicesX * _sectorSlicesZ; i++)
                resourceSectors[i] = new List<resourceSectorItem>();


            X_min = Mathf.Min(corner1.x, corner2.x);
            X_max = Mathf.Max(corner1.x, corner2.x);
            X_range = X_max - X_min;

            Z_min = Mathf.Min(corner1.z, corner2.z);
            Z_max = Mathf.Max(corner1.z, corner2.z);
            Z_range = Z_max - Z_min;

            sectorSlicesX = _sectorSlicesX;
            sectorSlicesZ = _sectorSlicesZ;

            position = pos;
        }

        private int SectorXZ_to_SectorIndex(int x, int z)
        {
            // Debug.Log(x + sectorSlicesX * z);
            return x + sectorSlicesX * z;
        }


        private int XZ_to_SectorIndex(float xPos, float zPos)
        {
            int xSec = (int)((sectorSlicesX * xPos) / X_range);
            int zSec = (int)((sectorSlicesZ * zPos) / Z_range);

            return SectorXZ_to_SectorIndex(xSec, zSec);
        }
        private int Vector3_to_SectorIndex(Vector3 v)
        {
            float xPos = v.x - X_min;
            float zPos = v.z - Z_min;

            return XZ_to_SectorIndex(xPos, zPos);
        }


        public void AddTree(Vector3 location, int terrainTreeIndex)
        {
            int sectorIndex = Vector3_to_SectorIndex(location);
            resourceSectorItem tsi = new resourceSectorItem(location, terrainTreeIndex);


            resourceSectors[sectorIndex].Add(tsi);

            resourceOrderedItem ROI = new resourceOrderedItem(tsi);
            resourceOrdered.Add(ROI);
        }

        private List<int> GetListOfTreeSectorsInRange(Vector3 location, float range)
        {
            List<int> result = new List<int>();
            // TODO - make this actually check Range.. currently it just generates the 9box around the start place
            for (int xx = -1; xx <= 1; xx++)
            {
                float xdiff = X_range / sectorSlicesX;
                for (int zz = -1; zz <= 1; zz++)
                {
                    float zdiff = Z_range / sectorSlicesZ;

                    float myX = (location.x + (xx * xdiff));
                    float myZ = (location.z + (zz * zdiff));
                    //   Debug.Log(myX + " " + myZ);

                    if (myX > X_range)
                        myX = X_range - 1f;
                    if (myX < 0)
                        myX = 1;
                    if (myZ < 0)
                        myZ = 1;
                    if (myZ > Z_range)
                        myZ = Z_range - 1f;

                    //if (myX < X_range && myZ < Z_range && myX > 0 && myZ > 0)
                    int myResult = XZ_to_SectorIndex(myX, myZ);
                    if (myResult < 0)
                        Debug.Log("wew");

                    if (result.IndexOf(myResult) == -1)
                        result.Add(myResult);
                    //else Debug.Log("side");
                }
            }

            return result;
        }
        public void MoveCapsulesToClosestTrees(Vector3 playerCurrentPosition, float range, CapsulePool tcp)
        {
            Debug.Log("Moving capsules");
            // Step 1 find the list of closest trees
            List<int> treeSectorsToSearch = GetListOfTreeSectorsInRange(playerCurrentPosition, range);
            List<resourceSectorRanger> tsrList = new List<resourceSectorRanger>();
            foreach (int sectorIndex in treeSectorsToSearch)
            {
                int tsiCount = 0;
                int newSectorIndex = -1;
                // if (sectorSlicesX * sectorSlicesZ < sectorIndex)
                //// {
                //     Debug.Log(sectorIndex);
                newSectorIndex = sectorIndex % (sectorSlicesX * sectorSlicesZ);

                //  }
                //  else newSectorIndex = sectorIndex;


                //TODO: temp fix, why does it sometimes call index with same number as length ?
                //   if (newSectorIndex < resourceSectors.Length)

                foreach (var tsi in resourceSectors[newSectorIndex])
                {

                    float dist = Vector3.Distance(tsi.position - position, playerCurrentPosition);
                    tsrList.Add(new resourceSectorRanger(newSectorIndex, tsiCount, dist));
                    tsiCount++;
                }
            }

            // Step 2 - Sort the list
            tsrList.Sort(delegate (resourceSectorRanger tsr1, resourceSectorRanger tsr2)
            {
                return tsr1.distance.CompareTo(tsr2.distance);
            });

            for (int i = 0; i < tcp.numCapsulesInPool; i++)
            {
                tcp.capsules[i].transform.position = Vector3.zero;
            }

            // Step 3 - Move the capsules
            for (int i = 0; i < tsrList.Count && i < tcp.numCapsulesInPool; i++)
            {
                if (resourceSectors[tsrList[i].sector][tsrList[i].listIndex].dead == false)
                {
                    tcp.capsules[i].transform.position = resourceSectors[tsrList[i].sector][tsrList[i].listIndex].position;
                    tcp.capsules[i].GetComponent<CapsuleController>().treeIndex = resourceSectors[tsrList[i].sector][tsrList[i].listIndex].treeIndexInTerrain;
                    tcp.capsules[i].GetComponent<CapsuleController>().sector = tsrList[i].sector;
                    tcp.capsules[i].GetComponent<CapsuleController>().index = tsrList[i].listIndex;
                    //Terrain.activeTerrain.terrainData.treeInstances[resourceSectors[tsrList[i].sector][tsrList[i].listIndex].treeIndexInTerrain].position.y = 10f;
                }
            }

            // Step 4 - there is no Step 4
        }
    }


    public class CapsulePool
    {

        public int numCapsulesInPool;       // initialized by constructor
        private Vector3 hidingPlace;        // initialized by constructor
        private Transform terrainTransform; // initialized by constructor
        public GameObject[] capsules;       // Created by makeCapsules; invoked by constructor

        private void makeCapsules() // Used by constructor
        {
            for (int i = 0; i < numCapsulesInPool; i++)
            {
                GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                capsule.AddComponent<CapsuleController>();
                capsule.layer = 11;

                CapsuleCollider capsuleCollider = capsule.GetComponent<Collider>() as CapsuleCollider;

                capsuleCollider.center = new Vector3(0, 5, 0);
                capsuleCollider.height = 10;

                // DestroyableTree tree = capsule.AddComponent<DestroyableTree>();
                // tree.terrainIndex = -1;                     // Needs to be set when moved  TODO
                capsule.transform.position = hidingPlace;   // Needs to be set when moved  
                                                            // capsule.tag = "Tree";
                capsule.transform.parent = terrainTransform;
                capsule.GetComponent<Renderer>().enabled = false;
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

    void Start()
    {
        terrain = GetComponent<Terrain>();

        numberOfTreeResourceEntitiesNeeded = (terrain.terrainData.treeInstanceCount - 1) / resourceEntityCap + 1;

        //find targets, may need to add and remove targets later
        //   GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //   foreach (GameObject player in players)
        //    {
        //        targetData targetData = new targetData(player, player.transform.position, new TreeCapsulePool(new Vector3(0, -100, 0), terrain.transform, 17));
        //        targets.Add(targetData);
        //    }



        Vector3 corner1 = (Vector3.Scale(new Vector3(0, 0, 0), terrain.terrainData.size)) + transform.position;
        Vector3 corner2 = Vector3.Scale(new Vector3(1, 0, 1), terrain.terrainData.size) + transform.position;
        TerrainSector = new terrainSector(corner1, corner2, 5, 5, terrain.transform.position);


        //generate terrain pool for each target
        // Create the TreeCapsulePool object that manages the pool of capsule colliders

        //foreach (targetData target in targets)
        //{
        //    treeCapsulePools.Add(new TreeCapsulePool(new Vector3(0, -100, 0), terrain.transform, 17));
        // }




        //generate sectors for each resource type

        // backup original terrain trees
        _originalTrees = terrain.terrainData.treeInstances;



        //generate ids, kill dead ones
        //if client, ask server for latest for this terrain
        //server tracks all players, client tracks own player


        //foreach (Transform child in nodeMaster.transform)
        // {
        //     nodes.Add(child.gameObject);
        // }


        var startTime = Time.realtimeSinceStartup;
        int treesToProcess = terrain.terrainData.treeInstanceCount;
        Debug.Log("<color=yellow>Starting to process " + treesToProcess + " Trees for colliders with radius of ...</color>");
        int treesChecked = 0;


        // This loop is MUCH faster if we use the foreach loop instead of 
        // the index.. - i.e  slow  --->  treeInstance = terrain.terrainData.treeInstances[i] 
        foreach (var treeInstance in terrain.terrainData.treeInstances)
            TerrainSector.AddTree(Vector3.Scale(treeInstance.position, terrain.terrainData.size) + transform.position, treesChecked++);

        /*
        for (int i = 0; i < terrain.terrainData.treeInstanceCount; i++)
        {
            // Debug.Log("wew");
            //   terrain.terrainData.treeInstances[i].position.y = 0.1f;

            List<TreeInstance> trees = new List<TreeInstance>(terrain.terrainData.treeInstances);

            TreeInstance PogChamp;
            //trees[evnt.ID] = new TreeInstance();
            PogChamp = trees[i];
            // PogChamp.position.y = 1f;
            trees[i] = PogChamp;
            //trees.RemoveAt(this);
            terrain.terrainData.treeInstances = trees.ToArray();


        }
        //terrain.Flush();
        */

        var secondsTaken = Time.realtimeSinceStartup - startTime;
        Debug.Log("<color=yellow>Terrain trees sectorized. " +
                    "This took " + secondsTaken + " seconds to complete and " +
                    "processed " + treesToProcess + " trees.</color>");



    }


    void Update()
    {
        /*
        if server, then check location for being blocked then respawn if its time 
        
        if target is outside terrain, skip
        check distance vs last generated distance for each target 
        if resource dies, check if collider has resource id

        if moving collider pool, get distances for adjanct 4 sectors and get distance between player and each resource


    */
        if (poolNeedsUpdating == true)
        {

            foreach (targetData target in targets)
            {
                processCapsulesForCurrentPlayerLocation(target);
            }
            poolNeedsUpdating = false;


        }
        foreach (targetData target in targets)
        {



            if (TerrainSector.X_min < target.gameobject.transform.position.x && TerrainSector.X_max > target.gameobject.transform.position.x && TerrainSector.Z_min < target.gameobject.transform.position.z && TerrainSector.Z_max > target.gameobject.transform.position.z)
            {
                //    Debug.Log(gameObject.name);
                processCapsulesForCurrentPlayerLocation(target);
            }
        }


    }

    private void processCapsulesForCurrentPlayerLocation(targetData TargetData)
    {
        //Vector3 playerCurrentPosition = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;

        if (poolNeedsUpdating == true)
        {
            TerrainSector.MoveCapsulesToClosestTrees((TargetData.gameobject.transform.position - transform.position), 10f, TargetData.capsulePool);
            TargetData.position = new Vector3(TargetData.gameobject.transform.position.x, TargetData.gameobject.transform.position.y, TargetData.gameobject.transform.position.z);
        }
        else if (Vector3.Distance(TargetData.position, TargetData.gameobject.transform.position) > 1f)
        {

            TerrainSector.MoveCapsulesToClosestTrees((TargetData.gameobject.transform.position - transform.position), 10f, TargetData.capsulePool);
            TargetData.position = new Vector3(TargetData.gameobject.transform.position.x, TargetData.gameobject.transform.position.y, TargetData.gameobject.transform.position.z);
        }
    }



    void OnApplicationQuit()
    {
        // restore original trees
        terrain.terrainData.treeInstances = _originalTrees;

        //TODO: restore living entities to terrain trees before saving?
    }
}
