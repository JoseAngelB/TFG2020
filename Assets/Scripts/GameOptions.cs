using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameOptions : MonoBehaviour
{
    public float volumen;

    public GUIStyle estiloGUIBotones;
    //public GUIStyle estiloGUISeleccionCartas;
    public Material reversoCartas;
    public Texture2D estiloBotones;
    public Texture2D[] listaTexturasBotones;
    public int nTexturasBotones;
    public Texture2D[] listaReversoCartas;
    public int nReversoCartas;


    private void Awake()
    {
        listaTexturasBotones = Resources.LoadAll<Texture2D>("Texturas/Botones");
        listaReversoCartas = Resources.LoadAll<Texture2D>("Texturas/CartasDetras");
        
        //asignamos opciones si no existen para cargarlas luego
        volumen = PlayerPrefs.GetFloat("volumen", 0.7f);
        nTexturasBotones = PlayerPrefs.GetInt("texturasBotones", Random.Range(0, listaTexturasBotones.Length));
        nReversoCartas = PlayerPrefs.GetInt("reversoCartas", Random.Range(0, listaReversoCartas.Length));
        
        estiloBotones = listaTexturasBotones[nTexturasBotones];
        estiloGUIBotones.normal.background = estiloBotones;
        estiloGUIBotones.onNormal.background = Resources.Load<Texture2D>("Texturas/selector");
        
        reversoCartas = Resources.LoadAll<Material>("Texturas/CartasDetras/Materials") [nReversoCartas];
        //estiloGUISeleccionCartas.fixedHeight = 15;//Screen.height / 4;
        //estiloGUISeleccionCartas.fixedWidth = 10;//Screen.width / (listaReversoCartas.Length + 1);
        //Debug.Log("volumen es " + volumen + " texturasBotones es " + PlayerPrefs.GetInt("texturasBotones") + " reversoCartas es " + PlayerPrefs.GetInt("reversoCartas"));
    }

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    private void Update()
    {
        
    }

    public void GuardarOpciones()
    {
        PlayerPrefs.SetInt("reversoCartas", nReversoCartas);
        PlayerPrefs.SetInt("texturasBotones", nTexturasBotones);
        PlayerPrefs.SetFloat("volumen", volumen);
        PlayerPrefs.Save();
        
    }
}
