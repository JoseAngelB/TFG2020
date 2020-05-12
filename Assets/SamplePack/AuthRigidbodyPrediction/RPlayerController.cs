using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class RPlayerController : Bolt.EntityEventListener<IRigidbodyPredictionPlayerState>
{
    Rigidbody rbody;
    Transform mirrorPlayer;

    public override void Attached()
    {
        rbody = GetComponent<Rigidbody>();

        state.SetTransforms(state.transform, transform);

        Invoke("test0", 1.0f);

        //CreateMirror();
    }

    public override void ControlGained()
    {
        if (BoltNetwork.IsServer == false)
        {
           // Destroy(mirrorPlayer);
            transform.SetParent(StaticTest.mirrorRoot);

        }
    }

    void test0()
    {
        if(entity.HasControl == false)
        {
            CreateMirror();
        }
    }

    public void CreateMirror()
    {
        GameObject mirrorPlayer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mirrorPlayer.GetComponent<MeshRenderer>().enabled = false;
        mirrorPlayer.transform.SetParent(StaticTest.mirrorRoot);
        mirrorPlayer.transform.localPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (mirrorPlayer)
        {
            mirrorPlayer.localPosition = transform.position;

        }
    }


    public void OnDisable()
    {
        Debug.Log("wew");
    }

    public override void SimulateController()
    {
        IRigidbodyPredictionPlayerCommandInput input = RigidbodyPredictionPlayerCommand.Create();

        input.back = Input.GetKey(KeyCode.S);

        input.forward = Input.GetKey(KeyCode.W);

        input.right = Input.GetKey(KeyCode.D);

        input.left = Input.GetKey(KeyCode.A);
        entity.QueueInput(input);
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        RigidbodyPredictionPlayerCommand cmd = (RigidbodyPredictionPlayerCommand)command;

        if (resetState)
        {
            transform.position = cmd.Result.position;
            transform.rotation = cmd.Result.rotation;
            rbody.velocity = cmd.Result.velocity;
            rbody.angularVelocity = cmd.Result.angularVelocity;
        }
        else
        {

            if (cmd.Input.right)
                rbody.AddForce(Vector3.right * Time.fixedDeltaTime * 400f);
            else if (cmd.Input.left)
                rbody.AddForce(Vector3.left * Time.fixedDeltaTime * 400f);
            if (cmd.Input.forward)
                rbody.AddForce(Vector3.forward * Time.fixedDeltaTime * 400f);
            else if (cmd.Input.back)
                rbody.AddForce(Vector3.back * Time.fixedDeltaTime * 400f);

            StaticTest.localPhysicsScene.Simulate(Time.fixedDeltaTime);

            cmd.Result.position = transform.position;
            cmd.Result.velocity = rbody.velocity;
            cmd.Result.angularVelocity = rbody.angularVelocity;
            cmd.Result.rotation = transform.rotation;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
