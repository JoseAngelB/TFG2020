using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JuegoCartas.Juego
{
    public class Jugador : BoltSingletonPrefab<Jugador>
    {
        public string alias;
        public Vector3 posicion;
        private PlayerManager playerManager;
        

        // Start is called before the first frame update
        void Start()
        {
            alias = Path.GetRandomFileName().Replace(".", "");
            
            Debug.LogFormat("Creado player con el alias {0}", alias);

            var eventoJugador = JugadorEvent.Create();
            eventoJugador.Alias = alias;
            
            eventoJugador.Send();

            playerManager = GameObject.FindWithTag("PlayerManager").GetComponent<PlayerManager>();
        }


        // Update is called once per frame
        void Update()
        {

        }

        

        public void PonerPosicion(Vector3 posicion)
        {
            transform.position = posicion;
            transform.LookAt(Vector3.zero);
        }
    }
}