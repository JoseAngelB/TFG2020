using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bolt;

[BoltGlobalBehaviour]
public class NetworkCallbacks : Bolt.GlobalEventListener
{
    private List<string> listaMensajes = new List<string>();
    private int numeroConexiones;

    //que no salgan todos los que han ocurrido, como mucho los últimos maxMensajes
    private int maxMensajes = 3;

    private float tiempoLogMostrado = 5f;
    private double tiempoTranscurrido = 0f;
    
    public override void SceneLoadLocalDone(string scene)
    {
        // aquí ponemos lo que pasa localmente cada vez que se carga una nueva escena

        //var spawnPosition = new Vector3(Random.Range(-16, 16), 0, Random.Range(-16, 16));
        //BoltNetwork.Instantiate(BoltPrefabs.Carta, spawnPosition, Quaternion.identity);
    }


    public override void OnEvent(ConexionesEvent evnt)
    {
        listaMensajes.Insert(0, evnt.Mensaje);
        numeroConexiones = evnt.NumeroConexiones;
        tiempoTranscurrido = 0f;
    }

    private void Update()
    {
        tiempoTranscurrido += Time.deltaTime;
    }

    private void OnGUI()
    {
        if (tiempoTranscurrido < tiempoLogMostrado)
        {
            int numeroMensajes = Mathf.Min(maxMensajes, listaMensajes.Count);
            GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, Screen.height - 100, 400, 100), GUI.skin.box);

            GUILayout.Label("Número de conectados: " + numeroConexiones);
            for (int i = 0; i < numeroMensajes; i++)
            {
                GUILayout.Label(listaMensajes[i]);
            }
            GUILayout.EndArea();
        }
    }
}