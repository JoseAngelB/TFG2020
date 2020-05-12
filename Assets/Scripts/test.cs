using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.color = Color.blue;
        GameObject.Find("BoltBehaviours").GetComponent<BoltConsole>().visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
