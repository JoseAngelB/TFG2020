using UnityEngine;
using System.Collections;
using Bolt;

public class RigidbodyPlayerController : Bolt.EntityEventListener<IRigidbodyPlayerState>
{
    public Rigidbody myRigidbody;

    public override void Attached()
    {
        myRigidbody = GetComponent<Rigidbody>();
        state.SetTransforms(state.transform, transform);
    }

    public override void SimulateOwner()
    {
        state.lookRotation = transform.GetChild(0).GetChild(3).transform.localRotation;



    }

    public void FixedUpdate()
    {
        if (entity.IsControllerOrOwner == false)
        {
            transform.GetChild(0).GetChild(3).transform.rotation = state.lookRotation;
        }

    }


    public override void SimulateController()
    {
        //  Vector3 position = Input.mousePosition;
        //  Ray ray = Camera.main.ScreenPointToRay(position);
        //  RaycastHit[] hits = Physics.RaycastAll(ray, 1000, validLayers);

        var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 18));

        var direction = worldMousePosition - transform.position;
        direction.y = 0;
        direction.Normalize();





        IrigidbodyPlayerCommandInput input = rigidbodyPlayerCommand.Create();
        input.moveRight = Input.GetKey(KeyCode.D);
        input.moveLeft = Input.GetKey(KeyCode.A);
        input.moveUp = Input.GetKey(KeyCode.W);
        input.moveDown = Input.GetKey(KeyCode.S);
        input.lookDirection = (Vector3)direction;
        entity.QueueInput(input);

    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        rigidbodyPlayerCommand cmd = (rigidbodyPlayerCommand)command;

        if (BoltNetwork.IsServer)
        {
            if (cmd.Input.moveRight)
            {
                Vector3 m_EulerAngleVelocity = new Vector3(0, 50, 0);
                Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.deltaTime);
                myRigidbody.MoveRotation(deltaRotation * myRigidbody.rotation);
            }
            else if (cmd.Input.moveLeft)
            {
                Vector3 m_EulerAngleVelocity = new Vector3(0, -50, 0);
                Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.deltaTime);
                myRigidbody.MoveRotation(deltaRotation * myRigidbody.rotation);
            }
            //myRigidbody.AddForce(Vector3.left * 15f);
            if (cmd.Input.moveUp)
                myRigidbody.AddForce(transform.forward * 15f);
            else if (cmd.Input.moveDown)
                myRigidbody.AddForce(transform.forward * -15f);
        }

        transform.GetChild(0).GetChild(3).transform.rotation = Quaternion.LookRotation(cmd.Input.lookDirection);

    }

}
