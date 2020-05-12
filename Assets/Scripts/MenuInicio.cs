using UnityEngine;
using System;
using UdpKit;
using UnityEngine.SceneManagement;
using udpkit.platform.photon;

public class MenuInicio : Bolt.GlobalEventListener
{
    private GameOptions opciones;

    private Estado estado;
    private string juegoActivo;
    private GUIStyle estiloGUIBotones;
    private GUIStyle estiloGUISeleccionCartas;
    private GUIContent contenidoGUI;
    private Texture2D estiloBotones;
    private Texture2D selector;
    private Texture2D logo;
    private AudioSource musica;
    private Rect espacioLogo;
    private Rect espacioOpciones;
    private Rect espacioBotones;
    private Rect espacioVolumen;
    
    enum Estado
    {
        SeleccionarModo,
        SeleccionarJuego,
        SeleccionarRoom,
        EmpezarServidor,
        EmpezarCliente,
        Empezado,
        Opciones,
    }

    private void Start()
    {
        selector = Resources.Load<Texture2D>("Texturas/selector");
        logo = Resources.Load<Texture2D>("Texturas/Logo");
        espacioLogo = new Rect(Screen.width/2 -70,20,140,110);
        espacioOpciones = new Rect(Screen.width *1/3,Screen.height *1/3,Screen.width *1/3,Screen.height *2/3);
        espacioBotones = new Rect(Screen.width *2/10, Screen.height *1/20, Screen.width *6/10, Screen.height *3/10);
        espacioVolumen = new Rect(Screen.width *1/3, Screen.height *1/3, Screen.width *1/3, Screen.height *2/10);

        musica = GetComponent<AudioSource>();
        //cargamos opciones
        opciones = GameObject.FindWithTag("GameOptions").GetComponent<GameOptions>();
        estiloBotones = opciones.estiloBotones;
        estiloGUIBotones = opciones.estiloGUIBotones;
        musica.volume = opciones.volumen;

    }

    private void OnGUI()
    {
        if(!opciones) Debug.LogError("Falta poner las opciones en el script MenuInicio!");
        
        GUI.skin.button = estiloGUIBotones;
        
        //GUI.Box(espacioLogo, logo);
        
        
        switch (estado)
        {
            case Estado.SeleccionarModo: EstadoSeleccionarModo();
                break;
            case Estado.SeleccionarJuego: EstadoSeleccionarJuego();
                break;
            case Estado.SeleccionarRoom: EstadoSeleccionarRoom();
                break;
            case Estado.EmpezarServidor: EstadoEmpezarServidor();
                break;
            case Estado.EmpezarCliente: EstadoEmpezarCliente();
                break;
            case Estado.Opciones: EstadoVerOpciones();
                break;
        }

    }

    private void EstadoVerOpciones()
    {

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
        
        
        GUILayout.BeginArea(espacioVolumen);
        
        GUILayout.BeginVertical();
        GUILayout.Label("Volumen");    //TODO: Ponerlo centrado
        PonerVolumen(GUILayout.HorizontalSlider(opciones.volumen,0,1));
        GUILayout.EndVertical();
        
        GUILayout.EndArea();
        
        
        Rect espacioCartas = new Rect(Screen.width *1/20, Screen.height *2/5, Screen.width *9/10, Screen.height *2/5);    //TODO:mover arriba
        GUILayout.BeginArea(espacioCartas);
        GUILayout.BeginHorizontal();
        //GUI.skin.button = estiloGUISeleccionCartas;
        GUI.skin.button.normal.background = null;
        
        opciones.nReversoCartas = GUILayout.SelectionGrid(opciones.nReversoCartas, opciones.listaReversoCartas,
            opciones.listaReversoCartas.Length, GUILayout.MaxWidth(Screen.width * 9/10),GUILayout.MaxHeight(Screen.height * 2/5));
        
        
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        
        
        GUI.skin.button.normal.background = opciones.estiloBotones;
        
        Rect espacioVolver = new Rect(Screen.width *1/3, Screen.height *8/10, Screen.width *1/3, Screen.height *2/10);    //TODO:mover arriba
        GUILayout.BeginArea(espacioVolver);
        GUI.skin.button = estiloGUIBotones;
        if (CrearBoton("Inicio"))
        {
            //guardamos las opciones antes de volver
            opciones.estiloGUIBotones.normal.background = opciones.listaTexturasBotones[opciones.nTexturasBotones];
            opciones.estiloBotones = opciones.listaTexturasBotones[opciones.nTexturasBotones];
            opciones.GuardarOpciones();
            estado = Estado.SeleccionarModo;
            //BoltLauncher.Shutdown();
        }
        GUILayout.EndArea();
    }

