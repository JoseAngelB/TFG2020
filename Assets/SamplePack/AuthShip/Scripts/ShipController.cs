using Bolt;
using UnityEngine;

public class ShipController : Bolt.EntityEventListener<IShipState>
{
    public CharacterController _cc;

    public float slideSpeed = 0.1f;
    public float yawSpeed = 0.1f;
    public float bankSpeed = 0.1f;
    public float pitchSpeed = 0.1f;
    public float boostFactor = 1.15f;
    public float decayFactor = 0.1f;
    float mX;
    float mY;

    float kX;
    float kY;
    float kZ;
    float kS;
    float kE;
    float kM;
    float sLerp;
    float eLerp;
    float mLerp;
    float x;
    float y;
    float z;
    float xLerp;
    float yLerp;
    float zLerp;

    float yaw;
    float pitch;   



    public override void Attached()
    {
        state.SetTransforms(state.transform, transform);
    }



    public override void SimulateController()
    {
        IShipCommandInput input = ShipCommand.Create();

        if (Input.GetKey(KeyCode.Space))
            input.elevate = 1;
        else if (Input.GetKey(KeyCode.LeftControl))
            input.elevate = -1;

        if (Input.GetKey(KeyCode.W))
            input.move = 1;
        else if (Input.GetKey(KeyCode.S))
            input.move = -1;

        if (Input.GetKey(KeyCode.D))
            input.strafe = 1;
        else if (Input.GetKey(KeyCode.A))
            input.strafe = -1;     

        input.yaw = yaw;
        input.pitch = -pitch;
        
        entity.QueueInput(input);      
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        ShipCommand cmd = (ShipCommand)command;

        if (resetState)
        {
            transform.position = cmd.Result.position;
            transform.rotation = cmd.Result.rotation;
            mLerp = cmd.Result.mLerp;
            eLerp = cmd.Result.eLerp;
            sLerp = cmd.Result.sLerp;
            xLerp = cmd.Result.xLerp;
            yLerp = cmd.Result.yLerp;
            zLerp = cmd.Result.zLerp;

        }
        else
        {
            slide(cmd.Input.elevate, cmd.Input.strafe, cmd.Input.move);
            rot(cmd.Input.yaw, cmd.Input.pitch, cmd.Input.yawButton, cmd.Input.pitchButton, cmd.Input.bankButton);

            cmd.Result.position = transform.position;
            cmd.Result.mLerp = mLerp;
            cmd.Result.eLerp = eLerp;
            cmd.Result.sLerp = sLerp;
            cmd.Result.xLerp = xLerp;
            cmd.Result.yLerp = yLerp;
            cmd.Result.zLerp = zLerp;
            cmd.Result.rotation = transform.rotation;
        }

    }


    void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (entity.IsControllerOrOwner)
        {
            yaw = Input.GetAxisRaw("Mouse X");
            pitch = Input.GetAxisRaw("Mouse Y");
        }
    }


    public void rot(float yaw, float pitch, float yawButton, float pitchButton, float bankButton)
    {
        mX = yaw;
        mY = pitch;

        kX = yawButton;

        kY = pitchButton;

        kZ = bankButton;

        x = Mathf.Clamp(mX + /*jX*/ +kX, -1.0f, 1.0f);
        y = Mathf.Clamp(mY + /*jY*/+kY, -1.0f, 1.0f);
        z = Mathf.Clamp(/*jZ*/ +kZ, -1.0f, 1.0f);

        bool smoothRot = false;

        if (Mathf.Abs(x) < 0.1f && smoothRot == true)
            xLerp = Mathf.Lerp(xLerp, 0, (2f * Time.fixedDeltaTime));
        else xLerp = x;

        if (Mathf.Abs(y) < 0.1f && smoothRot == true)
            yLerp = Mathf.Lerp(yLerp, 0, (2f * Time.fixedDeltaTime));
        else yLerp = y;

        if (Mathf.Abs(z) < 0.1f && smoothRot == true)
            zLerp = Mathf.Lerp(zLerp, 0, (2f * Time.fixedDeltaTime));
        else zLerp = z;
   
        transform.Rotate(yLerp * pitchSpeed, xLerp * yawSpeed, zLerp * bankSpeed);
    }

    void slide(float elevate, float strafe, float move)
    {  
        float s, e, m;
        e = elevate;
        s = strafe;
        m = move;

        if (Mathf.Abs(e) < 0.1f)
            eLerp = Mathf.Lerp(eLerp, 0, (2f * Time.fixedDeltaTime));
        else if (Mathf.Abs(e) >= 1.0f)
        {
            if (e > 0)
                eLerp = Mathf.Lerp(eLerp, 2, (2f * Time.fixedDeltaTime));
            else eLerp = Mathf.Lerp(eLerp, -2, (2f * Time.fixedDeltaTime));
            Debug.Log(eLerp);
        }
        else eLerp = e;

        if (Mathf.Abs(s) < 0.1f)
            sLerp = Mathf.Lerp(sLerp, 0, (2f * Time.fixedDeltaTime));
        else if (Mathf.Abs(s) >= 1.0f)
        {
            if (s > 0)
                sLerp = Mathf.Lerp(sLerp, 2, (2f * Time.fixedDeltaTime));
            else sLerp = Mathf.Lerp(sLerp, -2, (2f * Time.fixedDeltaTime));
            Debug.Log(sLerp);
        }
        else sLerp = s;

        if (Mathf.Abs(m) < 0.1f)
            mLerp = Mathf.Lerp(mLerp, 0, (2f * Time.fixedDeltaTime));
        else if (Mathf.Abs(m) >= 1.0f)
        {
            if (m > 0)
                mLerp = Mathf.Lerp(mLerp, 2, (2f * Time.fixedDeltaTime));
            else mLerp = Mathf.Lerp(mLerp, -2, (2f * Time.fixedDeltaTime));
            Debug.Log(mLerp);
        }
        else mLerp = m;

        float mySpeed = 0.1f;
    
      
        _cc.Move((transform.right * sLerp * mySpeed) + (transform.up * eLerp * mySpeed) + (transform.forward * mLerp * mySpeed));
    }

    public override void ControlGained()
    {
        GameObject.FindWithTag("MainCamera").transform.SetParent(this.transform);
        GameObject.FindWithTag("MainCamera").transform.localPosition = this.transform.position;
        GameObject.FindWithTag("MainCamera").transform.localRotation = this.transform.rotation;

    }

    void OnApplicationFocus(bool focused)
    {
        //  if (focused)
        //      Cursor.lockState = CursorLockMode.Locked;

        //  if (!focused)
        //       Cursor.lockState = CursorLockMode.None;
    }
}