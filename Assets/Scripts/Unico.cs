using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unico : MonoBehaviour
{
    private void Awake()
    {
        {
            if (gameObject.tag == "untagged")
            {
                Debug.LogWarning("Hay que ponerle un tag (único) en el objeto " + gameObject.name + " para hacerlo único");
            }
            else
            {
                GameObject[] objetos = GameObject.FindGameObjectsWithTag(this.gameObject.tag);
                if (objetos.Length > 1)
                {
                    Destroy(this.gameObject);
                }

                DontDestroyOnLoad(this.gameObject);
            }
        }
    }

}
