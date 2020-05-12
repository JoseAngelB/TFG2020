using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class RigidbodyPredictionCallbacks : Bolt.GlobalEventListener
{
    public GameObject Sphere;
    public GameObject PlayerPrefab;
    public GameObject EnvironmentPrefab;
    public override void SceneLoadLocalDone(string map)
    {
        if (BoltNetwork.IsServer)
        {
            GameObject GO = GameObject.Instantiate(PlayerPrefab, new Vector3(3, 0, 0), Quaternion.identity);
            //GO.transform.SetParent(StaticTest.mirrorRoot);
            // using (var mod = GO.GetComponent<BoltEntity>().ModifySettings())
            // {

            // mod.persistThroughSceneLoads = true;
            // mod.allowInstantiateOnClient = false;
            // mod.clientPredicted = false;
            // mod.prefabId = BoltPrefabs.Player;
            // mod.updateRate = 1;
            // mod.sceneId = Bolt.UniqueId.None;
            // mod.serializerId = BoltInternal.StateSerializerTypeIds.IPlayerState;
            //   }



            BoltNetwork.Attach(GO);

            GO.GetComponent<BoltEntity>().TakeControl();

            //   BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Player);
            // entity.TakeControl();
            //entity.transform.SetParent(StaticTest.mirrorRoot);

        }
    }


    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        if (BoltNetwork.IsServer)
        {
            GameObject GO = GameObject.Instantiate(PlayerPrefab, new Vector3(-3, 0, 0), Quaternion.identity);
            //GO.transform.SetParent(StaticTest.mirrorRoot);
            BoltNetwork.Attach(GO);

            GO.GetComponent<BoltEntity>().AssignControl(connection);
        }

    }




    // Start is called before the first frame update
    void Start()
    {

        var loadParams = new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics3D);
        StaticTest.localSimScene = SceneManager.LoadScene("AuthRigidbodySceneMirror", loadParams);
        StaticTest.localPhysicsScene = StaticTest.localSimScene.GetPhysicsScene();

        GameObject Env = GameObject.Instantiate(EnvironmentPrefab);



        Invoke("test0", 0.1f);
    }

    void test0()
    {
        GameObject EnvMirror = GameObject.Instantiate(EnvironmentPrefab);
        EnvMirror.transform.SetParent(StaticTest.mirrorRoot);

        GameObject GO = GameObject.Instantiate(Sphere);
        GameObject mirrorGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GO.GetComponent<SphereController>().mirrorSelf = mirrorGO;
        mirrorGO.GetComponent<MeshRenderer>().enabled = false;
        mirrorGO.transform.SetParent(StaticTest.mirrorRoot);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
