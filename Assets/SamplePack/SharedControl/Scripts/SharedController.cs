using UnityEngine;
using System.Collections;
using Bolt;

public class SharedController : Bolt.EntityEventListener<IDefaultNPCState>
{
    float cooldown = 0;

    public LayerMask validLayers = new LayerMask();
    public Vector3 _destinationPosition = Vector3.zero;
    public CharacterController _cc;

    public override void Attached()
    {
        _cc = GetComponent<CharacterController>();
        state.SetTransforms(state.transform, transform);
    }


    public void Update()
    {
        if (!entity.HasControl)
            if (cooldown <= 0)
                if (Input.GetMouseButtonDown(0))
                {
                    var casingEvnt = requestControl.Create(entity, EntityTargets.OnlyOwner);
                    casingEvnt.Send();
                    cooldown = 0.1f;
                }

        if (cooldown > 0)
            cooldown -= Time.deltaTime;
    }

    public override void OnEvent(requestControl evnt)
    {
        Debug.Log("got event");

        if (BoltNetwork.IsServer)
        {
            //sent by host
            if (evnt.RaisedBy == null)
            {
                //not controlled
                if (entity.IsControlled == false)
                {
                    entity.TakeControl();  //host takes control
                }
                else if (entity.Controller != null)
                {
                    entity.TakeControl();  //host takes control from client
                }
                else entity.ReleaseControl();  //if controlled by host, release
            }
            else if (entity.Controller != evnt.RaisedBy) //if event is not from controller
                entity.AssignControl(evnt.RaisedBy);


            //else entity.RevokeControl();
        }
    }

    public override void SimulateController()
    {
        IclickToMoveCommandInput input = clickToMoveCommand.Create();
        Vector3 position = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000, validLayers);


        if (Input.GetMouseButton(0))
            foreach (RaycastHit hit in hits)
            {
                //Debug.Log(hit.point);
                if (!hit.collider.isTrigger)
                {
                    _destinationPosition = hit.point;
                    break;
                }
            }


        input.click = _destinationPosition;
        //transform.position = _destinationPosition + Vector3.up;
        entity.QueueInput(input);
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        
        clickToMoveCommand cmd = (clickToMoveCommand)command;

        Debug.Log("Comando ejecutado: " + cmd);
        if (resetState)
        {
            //owner has sent a correction to the controller
            transform.position = cmd.Result.position;
        }
        else
        {

            if (cmd.Input.click != Vector3.zero)
            {
                if (Vector3.Distance(transform.position, new Vector3(cmd.Input.click.x, transform.position.y, cmd.Input.click.z)) > 0.3f) //cmd.Input.click) > 0.3f)
                {
                    transform.LookAt(new Vector3(cmd.Input.click.x, transform.position.y, cmd.Input.click.z));
                    _cc.Move(transform.TransformDirection(new Vector3(0, 0, 1) * 0.1f));
                    //_cc.Move(transform.InverseTransformDirection(new Vector3(cmd.Input.click.x, transform.position.y, cmd.Input.click.z)) * 0.01f);
                    //Debug.Log("transform es " + transform.position);
                    //Debug.Log("Input es " + new Vector3(cmd.Input.click.x, transform.position.y, cmd.Input.click.z));
                    //Debug.Log("TransformDirection es " + transform.TransformDirection(new Vector3(cmd.Input.click.x, transform.position.y, cmd.Input.click.z)));
                    //Debug.Log("InverseTransformDirection es " + transform.InverseTransformDirection(new Vector3(cmd.Input.click.x, transform.position.y, cmd.Input.click.z)));
                    //Debug.Log("TransformVector es " + transform.TransformVector(new Vector3(cmd.Input.click.x, transform.position.y, cmd.Input.click.z)));
                    //Debug.Log("InverseTransformVector es " + transform.InverseTransformVector(new Vector3(cmd.Input.click.x, transform.position.y, cmd.Input.click.z)));
                }
            }

            cmd.Result.position = transform.position;
            cmd.Result.velocity = GetComponent<CharacterController>().velocity;
        }
        //transform.position = _destinationPosition + Vector3.up;
    }

}
