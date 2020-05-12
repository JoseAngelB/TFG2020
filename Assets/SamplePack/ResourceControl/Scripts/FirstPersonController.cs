using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class FirstPersonController : Bolt.EntityEventListener<IPlayerState>
{
    ResourceManager.targetData myTargetData;

#pragma warning disable 0414 
    ResourceManagerGeneric.targetData myTargetData2;
#pragma warning restore 0414

    CharacterController _cc;
    bool isGrounded;
    public float airForward;
    public float airRight;
    float yaw;
    float pitch;

    float yVelocity = 0;
    public float Gravity = -9.81f;

    [Range(20, 60)]
    public int ImpulseFrames = 30;

    [Range(10, 40)]
    public float ImpulseForce = 1f;

    [Range(0, 20)]
    public int JumpDebounceFrames = 10;

    public bool SmoothImpulse = true;

    private int debounceFrames = 0;

    int jumpFrames;


    public override void ControlLost()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void ControlGained()
    {
        transform.GetChild(0).gameObject.SetActive(true);

        foreach (Terrain terrain in Terrain.activeTerrains)
        {
            if (terrain.GetComponent<ResourceManager>())
                myTargetData = terrain.GetComponent<ResourceManager>().addTarget(gameObject);
            if (terrain.GetComponent<ResourceManagerGeneric>())
                myTargetData2 = terrain.GetComponent<ResourceManagerGeneric>().addTarget(gameObject);
        }
    }

    public override void Attached()
    {
        if (entity.IsOwner)
        {
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                if (terrain.GetComponent<ResourceManager>())
                    myTargetData = terrain.GetComponent<ResourceManager>().addTarget(gameObject);

                if (terrain.GetComponent<ResourceManagerGeneric>())
                    myTargetData2 = terrain.GetComponent<ResourceManagerGeneric>().addTarget(gameObject);
            }
        }

        _cc = GetComponent<CharacterController>();
        state.SetTransforms(state.transform, transform);
    }
    public override void SimulateController()
    {
        IPlayerCommandInput input = PlayerCommand.Create();

        input.forward = Input.GetKey(KeyCode.W);
        input.backward = Input.GetKey(KeyCode.S);
        input.left = Input.GetKey(KeyCode.A);
        input.right = Input.GetKey(KeyCode.D);
        input.jump = Input.GetKey(KeyCode.Space);
        input.hitTree = Input.GetMouseButtonDown(0);
        yaw += (Input.GetAxisRaw("Mouse X") * 2f);
        yaw %= 360f;

        pitch += (-Input.GetAxisRaw("Mouse Y") * 2f);
        pitch = Mathf.Clamp(pitch, -85f, +85f);

        input.yaw = yaw;
        input.pitch = pitch;

        entity.QueueInput(input);
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        PlayerCommand cmd = (PlayerCommand)command;

        if (resetState)
        {
            //owner has sent a correction to the controller
            transform.position = cmd.Result.position;

            yVelocity = cmd.Result.yVelocity;
            jumpFrames = cmd.Result.jumpFrames;
            isGrounded = cmd.Result.isGrounded;
            debounceFrames = cmd.Result.debounceFrames;


            airForward = cmd.Result.airMovementForward;
            airRight = cmd.Result.airMovementRight;
        }
        else
        {


            //server side hit detection
            if (BoltNetwork.IsServer)
            {


                Debug.DrawRay(transform.position + Vector3.up, transform.forward, Color.green);
                if (cmd.Input.hitTree == true)
                {
                    RaycastHit hit;

                    if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, 5f))
                    {
                        if (hit.transform.GetComponent<CapsuleController>())
                            hit.transform.GetComponent<CapsuleController>().kill = true;
                        if (hit.transform.GetComponent<CapsuleControllerGeneric>())
                            hit.transform.GetComponent<CapsuleControllerGeneric>().kill = true;
                    }
                }
            }


            if (entity.HasControl)
            {
                transform.GetChild(0).localRotation = Quaternion.Euler(cmd.Input.pitch, 0, 0);
            }

            transform.localRotation = Quaternion.Euler(0, cmd.Input.yaw, 0);

            float moveSpeed = 6f;
            if (isGrounded == false)
            {
                moveSpeed *= 0.2f;
            }

            Vector3 movingDir = Vector3.zero;


            if (cmd.Input.forward)
            {
                movingDir.z = -1;
                _cc.Move(transform.forward * Time.fixedDeltaTime * moveSpeed);

            }
            else if (cmd.Input.backward)
            {
                movingDir.z = 1;
                _cc.Move(transform.forward * Time.fixedDeltaTime * -moveSpeed);

            }
            else
            {
                movingDir.z = 0;

            }

            if (cmd.Input.left)
            {
                movingDir.x = 1;
                _cc.Move(transform.right * Time.fixedDeltaTime * -moveSpeed);

            }
            else if (cmd.Input.right)
            {
                movingDir.x = -1;
                _cc.Move(transform.right * Time.fixedDeltaTime * moveSpeed);

            }
            else
            {
                movingDir.x = 0;
            }



            if (isGrounded == true)
            {

                movingDir = Vector3.Normalize(Quaternion.Euler(0, cmd.Input.yaw, 0) * movingDir);
                airForward = movingDir.z;
                airRight = movingDir.x;

            }


            if (cmd.Input.jump && jumpFrames == 0 && debounceFrames == 0 && isGrounded)
            {

                yVelocity = 0;
                jumpFrames = ImpulseFrames;
                debounceFrames = JumpDebounceFrames;

            }
            else if (jumpFrames > 0)
            {
                --jumpFrames;
            }
            else if (debounceFrames > 0)
            {
                --debounceFrames;
            }
            else if (isGrounded)
                yVelocity = -10f;

            if (isGrounded == false)
            {


                _cc.Move(new Vector3(airRight, 0, airForward) * Time.fixedDeltaTime * -4);


            }

            yVelocity += Gravity * Time.fixedDeltaTime; //accellerate
            float yForce = yVelocity;
            if (jumpFrames > 0)
                yForce += SmoothImpulse
                ? Mathf.Lerp(0, ImpulseForce, (float)jumpFrames / ImpulseFrames)
                                                        : ImpulseForce;



            _cc.Move(Vector3.up * yForce * Time.fixedDeltaTime);











            cmd.Result.position = transform.position;


            isGrounded = _cc.isGrounded;

            cmd.Result.yVelocity = yVelocity;
            cmd.Result.isGrounded = isGrounded;
            cmd.Result.jumpFrames = jumpFrames;
            cmd.Result.debounceFrames = debounceFrames;


            cmd.Result.airMovementForward = airForward;
            cmd.Result.airMovementRight = airRight;


        }

    }
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (myTargetData == null)
            {
                foreach (Terrain terrain in Terrain.activeTerrains)
                {
                    if (terrain.GetComponent<ResourceManager>())
                        myTargetData = terrain.GetComponent<ResourceManager>().addTarget(gameObject);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            if (myTargetData != null)
                foreach (Terrain terrain in Terrain.activeTerrains)
                {
                    if (terrain.GetComponent<ResourceManager>())
                    {
                        terrain.GetComponent<ResourceManager>().removeTarget(myTargetData);
                        myTargetData = null;
                    }
                }
        }

    }
}
