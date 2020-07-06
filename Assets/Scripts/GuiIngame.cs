using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Bolt;
using JuegoCartas.Juego;
using UnityEngine.SceneManagement;
using Event = Bolt.Event;

public class GuiIngame : Bolt.EntityEventListener<IGuiIngameState>
{
    //private enum barajas {espanyola1 = 0, francesa1 = 1, francesa2 = 2, francesa3 = 3, francesa4 = 4};


    private GameOptions opciones;
    private AudioSource musica;
    private CreaCartas creaCartas;
    public Estado estado;

    private Rect espacioBotonOpciones;
    private Rect espacioBotonCamara;
    private Rect espacioBotones;
    private Rect espacioVolumen;
    private Rect espacioElegirPalosCartas;
    private Rect pantallaEntera;
    private Rect espacioFinal;
    private Rect espacioCartas;

    private Texture2D botonOpciones;
    private Texture2D botonCamara;
    private Texture2D botonCerrar;
    private Texture2D[] listaBarajas;

    private GUIStyle estiloGUIBotones;

    private bool pulsadoSiSalir;
    private bool pulsadoNoSalir;

    private int palosSeleccionados = 4;
    private int cartasPorPaloSeleccionada = 12;
    private int maxCartasPorPalo = 12;
    private string stringBaraja = "espanyola1";
    private int indiceBaraja = 1;
    private string[] barajas = new string[] {"espanyola1", "francesa1", "francesa2", "francesa3"};//, "francesa4"};



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
        listaBarajas = new Texture2D[]
        {
            Resources.Load<Texture2D>("Texturas/Cartas/espanyola1/oro1"), Resources.Load<Texture2D>("Texturas/Cartas/francesa1/heart13"), 
            Resources.Load<Texture2D>("Texturas/Cartas/francesa2/heart13"), Resources.Load<Texture2D>("Texturas/Cartas/francesa3/heart13"), 
            //Resources.Load<Texture2D>("Texturas/Cartas/francesa4/heart13"), 
        };
        
        opciones = GameObject.FindWithTag("GameOptions").GetComponent<GameOptions>();
        
        estiloGUIBotones = opciones.estiloGUIBotones;
        
        musica = GetComponent<AudioSource>();
        musica.volume = opciones.volumen;
        musica.Play();
        
        creaCartas = GameObject.FindWithTag("CreaCartas").GetComponent<CreaCartas>();
        
        espacioBotonOpciones = new Rect(Screen.width * 9/10, 0, Screen.width * 1/10, Screen.width * 1/10);
        espacioBotonCamara = new Rect(Screen.width * 9/20, 0, Screen.width * 1/10, Screen.width * 1/10);
        espacioBotones = new Rect(Screen.width *2/10, Screen.height *1/20, Screen.width *6/10, Screen.height *3/10);
        espacioVolumen = new Rect(Screen.width *1/3, Screen.height *1/3, Screen.width *1/3, Screen.height *2/10);
        espacioElegirPalosCartas = new Rect(Screen.width *1/3, Screen.height *2/10, Screen.width *1/3, Screen.height *2/10);
        botonOpciones = Resources.Load<Texture2D>("Texturas/iconoEngranaje");
        botonCamara = Resources.Load<Texture2D>("Texturas/iconoCamara");
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

        if (GUI.Button(espacioBotonCamara, botonCamara))
        {
            GetComponent<CamaraJugador>().MoverCamara();
        }
        
