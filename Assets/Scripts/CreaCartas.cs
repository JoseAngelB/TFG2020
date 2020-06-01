using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CreaCartas : Bolt.EntityEventListener<ICreaCartasState>
{

	public GameObject carta;
	[Range(1, 4)] public int maxPalos;
	[Range(1, 12)] public int maxCartasPorPalo;
	public string tipoBaraja;

	private ApilaCartas apilacartas;

	private GameObject[] camaras;

	// Use this for initialization
	void Start () {
		apilacartas = GameObject.FindWithTag("Apilacartas").GetComponent<ApilaCartas>();
		//camaras = GameObject.FindGameObjectsWithTag ("MainCamera");
		//PonerCamaras ();
	}

	
	// Update is called once per frame
	void Update () {
		
	}

	public void CrearCartas () {
		for (int i = 0; i < maxPalos; i++) {
			string palo = "";
			switch (i) {
			case 0:
				palo = "oro";
				break;
			case 1:
				palo = "copa";
				break;
			case 2:
				palo = "espada";
				break;
			case 3:
				palo = "basto";
				break;
			}
			for (int j = 0; j < maxCartasPorPalo; j++) {
				GameObject nuevaCarta = (GameObject) BoltNetwork.Instantiate (carta, new Vector3(j * 4 -10, i + j, i * 10 -10) , Quaternion.identity);
				nuevaCarta.GetComponent<Carta>().PonerPropiedades(palo, (j+1).ToString(), tipoBaraja);
				/*nuevaCarta.GetComponent<Carta> ().palo = palo;
				nuevaCarta.GetComponent<Carta> ().numero = "" + (j+1);
				nuevaCarta.GetComponent<Carta> ().tipoBaraja = tipoBaraja;
				nuevaCarta.GetComponent<Carta>().PonerImagen();*/
			}
		}

		//apilacartas.Apilar();
	}
	

}
