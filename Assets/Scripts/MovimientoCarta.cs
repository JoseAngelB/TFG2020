using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using UnityEngine.Experimental.UIElements;

public class MovimientoCarta : Bolt.EntityEventListener<ICartaState>
{
    private bool moviendo = false;
    private float distancia;
    private Vector3 distanciaInicial;

    private float cooldown = 0f;

    public float altura;
    private Plane planoMovimiento;
    
    public override void Attached()
    {
        state.SetTransforms(state.CartaTransform, transform);
        
        planoMovimiento = new Plane(Vector3.up, Vector3.up * altura);
        
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
    }
 
    void OnMouseUp()
    {
        moviendo = false;
    }

    private void Update()
    {
        /*if (!entity.HasControl)
            if (cooldown <= 0)
                if (Input.GetMouseButtonDown(0))
                {
                    var casingEvnt = requestControl.Create(entity, EntityTargets.OnlyOwner);
                    casingEvnt.Send();
                    cooldown = 0.1f;
                }

        if (cooldown > 0)
            cooldown -= Time.deltaTime;
        */
        
        
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
        input.position = transform.position + distanciaInicial;
        entity.QueueInput(input);

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
                Debug.Log("El server coge el control");
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
                Debug.Log("Asignado el control a: " + evnt.RaisedBy);
            }

            else entity.RevokeControl();
        }
        //Debug.Log("El control lo tiene ahora: " + entity.Controller);
    }

    public override void ControlGained()
    {
        base.ControlGained();
    }
}
