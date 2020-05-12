using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ImageController : Bolt.GlobalEventListener
{
    bool loadingStart;
    bool loadingDone;
    float fade = 0;


    public void SceneLoadStart()
    {
        loadingStart = true;
    }

    public override void SceneLoadLocalBegin(string map)
    {
        // loadingStart = true;
    }

    public override void SceneLoadLocalDone(string map)
    {
        fade = 1;
        loadingDone = true;
    }

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(transform.root.gameObject);


    }

    // Update is called once per frame
    void Update()
    {
        if (loadingDone == true)
        {
            Color a = GetComponent<Image>().color;
            fade -= 0.02f;
            a.a = fade;
            GetComponent<Image>().color = a;
            Color b = transform.GetChild(0).GetComponent<Text>().color;
            b.a = fade;
            transform.GetChild(0).GetComponent<Text>().color = b;
        }

        else if (loadingStart == true)
        {

            Color a = GetComponent<Image>().color;
            fade += 0.08f;
            a.a = fade;
            GetComponent<Image>().color = a;

            Color b = transform.GetChild(0).GetComponent<Text>().color;
            b.a = fade;
            transform.GetChild(0).GetComponent<Text>().color = b;
        }
    }
}