        opciones.estiloGUIBotones.normal.background = opciones.estiloBotones;    //volvemos a poner el fondo
    }

    private void EstadoServidorEsperandoEmpezar()
    {
        
        //elegir número de cartas y palos
        GUILayout.BeginArea(espacioElegirPalosCartas);
        
        GUILayout.BeginVertical();
        GUILayout.Label("Número de palos: " + palosSeleccionados);
        PonerPalos(GUILayout.HorizontalSlider(palosSeleccionados, 1, 4));
        GUILayout.Label("Número de cartas por palo: " + cartasPorPaloSeleccionada);
        PonerCartasPorPalo(GUILayout.HorizontalSlider(cartasPorPaloSeleccionada, 1, maxCartasPorPalo));
        GUILayout.EndVertical();
        
        GUILayout.EndArea();

        
        //selección tipo baraja
        GUILayout.BeginArea(espacioCartas);
        GUILayout.BeginHorizontal();
        GUI.skin.button.normal.background = null;
        
        //opciones.nReversoCartas = GUILayout.SelectionGrid(opciones.nReversoCartas, opciones.listaReversoCartas,
           //opciones.listaReversoCartas.Length, GUILayout.MaxWidth(Screen.width * 9/10),GUILayout.MaxHeight(Screen.height * 2/5));
        
        //GUI.skin.button.normal.background = opciones.estiloBotones;
        indiceBaraja = GUILayout.SelectionGrid(indiceBaraja, listaBarajas, listaBarajas.Length,
            GUILayout.MaxWidth(Screen.width * 9/10),GUILayout.MaxHeight(Screen.height * 2/5));
        creaCartas.tipoBaraja = barajas[indiceBaraja];
        
        //si es la baraja española hay 12 cartas max por palo, sino, son 13
        if (indiceBaraja == 0)    
        {
            maxCartasPorPalo = 12;
            if (cartasPorPaloSeleccionada == 13) cartasPorPaloSeleccionada = 12;
        }
        else
        {
            maxCartasPorPalo = 13;
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        GUI.skin.button.normal.background = opciones.estiloBotones;
        
        //boton empezar partida
        GUILayout.BeginArea(espacioFinal);
        GUI.skin.button = estiloGUIBotones;
        if (CrearBoton("Empezar partida"))
        {
            GameObject.FindGameObjectWithTag("CreaCartas").GetComponent<CreaCartas>().CrearCartas();
            GameObject.FindGameObjectWithTag("Apilacartas").GetComponent<ApilaCartas>().Apilar();
            estado = Estado.Jugando;
            var eventoEmpezar = GuiIngameEvent.Create();
            eventoEmpezar.JuegoEmpezado = true;
            eventoEmpezar.Send();
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

        GUILayout.Label("Estilo botones");
        opciones.nTexturasBotones = GUILayout.SelectionGrid(opciones.nTexturasBotones, opciones.listaTexturasBotones,
            opciones.listaTexturasBotones.Length/2, GUILayout.MaxHeight(Screen.height * 2/10));
        
        GUILayout.EndArea();
        opciones.estiloGUIBotones.padding.top = 25;
        opciones.estiloGUIBotones.padding.bottom = 25;
        opciones.estiloGUIBotones.normal.background = opciones.estiloBotones;
        
        
        //volumen
        GUILayout.BeginArea(espacioVolumen);
        
        GUILayout.BeginVertical();
        GUILayout.Label("Volumen");
        PonerVolumen(GUILayout.HorizontalSlider(opciones.volumen,0,1));
        GUILayout.EndVertical();
        
        GUILayout.EndArea();
        
        
        //selección reverso cartas
        GUILayout.BeginArea(espacioCartas);
        GUILayout.BeginHorizontal();
        GUI.skin.button.normal.background = null;
        
        opciones.ponerMaterial(GUILayout.SelectionGrid(opciones.nReversoCartas, opciones.listaReversoCartas,
            opciones.listaReversoCartas.Length, GUILayout.MaxWidth(Screen.width * 9/10),GUILayout.MaxHeight(Screen.height * 2/5)));
        foreach (GameObject objetoCarta in GameObject.FindGameObjectsWithTag("Carta"))
        {
            objetoCarta.GetComponent<Carta>().PonerReverso();
        }

        
        
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
        if (BoltNetwork.IsConnected) BoltNetwork.Shutdown();
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

    private void PonerPalos(float horizontalSlider)
    {
        palosSeleccionados = (int) Math.Round(horizontalSlider);
        creaCartas.maxPalos = palosSeleccionados;
    }
    
    private void PonerCartasPorPalo(float horizontalSlider)
    {
        cartasPorPaloSeleccionada = (int) Math.Round(horizontalSlider);
        creaCartas.maxCartasPorPalo = cartasPorPaloSeleccionada;
    }


    public override void OnEvent(GuiIngameEvent evnt)
    {
        Debug.Log("Evento guiIngame con estado " + evnt.JuegoEmpezado);
        if (evnt.JuegoEmpezado)
            estado = Estado.Jugando;
    }
}
