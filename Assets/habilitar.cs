using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class habilitar : MonoBehaviour
{
    public GameObject[] objetos;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var objeto in objetos)
            objeto.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
