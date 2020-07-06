using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Server)]
public class NetworkCallbacksServer : Bolt.GlobalEventListener
{
    private int numeroConexiones = 0;

    
    
        // aquí ponemos lo que pasa localmente cada vez que se carga una nueva escena
    public override void SceneLoadLocalDone(string scene)
    {
        Debug.Log("Ponemos las conexiones a cero");
        numeroConexiones = 0;
        
        if (scene == "Juego Libre")
        {
            //GameObject.FindGameObjectWithTag("CreaCartas").GetComponent<CreaCartas>().CrearCartas();
        }

        if (scene == "Menu")
        {
            //cerramos todas las conexiones posibles al inicio para poder crear nuevas partidas
            BoltNetwork.Shutdown();
        }
    }


    public override void Connected(BoltConnection connection)
    {
        var log = ConexionesEvent.Create();
        log.Mensaje = string.Format("Conectado {0}", connection.RemoteEndPoint);
        log.NumeroConexiones = ++numeroConexiones;
        Debug.LogFormat("Nueva conexión desde {0}, ahora hay {1} conexiones", connection.RemoteEndPoint,
            numeroConexiones);
        log.Send();

        //no queremos que alguien se conecte alguien cuando estamos jugando
        if (FindObjectOfType<GuiIngame>().estado != GuiIngame.Estado.ServidorEsperandoEmpezar)
        {
            //connection.Disconnect();
        }
    }

    public override void Disconnected(BoltConnection connection)
    {
        var log = ConexionesEvent.Create();
        log.Mensaje = string.Format("Desconectado {0}", connection.RemoteEndPoint);
        log.NumeroConexiones = --numeroConexiones;
        Debug.LogFormat("Desconectado desde {0}, ahora hay {1} conexiones", connection.RemoteEndPoint, numeroConexiones);
        log.Send();
    }
    
}