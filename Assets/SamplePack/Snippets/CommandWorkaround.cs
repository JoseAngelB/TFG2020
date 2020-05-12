#if false
using UnityEngine;
using System.Collections;
using Bolt;

public class hostMigrationPlayer : Bolt.EntityEventListener<IhostMigrationPlayerState>
{
    public bool[] frames = new bool[100000];

    public CharacterController _cc;

    public override void Attached()
    {
        _cc = GetComponent<CharacterController>();
        state.SetTransforms(state.transform, transform);
    }



    public override void SimulateController()
    {
        IhostMigrationPlayerCommandInput input = hostMigrationPlayerCommand.Create();
        input.forward = Input.GetKey(KeyCode.W);
        input.back = Input.GetKey(KeyCode.S);
        input.left = Input.GetKey(KeyCode.A);
        input.right = Input.GetKey(KeyCode.D);

        entity.QueueInput(input);
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        hostMigrationPlayerCommand cmd = (hostMigrationPlayerCommand)command;
        if (BoltNetwork.IsServer && entity.hasControl == false)
        {
            if (frames[cmd.ServerFrame] != true)
            {
                frames[cmd.ServerFrame] = true;
            }
            else
            {
                cmd.Result.position = transform.position;
                return;
            }
        }


        if (resetState)
        {
            //owner has sent a correction to the controller
            transform.position = cmd.Result.position;

        }
        else
        {
            if (cmd.Input.forward)
                _cc.Move(Vector3.forward * Time.fixedDeltaTime * 10f);
            else if (cmd.Input.back)
                _cc.Move(Vector3.back * Time.fixedDeltaTime * 10f);
            if (cmd.Input.left)
                _cc.Move(Vector3.left * Time.fixedDeltaTime * 10f);
            else if (cmd.Input.right)
                _cc.Move(Vector3.right * Time.fixedDeltaTime * 10f);

            cmd.Result.position = transform.position;

        }
    }

}
#endif