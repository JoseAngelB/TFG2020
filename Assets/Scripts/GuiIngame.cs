using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Bolt;
using UnityEngine.SceneManagement;

public class GuiIngame : MonoBehaviour
{

    private GameOptions opciones;
    private AudioSource musica;
    private Estado estado;

    private Rect espacioBotonOpciones;
    private Rect espacioBotones;
    private Rect espacioVolumen;
    private Rect pantallaEntera;
    private Rect espacioFinal;
    private Rect espacioCartas;

    private Texture2D botonOpciones;
    private Texture2D botonCerrar;

    private GUIStyle estiloGUIBotones;
    
    private bool pulsadoSiSalir;
    private bool pulsadoNoSalir;


    public enum Estado
    {
        ServidorEsperandoEmpezar,
        ClienteEsperandoEmpezar,
        Jugando,
        OpcionesJuego,
        OpcionesTurno,
        Confirmarsalir
    }


    void Start()
    {
        opciones = GameObject.FindWithTag("GameOptions").GetComponent<GameOptions>();
        
        estiloGUIBotones = opciones.estiloGUIBotones;
        
        musica = GetComponent<AudioSource>();
        musica.volume = opciones.volumen;
        musica.Play();
        
        espacioBotonOpciones = new Rect(Screen.width * 9/10, 0, Screen.width * 1/10, Screen.width * 1/10);
        espacioBotones = new Rect(Screen.width *2/10, Screen.height *1/20, Screen.width *6/10, Screen.height *3/10);
        espacioVolumen = new Rect(Screen.width *1/3, Screen.height *1/3, Screen.width *1/3, Screen.height *2/10);
        botonOpciones = Resources.Load<Texture2D>("Texturas/iconoEngranaje");
        botonCerrar = Resources.Load<Texture2D>("Texturas/iconoCerrar");

        espacioFinal = new Rect(Screen.width *1/3, Screen.height *8/10, Screen.width *1/3, Screen.height *2/10);
        espacioCartas = new Rect(Screen.width *1/20, Screen.height *2/5, Screen.width *9/10, Screen.height *2/5);
        
        pantallaEntera = new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height));

        //ponemos el estado que corresponde dependiendo si somos el servidor o no
        estado = BoltNetwork.IsServer ? Estado.ServidorEsperandoEmpezar : Estado.ClienteEsperandoEmpezar;
    }

    
    public void PonerEstado(Estado estadoPuesto)
    {
        estado = estadoPuesto;
    }

    private void OnGUI()
    {
        //si no estamos jugando ponemos un fondo para no interactuar con el propio juego mientras está el menu
        if (estado != Estado.Jugando)
        {
            GUI.Box(pantallaEntera, "");
        }

        switch (estado)
        {
            case Estado.Jugando: EstadoJugando();
                break;
            case Estado.ServidorEsperandoEmpezar: EstadoServidorEsperandoEmpezar();
                break;
            case Estado.ClienteEsperandoEmpezar: EstadoClienteEsperandoEmpezar();
                break;
            case Estado.OpcionesJuego: EstadoOpcionesJuego();
                break;
            case Estado.OpcionesTurno: EstadoOpcionesTurno();
                break;
            case Estado.Confirmarsalir: EstadoConfirmarsalir();
                break;
        }
    }

    private void EstadoJugando()
    {
        opciones.estiloGUIBotones.normal.background = null;    //quitamos el dibujo del botón por defecto porque solo son las texturas lo que queremos mostrar
        if (GUI.Button(espacioBotonOpciones, botonOpciones))
        {
            estado = Estado.OpcionesJuego;
        }
        opciones.estiloGUIBotones.normal.background = opciones.estiloBotones;    //volvemos a poner el fondo
    }

    private void EstadoServidorEsperandoEmpezar()
    {
        GUILayout.BeginArea(espacioFinal);
        GUI.skin.button = estiloGUIBotones;
        if (CrearBoton("Empezar partida"))
        {
            GameObject.FindGameObjectWithTag("CreaCartas").GetComponent<CreaCartas>().CrearCartas();
            estado = Estado.Jugando;
        }
        GUILayout.EndArea();
    }
    
    private void EstadoClienteEsperandoEmpezar()
    {
        Debug.Log("EstadoClienteEsperandoEmpezar() no implementado");
        estado = Estado.Jugando;
    }
    
    private void EstadoOpcionesJuego()
    {
        //fondo de los botones del menu
        opciones.estiloGUIBotones.padding.top = 0;
        opciones.estiloGUIBotones.padding.bottom = 0;
        opciones.estiloGUIBotones.normal.background = null;    //quitamos el dibujo del botón por defecto porque solo son las texturas lo que queremos mostrar
        GUILayout.BeginArea(espacioBotones);

        GUILayout.Label("Estilo botones");    //TODO: Ponerlo centrado
        opciones.nTexturasBotones = GUILayout.SelectionGrid(opciones.nTexturasBotones, opciones.listaTexturasBotones,
            opciones.listaTexturasBotones.Length/2, GUILayout.MaxHeight(Screen.height * 2/10));
        
        GUILayout.EndArea();
        opciones.estiloGUIBotones.padding.top = 25;
        opciones.estiloGUIBotones.padding.bottom = 25;
        opciones.estiloGUIBotones.normal.background = opciones.estiloBotones;
        
        
        //volumen
        GUILayout.BeginArea(espacioVolumen);
        
        GUILayout.BeginVertical();
        GUILayout.Label("Volumen");    //TODO: Ponerlo centrado
        PonerVolumen(GUILayout.HorizontalSlider(opciones.volumen,0,1));
        GUILayout.EndVertical();
        
        GUILayout.EndArea();
        
        
        //selección reverso cartas
        GUILayout.BeginArea(espacioCartas);
        GUILayout.BeginHorizontal();
        GUI.skin.button.normal.background = null;
        
        opciones.nReversoCartas = GUILayout.SelectionGrid(opciones.nReversoCartas, opciones.listaReversoCartas,
            opciones.listaReversoCartas.Length, GUILayout.MaxWidth(Screen.width * 9/10),GUILayout.MaxHeight(Screen.height * 2/5));
        
        GUI.skin.button.normal.background = opciones.estiloBotones;
        
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        
        
        //botón para salir
        GUILayout.BeginArea(espacioFinal);
        GUI.skin.button = estiloGUIBotones;
        if (CrearBoton("Salir del juego"))
        {
            //guardamos las opciones antes de volver
            opciones.estiloGUIBotones.normal.background = opciones.listaTexturasBotones[opciones.nTexturasBotones];
            opciones.estiloBotones = opciones.listaTexturasBotones[opciones.nTexturasBotones];
            opciones.GuardarOpciones();
            estado = Estado.Confirmarsalir;
            //BoltLauncher.Shutdown();
        }
        GUILayout.EndArea();
        
        //botón para cerrar las opciones
        opciones.estiloGUIBotones.normal.background = null;    //quitamos el dibujo del botón por defecto porque solo son las texturas lo que queremos mostrar
        if (GUI.Button(espacioBotonOpciones, botonCerrar))
        {
            estado = Estado.Jugando;
        }
        opciones.estiloGUIBotones.normal.background = opciones.estiloBotones;    //volvemos a poner el fondo
    }
    
    private void EstadoOpcionesTurno()
    {
        Debug.Log("EstadoOpcionesTurno() no implementado");
        estado = Estado.Jugando;
    }
    
    private void EstadoConfirmarsalir()
    {
        Debug.Log("EstadoConfirmarsalir() no implementado, salimos directamente");
        if (BoltNetwork.IsServer)
        {
            BoltNetwork.LoadScene("Menu");
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
        BoltNetwork.Shutdown();
    }
    
    
    
    private bool CrearBoton(string texto)
    {
        return GUILayout.Button(texto);//, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(30), GUILayout.MaxWidth(300));
    }
    private bool CrearBoton(Texture2D textura)
    {
        return GUILayout.Button(textura);//, GUILayout.MaxWidth(Screen.width/(opciones.listaReversoCartas.Length +1)),GUILayout.MaxHeight(Screen.height * 2/5));
    }
    
    private void PonerVolumen(float horizontalSlider)
    {
        opciones.volumen = horizontalSlider;
        musica.volume = horizontalSlider;
    }
}
