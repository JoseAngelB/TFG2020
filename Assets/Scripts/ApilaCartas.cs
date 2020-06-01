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
	public GameObject carta;
	
	[SerializeField] private float tiempoEsperaGirarCartas;
	[SerializeField] private float tiempoEsperaTrasladarCartas;
	
	//la distancia que hay entre el punto donde se tienen que apilar y donde tienen que parar los apiladores
	private float sobrepasoX;
	private float sobrepasoZ;

	// Use this for initialization
	void Start ()
	{
		
		Collider[] collidersApiladores = gameObject.GetComponentsInChildren<Collider> ();
		apiladores = new GameObject[collidersApiladores.Length];
		posicionesApiladores = new Vector3[collidersApiladores.Length];
		for (int i = 0; i < apiladores.Length; i++) {
			apiladores [i] = collidersApiladores [i].gameObject;
			posicionesApiladores [i] = apiladores [i].transform.position;
		}
		sobrepasoX = (carta.transform.localScale.x / 2) + 50;
		sobrepasoZ = (carta.transform.localScale.z / 2) + 50;
		primeraApilada = true;
		//Apilar ();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Apilar ()
	{
		//Usamos las físicas para apilar
		/*		No las usamos
		 ApiladoresActivados (true);

		StartCoroutine (MoverApilador (apiladores [0], new Vector3 (posicionApiladas.x + sobrepasoX, posicionesApiladores[0].y, posicionesApiladores[0].z)));
		StartCoroutine (MoverApilador (apiladores [1], new Vector3 (posicionesApiladores[1].x, posicionesApiladores[1].y, posicionApiladas.z + sobrepasoZ)));
		StartCoroutine (MoverApilador (apiladores [2], new Vector3 (posicionApiladas.x - sobrepasoX, posicionesApiladores[2].y, posicionesApiladores[2].z)));
		StartCoroutine (MoverApilador (apiladores [3], new Vector3 (posicionesApiladores[3].x, posicionesApiladores[3].y, posicionApiladas.z - sobrepasoZ)));


		if (primeraApilada) {
			//StartCoroutine (EsperarParaDesactivar ());
		}
		*/
		
		Invoke("GirarCartas", tiempoEsperaGirarCartas);
		Invoke("TransladarCartas", tiempoEsperaTrasladarCartas);
		Invoke("Barajar", tiempoEsperaTrasladarCartas + carta.GetComponent<MovimientoCarta>().tiempoTranslacion + 1);
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

	public void Empezar()
	{
		
	}

	void GirarCartas()
	{
		GameObject[] cartas = GameObject.FindGameObjectsWithTag("Carta");
		foreach (var carta in cartas)
		{
			carta.GetComponent<Rigidbody>().isKinematic = true;
			carta.GetComponent<MovimientoCarta>().Voltear(true);
		}
	}

	void TransladarCartas()
	{
		GameObject[] cartas = GameObject.FindGameObjectsWithTag("Carta");
		for (int i = 0; i < cartas.Length; i++)
		{
			//cartas[i].transform.position = posicionApiladas + (Vector3.up * (i * carta.localScale.y));
			cartas[i].GetComponent<MovimientoCarta>().TrasladarCarta(posicionApiladas + (Vector3.up * ((i+1) * carta.transform.localScale.y)));
		}
		
	}

	void Barajar()
	{
		GameObject[] cartas = GameObject.FindGameObjectsWithTag("Carta");
		for (int i = 0; i < cartas.Length; i++)
		{
			IntercambiarCartas(cartas[i], cartas[Random.Range(0,cartas.Length)]);
		}
	}

	void IntercambiarCartas(GameObject carta1, GameObject carta2)
	{
		Vector3 posicionTemp = carta1.transform.position;
		carta1.transform.position = carta2.transform.position;
		carta2.transform.position = posicionTemp;
		
	}
}
