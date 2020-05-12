using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TouchScript.Behaviors;
using TouchScript.Gestures.TransformGestures;


public class Carta : MonoBehaviour
//public class Carta : Bolt.EntityEventListener<ICartaState>
{
    public string palo;
    public string numero;
    public string tipoBaraja;

    private GameObject camara;
    private float rotacionY;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Empieza carta");
        //transform.Rotate(new Vector3(0, 180, 0));
        Vector3 posicionInicial = transform.position;
        camara = GameObject.FindWithTag("MainCamera");
        transform.position = Vector3.zero;
        transform.LookAt(new Vector3(camara.transform.position.x, transform.position.y, camara.transform.position.z));
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
        rotacionY = transform.rotation.y;
        transform.position = posicionInicial;
    }


    void Update()
    {
        
    }

    public void PonerImagen()
    {
        //Debug.Log ("PonerImagen");
        gameObject.GetComponentsInChildren<MeshRenderer>()[1].material =
            Resources.Load("Texturas/Cartas/" + tipoBaraja + "/Materials/" + (palo + numero)) as Material;
        //Debug.Log("poniendo material desde: Texturas/Cartas/" + tipoBaraja + "/Materials/" + (palo + numero));
    }

    void OnTriggerEnter(Collider otro)
    {
        /*Debug.Log ("OnTriggerEnter otro.gameObject.tag es: " + otro.gameObject.tag);
        if (otro.gameObject.tag == "Player") {
            otro.gameObject.GetComponent<Player> ().PonerAutoridad (this.gameObject);
            Debug.Log ("hasAuthority es: " + hasAuthority);
        }*/
    }
}