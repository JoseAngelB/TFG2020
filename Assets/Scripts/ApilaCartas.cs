using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApilaCartas : MonoBehaviour {

	public Vector3 posicionApiladas;
	public float tiempoTranslacion;

	public GameObject[] apiladores;
	private Vector3[] posicionesApiladores;
	public bool primeraApilada;
	public GameObject[] transicionables;

	[SerializeField] private Transform carta;
	
	//la distancia que hay entre el punto donde se tienen que apilar y donde tienen que parar los apiladores
	private float sobrepasoX;
	private float sobrepasoZ;

	// Use this for initialization
	void Start ()
	{
		sobrepasoX = (carta.localScale.x / 2) + 51;
		sobrepasoZ = (carta.localScale.z / 2) + 51;
		
		Collider[] collidersApiladores = gameObject.GetComponentsInChildren<Collider> ();
		apiladores = new GameObject[collidersApiladores.Length];
		posicionesApiladores = new Vector3[collidersApiladores.Length];
		for (int i = 0; i < apiladores.Length; i++) {
			apiladores [i] = collidersApiladores [i].gameObject;
			posicionesApiladores [i] = apiladores [i].transform.position;
		}
		primeraApilada = true;
		//Apilar ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Apilar ()
	{
		//GirarCartas();
		ApiladoresActivados (true);

		StartCoroutine (MoverApilador (apiladores [0], new Vector3 (posicionApiladas.x + sobrepasoX, posicionesApiladores[0].y, posicionesApiladores[0].z)));
		StartCoroutine (MoverApilador (apiladores [1], new Vector3 (posicionesApiladores[1].x, posicionesApiladores[1].y, posicionApiladas.z + sobrepasoZ)));
		StartCoroutine (MoverApilador (apiladores [2], new Vector3 (posicionApiladas.x - sobrepasoX, posicionesApiladores[2].y, posicionesApiladores[2].z)));
		StartCoroutine (MoverApilador (apiladores [3], new Vector3 (posicionesApiladores[3].x, posicionesApiladores[3].y, posicionApiladas.z - sobrepasoZ)));


		if (primeraApilada) {
			//StartCoroutine (EsperarParaDesactivar ());
		}
	}

	private IEnumerator MoverApilador (GameObject apilador, Vector3 posicion) {
		
		float TiempoInicio = Time.time;
		float tiempoRelativo = Time.time - TiempoInicio;
		Vector3 posicionInicio = apilador.transform.position;

		while (tiempoRelativo <= tiempoTranslacion) {
			tiempoRelativo = Time.time - TiempoInicio;
			apilador.transform.position = Vector3.Lerp (posicionInicio, posicion, tiempoRelativo/tiempoTranslacion);
			yield return 1;
		}
	}

	private IEnumerator EsperarParaDesactivar () {
		yield return new WaitForSeconds ((float) (tiempoTranslacion * 2));

		ApiladoresActivados (false);
		ActivarTransiciones ();
	}

	private void ApiladoresActivados (bool activado) {
		for (int i = 0; i < apiladores.Length; i++) {
			apiladores [i].SetActive (activado);
		}
	}

	private void ActivarTransiciones () {
		GameObject [] players = GameObject.FindGameObjectsWithTag ("Player");

		//Debug.Log ("transicionables.Length es: " + transicionables.Length);
		//Debug.Log ("numeroPlayers es: " + numeroPlayers);
		for (int i = 0; i < players.Length -1; i++) {
			//Debug.Log ("transicionables [i].name es: " + transicionables [i].name);
			//Debug.Log ("transicionables [i] está activo: " + transicionables [i].activeSelf);
			transicionables [i].SetActive (true);
			//Debug.Log ("transicionables [i] está activo: " + transicionables [i].activeSelf);

			//players [i].GetComponent<Player> ().SetMiTransicionable (transicionables);
		}
	}

	void GirarCartas()
	{
		GameObject[] cartas = GameObject.FindGameObjectsWithTag("Carta");
		foreach (var carta in cartas)
		{
			carta.GetComponent<MovimientoCarta>().Voltear(true);
		}
	}
}
