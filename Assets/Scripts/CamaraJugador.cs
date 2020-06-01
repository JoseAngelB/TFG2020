using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JuegoCartas.Juego
{

    public class CamaraJugador : BoltSingletonPrefab<CamaraJugador>
    {
        [SerializeField] private float altura;
        [SerializeField] private float distanciaCentroInicial;
        [SerializeField] private float distanciaCentroJugador;
        [SerializeField] private float tiempoMover;
        private float tiempoTranscurrido;

        private bool viendoMesa;
        private bool moviendo;
        
        private Vector3 posicionInicialMesa;
        private Vector3 posicionInicialJugador;
        private Quaternion rotacionInicial;


        private void Awake()
        {
            transform.position = new Vector3(0,altura,0);
            if (BoltNetwork.Connections.Count() == 0)
            {
                transform.rotation = Quaternion.Euler(Vector3.zero);
            } else if (BoltNetwork.Connections.Count() >= 1) //TODO: poner solo para ==1 y poner las rotaciones en los demás jugadores
            {
                transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
            }
            
            transform.Translate(new Vector3(0,0,distanciaCentroInicial));
            posicionInicialMesa = transform.position;
            transform.Translate(new Vector3(0,0,distanciaCentroJugador - distanciaCentroInicial));
            posicionInicialJugador = transform.position;
            transform.position = posicionInicialMesa;
            transform.LookAt(Vector3.zero);

            
            
            posicionInicialJugador = new Vector3(0,altura,distanciaCentroJugador);
            
            transform.position = posicionInicialMesa;
            transform.LookAt(Vector3.zero);

            viendoMesa = true;
            moviendo = false;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            //if (!moviendo) MoverCamara();
            if (moviendo)
            {
                tiempoTranscurrido += Time.deltaTime;
            }
        }

        public void MoverCamara()
        {
            if (!moviendo)
            {
                moviendo = true;
                tiempoTranscurrido = 0;
                StartCoroutine(Moviendo());
            }
        }

        IEnumerator Moviendo()
        {
            while (tiempoTranscurrido < tiempoMover)
            {
                transform.position = Vector3.Lerp(transform.position, viendoMesa ? posicionInicialJugador : posicionInicialMesa, tiempoTranscurrido / tiempoMover);
                yield return 1;
            }
            tiempoTranscurrido = 0;
            moviendo = false;
            viendoMesa = !viendoMesa;
        }
        
    }
}