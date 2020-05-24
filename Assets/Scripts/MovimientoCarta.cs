using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using UnityEngine.Experimental.UIElements;

public class MovimientoCarta : Bolt.EntityEventListener<ICartaState>
{
    private bool moviendo = false;
    private bool rotando = false;
    private bool hayQueRotar = false;
    private bool hayQueRotarRapido = false;
    private float distancia;
    private Vector3 distanciaInicial;

    private float cooldown = 0f;

    public float altura;
    private Plane planoMovimiento;

    [SerializeField] private float tiempoExtraMoviendo;
    [SerializeField] private float tiempoDobleClick;
    [SerializeField] private float tiempoRotacion;
    [SerializeField] private float alturaRotacion;
    [SerializeField] private float tiempoRotacionParada;
    
    private float tiempoTranscurrido;

    private Quaternion qinicio;
    private Quaternion qfinal;
    private Vector3 v3inicio;
    
    
    public override void Attached()
    {
        state.SetTransforms(state.CartaTransform, transform);
        
        planoMovimiento = new Plane(Vector3.up, Vector3.up * altura);
        
    }

    private void Start()
    {
        tiempoTranscurrido = 0;
        v3inicio = transform.position;
    }

    void OnMouseEnter()
    {
        //marcar el que se puede mover
    }
 
    void OnMouseExit()
    {
        //desmarcar el que se puede mover
    }
 
    void OnMouseDown()
    {
        
        //si no tenemos el control lo pediremos
        if (!entity.HasControl)
        {
            var casingEvnt = requestControl.Create(entity, EntityTargets.OnlyOwner);
            casingEvnt.Send();
        }
        
        distancia = Vector3.Distance(transform.position, Camera.main.transform.position);
        moviendo = true;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(distancia);
        distanciaInicial = transform.position - rayPoint;

        if (tiempoTranscurrido < tiempoDobleClick && !rotando)
        {
            hayQueRotar = true;
            var eventoRotar = GirarCartaEvent.Create(entity, EntityTargets.EveryoneExceptController);
            eventoRotar.Despacio = true;
            eventoRotar.Send();
        }
        else
        {
            v3inicio = transform.position;
        }
    }
 
    void OnMouseUp()
    {
        moviendo = false;
        tiempoTranscurrido = 0;
    }

    private void Update()
    {
        tiempoTranscurrido += Time.deltaTime;

        if (moviendo)
        {
            //Debug.Log("Intento mover en el update");
            Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Vector3 puntoDelRayo = rayo.GetPoint(distancia);
            //transform.position = puntoDelRayo + distanciaInicial;
            
            
            if(planoMovimiento.Raycast(rayo, out distancia))
            {
                transform.position = rayo.GetPoint(distancia) + distanciaInicial;    //distancia del rayo
            }
        }

        if (hayQueRotar)
        {
            hayQueRotar = false;
            StartCoroutine (Rotandome ());
        }
        if (hayQueRotarRapido)
        {
            hayQueRotarRapido = false;
            RotarRapido ();
        }
    }


    public override void SimulateController()
    {
        IComandoMoverCartaInput input = ComandoMoverCarta.Create();

        if (moviendo)
        {
            Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (planoMovimiento.Raycast(rayo, out distancia))
            {
                transform.position = rayo.GetPoint(distancia);
                input.click = rayo.GetPoint(distancia); //distancia del rayo
                input.position = transform.position + distanciaInicial;
            }
            entity.QueueInput(input);
        }

        if (tiempoTranscurrido < tiempoExtraMoviendo)
        {
            input.position = transform.position + distanciaInicial;
            entity.QueueInput(input);
        }

        if (hayQueRotar)
        {
            hayQueRotar = false;
            StartCoroutine (Rotandome ());
        }
        if (hayQueRotarRapido)
        {
            hayQueRotarRapido = false;
            RotarRapido ();
        }

        if (rotando)
        {
            input.position = transform.position;
            input.rotation = transform.rotation;
            entity.QueueInput(input);
        }
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        
        ComandoMoverCarta cmd = (ComandoMoverCarta)command;

        //Debug.Log("Comando ejecutado: " + cmd);
        
        if (resetState)
        {
            //owner has sent a correction to the controller
            transform.position = cmd.Result.posicion;
        }
        else
        {

            if (cmd.Input.click != Vector3.zero)
            {
                transform.position = cmd.Input.position;
            }

            cmd.Result.posicion = transform.position;
        }
        transform.position = cmd.Result.posicion;
    }

