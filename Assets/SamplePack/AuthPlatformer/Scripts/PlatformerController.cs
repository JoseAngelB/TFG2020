using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class PlatformerController : Bolt.EntityEventListener<IPlatformerPlayer>
{
    //how fast player goes
    private int speed = 5;

    //keep track of old position to see if we're moving in a new direct and need to flip player
    private float oldPositionX;

    //mask for player collisions with objects
    [SerializeField]
    private LayerMask mask;

    //velocity for jumping
    private float velocity = 0;

    //whether or not player is grounded, need to be grounded to jump
    private bool grounded = false;

    public override void Attached()
    {
        state.SetTransforms(state.transform, transform);
    }

    public override void SimulateController()
    {
        IplatformerPlayerCommandInput input = platformerPlayerCommand.Create();

        input.left = Input.GetKey(KeyCode.A);
        input.right = Input.GetKey(KeyCode.D);
        input.up = Input.GetKey(KeyCode.W);

        entity.QueueInput(input);
    }

    private void Update()
    {
        //see if we're moving in a new direct and need to flip player
        if (oldPositionX != transform.position.x)
        {
            if (oldPositionX < transform.position.x)
                GetComponent<SpriteRenderer>().flipX = false;
            else
                GetComponent<SpriteRenderer>().flipX = true;

            oldPositionX = transform.position.x;

        }
    }

    //helper method to check if player is colliding with something
    public bool RayCheck(Vector3 rayOrigin, Vector3 dir)
    {
        Debug.DrawRay(rayOrigin, dir, Color.red);
        return Physics2D.Raycast(rayOrigin, dir, dir.magnitude, mask);

    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        platformerPlayerCommand cmd = (platformerPlayerCommand)command;

        if (resetState)
        {
            //owner has sent a correction to the controller
            transform.position = cmd.Result.position;
            velocity = cmd.Result.velocity;
            grounded = cmd.Result.grounded;
        }
        else
        {

            velocity -= Time.fixedDeltaTime * 2 * speed;

            bool collisionLeft;
            bool collisionRight;
            bool collisionAbove;


            if (velocity > 0)
            {
                //check above collision
                collisionAbove = RayCheck(transform.position + new Vector3(0.4f, -0.5f, 0), Vector2.up) ||
                    RayCheck(transform.position + new Vector3(-0.4f, -0.5f, 0), Vector2.up);

                if (collisionAbove == true)
                {
                    velocity = -1;
                }
            }


            //check right side collision

            collisionRight = RayCheck(transform.position + new Vector3(0, 0.1f, 0), Vector2.right * 0.5f) ||
                    RayCheck(transform.position + new Vector3(0, -0.9f, 0), Vector2.right * 0.5f);



            //check left side collision

            collisionLeft = RayCheck(transform.position + new Vector3(0, 0.1f, 0), Vector2.left * 0.5f) ||
                  RayCheck(transform.position + new Vector3(0, -0.9f, 0), Vector2.left * 0.5f);




            if (cmd.Input.left && !collisionLeft)
                transform.Translate(Vector2.left * Time.fixedDeltaTime * speed);
            else if (cmd.Input.right && !collisionRight)
                transform.Translate(Vector2.right * Time.fixedDeltaTime * speed);

            if (cmd.Input.up)
            {
                if (grounded)
                {
                    velocity = 1.3f * speed;
                }
            }


            bool groundHit = RayCheck(transform.position + new Vector3(0.4f, 0f, 0), Vector2.down) ||
                  RayCheck(transform.position + new Vector3(-0.4f, 0f, 0), Vector2.down);


            if (!groundHit)
            {
                grounded = false;
            }
            else if (velocity < 0)
            {
                velocity = 0;
                grounded = true;
            }

            transform.Translate(Vector2.up * Time.deltaTime * velocity);

            cmd.Result.position = transform.position;
            cmd.Result.grounded = grounded;
            cmd.Result.velocity = velocity;
        }
        //when jump pressed, velocity goes to 1 and gradually goes down until terminal velocity

    }
}
