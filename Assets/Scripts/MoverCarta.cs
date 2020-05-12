/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;
using UnityEngine.Networking;

public class MoverCarta : NetworkBehaviour {

	public int offset;

	PressGesture cogido;
	ReleaseGesture soltar;
	bool usarGravedad;
	Rigidbody rigidbod;

	public float tiempoTranslacion;
	private Vector3 v3inicio;
	private Vector3 v3final;
	private bool hayQueMover;
	private Quaternion qinicio;

	public Vector3 posicionAMover;

	private IEnumerator cambiandoAutoridad;

	[SyncVar]
	public bool moviendo;

	// Use this for initialization
	void Start () {

		cogido = gameObject.GetComponent<PressGesture> ();
		cogido.Pressed += Cogido;
		soltar = gameObject.GetComponent<ReleaseGesture> ();
		soltar.Released += Soltado;

		rigidbod = GetComponent<Rigidbody> ();
		moviendo = false;
		hayQueMover = false;

	}
	
	// Update is called once per frame
	void Update () {


	}

	private void Soltado (object sender, System.EventArgs e) {
		//Debug.Log ("soltado");
		rigidbod.useGravity = true;
		//rigidbod.freezeRotation = false;

	}

	private void Cogido (object sender, System.EventArgs e) {
		Debug.Log ("La carta hasAuthority es: " + hasAuthority);
		transform.position = new Vector3 (transform.position.x, offset, transform.position.z);
		rigidbod.useGravity = false;
		//rigidbod.freezeRotation = true;
	}

	public void Soltada () {
		//Debug.Log ("Soltada");
		if (hayQueMover) {
			
			TrasladarCarta (posicionAMover);
			//trasladarCarta (new Vector3 (transform.position.x, transform.position.y, posicionAMover));
		}
	}

	void OnTriggerEnter (Collider otro) {
		//Debug.Log ("colisionado con: " + otro);
		if (otro.gameObject.tag == "Transicionable") {
			hayQueMover = true;
			posicionAMover = otro.transform.position + (otro.transform.forward * 100);
		} else if (otro.gameObject.tag == "Volvible") {
			hayQueMover = true;
			posicionAMover = Vector3.zero;
		}

		if (otro.gameObject.tag == "Player") {
			if (cambiandoAutoridad != null) {
				StopCoroutine (cambiandoAutoridad);
			}
			cambiandoAutoridad = CambiarAutoridad (otro.gameObject);
			//cambiandoAutoridad = StartCoroutine(CambiarAutoridad (otro.gameObject));
			StartCoroutine(cambiandoAutoridad);
		}
	}

	void OnTriggerExit (Collider otro) {
		if (otro.gameObject.tag == "Transicionable" || otro.gameObject.tag == "Volvible") {
			hayQueMover = false;
		}
	}

	//[Command]
	//[Server]
	public void TrasladarCarta (Vector3 posicionfinal) {
		//Debug.Log ("trasladarCarta");
		transform.position += Vector3.up * 3;
		v3inicio = transform.position;
		v3final = posicionfinal;
		StartCoroutine(Moviendome());
	}

	/*public void RepetirTrasladar () {
		TrasladarCarta (v3final);
	}#1#

	IEnumerator Moviendome() {
		/*while (rotando) {
			yield return 1;
		}#1#
		moviendo = true;
		qinicio = transform.rotation;
		float TiempoInicio = Time.time;
		float tiempoRelativo = Time.time - TiempoInicio;

		while (tiempoRelativo <= tiempoTranslacion) {
			//Debug.Log ("moviendo deberia ser true y es: " + moviendo);
			tiempoRelativo = Time.time - TiempoInicio;
			transform.position = Vector3.Lerp (v3inicio, v3final, tiempoRelativo/tiempoTranslacion);
			transform.rotation = qinicio;
			yield return 1;
		}
		moviendo = false;
	}

	IEnumerator CambiarAutoridad (GameObject jugador){
		//Debug.Log ("moviendo es: " + moviendo);
		while (moviendo) {
			//Debug.Log ("esperando para cambiar autoridad");
			yield return 1;
		}
		jugador.GetComponent<Player> ().CambiarAutoridad (gameObject);
	}
}
*/