    private void EstadoSeleccionarModo()
    {
        GUI.Box(espacioLogo, logo);


        GUILayout.BeginArea(espacioOpciones);
        if (CrearBoton("Servidor"))
        {
            estado = Estado.SeleccionarJuego;
        }

        if (CrearBoton("Cliente"))
        {
            estado = Estado.EmpezarCliente;
        }
        
        if (CrearBoton("Opciones"))
        {
            estado = Estado.Opciones;
        }
        GUILayout.EndArea();
    }

    private void EstadoSeleccionarJuego()
    {
        GUI.Box(espacioLogo, logo);


        GUILayout.BeginArea(espacioOpciones);
        GUILayout.BeginVertical();

        foreach (string juego in BoltScenes.AllScenes)    //de todas las escenas que existen
        {
            if (SceneManager.GetActiveScene().name != juego)    //las buscamos todas excepto la actual
            {
                if (CrearBoton(juego))
                {
                    juegoActivo = juego;
                    estado = Estado.EmpezarServidor;
                }
            }
        }
        
        if (CrearBoton("Opciones"))
        {
            estado = Estado.Opciones;
        }
        if (CrearBoton("Volver al inicio"))
        {
            estado = Estado.SeleccionarModo;
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    
    private void EstadoSeleccionarRoom()
    {
        GUI.Box(espacioLogo, logo);


        GUILayout.BeginArea(espacioOpciones);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Buscando servidores...");
        GUILayout.EndHorizontal();
        
        GUILayout.BeginVertical();
        foreach (var sesion in BoltNetwork.SessionList)
        {
            var photonSession = sesion.Value as PhotonSession;

            if (photonSession.Source == UdpSessionSource.Photon)
            {
                var nombrePartida = photonSession.HostName;
                var textoBotonConectar = string.Format("Conectar a " + nombrePartida);

                if (CrearBoton(textoBotonConectar))
                {
                    BoltNetwork.Connect(photonSession);
                    estado = Estado.Empezado;
                }
            }
        }
        if (CrearBoton("Volver al inicio"))
        {
            BoltLauncher.Shutdown();
            estado = Estado.SeleccionarModo;
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void EstadoEmpezarCliente()
    {
        BoltLauncher.StartClient();
        estado = Estado.SeleccionarRoom;
    }

    private void EstadoEmpezarServidor()
    {
        BoltLauncher.StartServer();
        estado = Estado.Empezado;
    }


    private bool CrearBoton(string texto)
    {
        return GUILayout.Button(texto);//, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(30), GUILayout.MaxWidth(300));
    }
    private bool CrearBoton(Texture2D textura)
    {
        return GUILayout.Button(textura, GUILayout.MaxWidth(Screen.width/(opciones.listaReversoCartas.Length +1)),GUILayout.MaxHeight(Screen.height * 2/5));
    }
    
    private void PonerVolumen(float horizontalSlider)
    {
        opciones.volumen = horizontalSlider;
        musica.volume = horizontalSlider;
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            var id = Guid.NewGuid().ToString().Split('-')[0];
            var nombrePartida = string.Format("{0} - {1}", id, juegoActivo);

            BoltNetwork.SetServerInfo(nombrePartida, null);
            BoltNetwork.LoadScene(juegoActivo);
        }
    }
    
    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        Debug.LogFormat("Nueva lista de sesiones. {0} sesiones totales", sessionList.Count);
    }
    
}
