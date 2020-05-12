using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenObjectPersist : Bolt.GlobalEventListener
{

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void SceneLoadLocalDone(string map)
    {
        GameObject.Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {


    }
}