    public override void OnEvent(requestControl evnt)
    {
        //Debug.Log("El control lo tenía: " + entity.Controller);
        //Debug.Log("entity.name es : " + entity.name +" entity.NetworkId es : " + entity.NetworkId);

        if (BoltNetwork.IsServer)
        {
            //enviado por el host
            if (evnt.RaisedBy == null)
            {
                //Debug.Log("El server coge el control");
                //si no está controlado
                if (entity.IsControlled == false)
                {
                    entity.TakeControl();  //el host coge el control
                }
                else if (entity.Controller != null)
                {
                    entity.TakeControl();  //el host coge el control del cliente
                }
                else entity.ReleaseControl();  //si está controlado por el host lo liberamos
            }
            else if (entity.Controller != evnt.RaisedBy) //si el evento no viene por el controlador
            {
                entity.AssignControl(evnt.RaisedBy);
                //Debug.Log("Asignado el control a: " + evnt.RaisedBy);
            }

            else entity.RevokeControl();
        }
        //Debug.Log("El control lo tiene ahora: " + entity.Controller);
    }

    public override void OnEvent(GirarCartaEvent evnt)
    {
        Debug.Log("Recibo evento para girar carta");
        if (evnt.Despacio)
        {
            hayQueRotar = true;
        }
        else
        {
            hayQueRotarRapido = true;
        }
    }

    IEnumerator Rotandome()
    {
        GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
        qinicio = transform.rotation;
        transform.Rotate(0, 0, 180);
        qfinal = transform.rotation;
        transform.rotation = qinicio;
        //v3inicio = transform.position;
        //transform.position = v3inicio;
        rotando = true;
        float tiempoInicio = Time.time;
        float tiempoPasado = Time.time - tiempoInicio;

        while (tiempoPasado <= tiempoRotacion) {
            tiempoPasado = Time.time - tiempoInicio;
            transform.rotation = Quaternion.Lerp (qinicio, qfinal, tiempoPasado/tiempoRotacion);

            float altura = tiempoPasado * alturaRotacion * 2;
            if (altura > alturaRotacion)
                altura = alturaRotacion - (altura - alturaRotacion);

            transform.position = v3inicio + Vector3.up * altura;
            yield return 1;
        }
        rotando = false;
        v3inicio = transform.position;	//esto es por si después de rotar se tiene que mover como por ejemplo en "robar"

        GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation;
        yield return tiempoRotacionParada * 30;
        GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
    }
    
    /// <summary>
    /// voltea la carta boca abajo si no lo está
    /// </summary>
    /// <param name="bocaAbajo">true boca abajo, false boca arriba</param>
    public void Voltear(bool bocaAbajo)
    {
        if (!EstaBocaAbajo())
        {
            hayQueRotar = true;
            var eventoRotar = GirarCartaEvent.Create(entity, EntityTargets.EveryoneExceptController);
            eventoRotar.Despacio = false;
            eventoRotar.Send();
        }
    }

    bool EstaBocaAbajo()
    {
        float yDelante = GetComponentsInChildren<Transform>()[1].position.y;
        float yDetras = GetComponentsInChildren<Transform>()[2].position.y;
        Debug.LogFormat("yDelante es {0} yDetras es {1}", yDelante, yDetras);
        return yDelante > yDetras;
    }
    
    
    void RotarRapido()
    {
        Quaternion rotacion = transform.rotation;
        float rotacionX = rotacion.eulerAngles.x;
        rotacionX += 180;
        if (rotacionX > 360 || rotacionX < -360)
            rotacionX = rotacionX % 360;
        transform.rotation = Quaternion.Euler(rotacionX, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}
