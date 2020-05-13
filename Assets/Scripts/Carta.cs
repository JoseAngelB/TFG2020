using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TouchScript.Behaviors;
using TouchScript.Gestures.TransformGestures;


//public class Carta : MonoBehaviour
public class Carta : Bolt.EntityEventListener<ICartaState>
{
    public string palo;
    public string numero;
    public string tipoBaraja;

    public float tiempoRecuperarPropiedades;

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
        rotacionY = transform.rotation.y;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
        transform.position = posicionInicial;
        
        RecuperarPropiedades();
    }
    

    void RecuperarPropiedades()
    {
        //Debug.Log("Recupero propiedades");
        palo = state.CartaPalo;
        numero = state.CartaNumero;
        tipoBaraja = state.CartaTipoBaraja;
        if (palo != "" && numero != "" && tipoBaraja != "")
        {
            PonerImagen();
        }
        else
        {
            Invoke("RecuperarPropiedades", tiempoRecuperarPropiedades);
        }
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

    public void PonerPropiedades(string palo, string numero, string tipoBaraja)
    {
        this.palo = palo;
        this.numero = numero;
        this.tipoBaraja = tipoBaraja;
        PonerImagen();


        /*//creamos el evento de red para sincronizarlo        //no sirve porque puede que no exista todavía
        var eventoSinc = PropiedadesCartaEvent.Create(entity);
        eventoSinc.Numero = numero;
        eventoSinc.Palo = palo;
        eventoSinc.TipoBaraja = tipoBaraja;
        Debug.LogFormat("Envío el evento {0}", eventoSinc.ToString());
        eventoSinc.Send();*/
        
        //lo ponemos en el objeto a sincronizar
        state.CartaPalo = palo;
        state.CartaNumero = numero;
        state.CartaTipoBaraja = tipoBaraja;
    }

    public override void OnEvent(PropiedadesCartaEvent evento)
    {
        Debug.LogFormat("Recibo el evento");
        palo = evento.Palo;
        numero = evento.Numero;
        tipoBaraja = evento.TipoBaraja;
        PonerImagen();
    }
}