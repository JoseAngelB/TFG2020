using System;
using System.Collections;
using System.Collections.Generic;
using JuegoCartas.Juego;
using UdpKit;
using UnityEngine;

//problemas:
//el servidor no recoge los eventos de añadir jugadores de los clientes
//el cliente no puede modificar la propiedad numeroJugadores porque no es el creador del objeto


public class PlayerManager : Bolt.EntityEventListener<IPlayerManagerState>
{
    public struct JugadorM
    {
        public string alias;
        public Vector3 posicion;
        
        /// <summary>
        /// debe coincidir con el indice del array en el state
        /// </summary>
        public int numeroJugador;

        public JugadorM(string alias, Vector3 posicion, int numeroJugador)
        {
            this.alias = alias;
            this.posicion = posicion;
            this.numeroJugador = numeroJugador;
        }
    }
    
    public List<JugadorM> Jugadores = new List<JugadorM>();

    [SerializeField] private float distanciaJugador;
    [SerializeField] private float tiempoBuscarJugador;

    public int turno;
    public int numeroJugadores;
    public string alias1;
    private Jugador miJugador;



    void Start()
    {
        
    }

    public override void Attached()
    {
        //RecuperarJugadores();
    }

    public override void SimulateOwner()
    {
        //RecuperarJugadores();
    }

    public void NuevoJugador(JugadorM jugadorM)
    {
        Jugadores.Add(jugadorM);
    }

    /*public override void OnEvent(JugadorEvent evnt)
    {
        Jugadores.Add(new JugadorM(evnt.Alias, evnt.Posicion, numeroJugadores++));
        alias1 = evnt.Alias;
    }*/

    /// <summary>
    /// si no es el servidor recupera los jugadores que han sido guardados en el state, ya sean del servidor o de otro cliente
    /// busca el jugador propio para añadirlo
    /// </summary>
    private void RecuperarJugadores()
    {
        numeroJugadores = state.NumeroJugadores;
        Debug.LogFormat("He recuperado {0} jugadores", numeroJugadores);
        for (int i = 0; i < 4; i++)
        {
            Jugadores.Add(new JugadorM(state.AliasJugadores[i], state.PosicionJugadores[i], i));
            Debug.LogFormat("Jugador {0} en la posicion {1}, con el numero {2}", state.AliasJugadores[i], state.PosicionJugadores[i], i);
        }

        //si soy el cliente busco el jugador si he recuperado los otros jugadores y todavía no he recuperado el mío
        if (BoltNetwork.IsClient && numeroJugadores > 0 && miJugador == null)
        {
            BuscarMiJugador();
        }
        if (BoltNetwork.IsServer && miJugador == null)
            BuscarMiJugador();

        GuiIngame guiIngame = FindObjectOfType<GuiIngame>();
        if (guiIngame == null || guiIngame.estado == GuiIngame.Estado.ClienteEsperandoEmpezar ||
            guiIngame.estado == GuiIngame.Estado.ServidorEsperandoEmpezar)
        {
            Debug.LogFormat(
                "GuiIngame == null es {0}, guiIngame.estado == GuiIngame.Estado.ClienteEsperandoEmpezar es {1}, guiIngame.estado == GuiIngame.Estado.ServidorEsperandoEmpezar es {2}",
                guiIngame == null, guiIngame.estado == GuiIngame.Estado.ClienteEsperandoEmpezar,
                guiIngame.estado == GuiIngame.Estado.ServidorEsperandoEmpezar);
            Invoke("RecuperarJugadores", tiempoBuscarJugador);
        }
    }

    private void BuscarMiJugador()
    {
        miJugador = FindObjectOfType<Jugador>();
        if (miJugador == null)
        {
            Invoke("BuscarMiJugador", tiempoBuscarJugador);
            Debug.LogWarning("No he encontrado el jugador, espero para reintentar");
        }
        else
        {
            Debug.LogWarning("Encontrado mi jugador " + miJugador.alias);
            state.AliasJugadores[numeroJugadores] = miJugador.alias;
            state.PosicionJugadores[numeroJugadores] = miJugador.posicion;
            state.NumeroJugadores = numeroJugadores+1;
            Debug.LogWarning("Pongo los jugadores a "+ state.NumeroJugadores);
            numeroJugadores = state.NumeroJugadores;
        }
    }

    public void PasarTurno()
    {
        turno++;
        if (turno >= Jugadores.Count) turno = 0;
    }
}
