using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerControl
{
    Collision mCollision = null;

    #region 貓貓控制
    void CatControl()
    {
        #region Btn A
        if (BtnAIsPress && mCollision == null)
        {
            if (NowControlType != ControlType.Rotate)
            {
                ClientCommand.identity.id = 4;
                ClientCommand.identity.running = Vector3.zero;
                ClientCommand.identity.runningForward = Vector3.zero;
                ClientCommand.identity.mapping = Clockwise ? 1 : 0;
                GM.services.Send();
            }
        }
        else if (BtnAIsPress == false)
        {
            if (NowControlType == ControlType.Rotate)
            {
                ClientCommand.identity.id = 1;
                ClientCommand.identity.running = Vector3.zero;
                ClientCommand.identity.runningForward = Vector3.zero;
                GM.services.Send();
            }
        }
        #endregion

        #region Btn B
        if (BtnBIsPress && Time.time > SpeedupCD)
        {
            SpeedupCD = Time.time + 5f;
            ClientCommand.identity.id = 3;
            ClientCommand.identity.running = transform.position;
            ClientCommand.identity.runningForward = transform.forward;

            GM.services.Send();
        }
        #endregion

        switch (NowControlType)
        {
            case ControlType.Move:
                CatMove();
                break;
            case ControlType.Rotate:
                CatRotate();
                break;
            case ControlType.ReadyRun:
                break;
            case ControlType.Run:
                break;
        }
    }

    void CatMove()
    {
        this.transform.Translate(transform.forward * mDeltaTime * CatSpeed, Space.World);

        if (ModelControl != null)
            ModelControl.Move();

        if (mCollision != null && Vector3.Distance(mCollision.contacts[0].point, transform.position) < 0.1f)
            mCollision = null;
    }

    void CatRotate()
    {
        ModelControl.Move();
        this.transform.Rotate(Clockwise ? -Vector3.up : Vector3.up, mDeltaTime * RoateSpeed);
    }
    #endregion

    void OnCollisionEnter(Collision _collision)
    {
        if (_collision != mCollision)
        {
            mCollision = _collision;
            var refrect = Vector3.Reflect(this.transform.forward, _collision.contacts[0].normal);
            transform.forward = refrect;
        }
    }

    void OnCollisionExit(Collision _collision)
    {
        mCollision = null;
    }
}
