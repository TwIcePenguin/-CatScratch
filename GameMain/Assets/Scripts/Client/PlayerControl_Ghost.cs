using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerControl
{
    #region 鬼控制
    void GhostControl()
    {
        if (IsInitPoint == false)
            return;

        #region Btn B
        if (BtnBIsPress && Time.time > mRunCDTimer)
        {
            if (NowControlType == ControlType.Move)
            {
                NowControlType = ControlType.ReadyRun;

                #region 鬼魂 衝刺前準備
                GhostRotateVal = Random.Range(0f, 1f);

                RandomF = Random.Range(0, 10) > 5;
                CacheForward = transform.forward;
                CacheRight = transform.right;

                ClientCommand.identity.id = 4;
                ClientCommand.identity.running = transform.position;
                ClientCommand.identity.runningForward = transform.forward;
                ClientCommand.identity.fVaule = GhostRotateVal;
                ClientCommand.identity.mapping = RandomF ? 1 : 0;
                #endregion

                GM.services.Send();
            }

            if (NowControlType == ControlType.ReadyRun)
                GhostReadyRun();
        }
        else if (BtnBIsPress == false)
        {
            if (NowControlType == ControlType.ReadyRun)
            {
                NowControlType = ControlType.Run;
                OBTimer = -1f;

                RaycastHit hit;
                RunFinalPoint = Vector3.zero;

                if (Physics.Raycast(transform.position, transform.forward, out hit))
                {
                    RunFinalPoint = hit.point;
                    TargetMoveIndex = PointsTools.Instance.FindMoveIndex(hit.collider);
                }
                else
                {
                    OBTimer = Time.time + OBDelay;
                    TargetMoveIndex = Random.Range(0, 5);
                }

                ClientCommand.identity.id = 3;
                ClientCommand.identity.fVaule = OBTimer;
                ClientCommand.identity.running = RunFinalPoint;
                ClientCommand.identity.runningForward = transform.forward;
                ClientCommand.identity.mapping = TargetMoveIndex;

                GM.services.Send();
            }
        }
        #endregion

        if (NowControlType == ControlType.Move)
        {
            GhostMove();
        }
        else if (NowControlType == ControlType.Run)       //開始衝刺
        {
            GhostRun();
            //達到目標點
            if (Vector3.Distance(RunFinalPoint, transform.position) < 0.1f)
            {
                transform.position = RunFinalPoint;
                mRunCDTimer = Time.time + mRunCD;
                OBTimer = -1;

                ClientCommand.identity.id = 1;
                ClientCommand.identity.running = transform.position;
                ClientCommand.identity.runningForward = transform.forward;
                ClientCommand.identity.gotchaId = 0;
                ClientCommand.identity.mapping = Clockwise ? 1 : 0;
                GM.services.Send();

                NowControlType = ControlType.Move;
            }
        }
    }

    /// <summary> 鬼決定抓人的方向 </summary>
    void GhostReadyRun()
    {
        GhostRotateVal = RandomF ? GhostRotateVal + Time.deltaTime : GhostRotateVal - Time.deltaTime;

        float val = Mathf.Lerp(MinRange, MaxRange, GhostRotateVal);

        transform.eulerAngles = Quaternion.LookRotation(CacheForward + CacheRight * val).eulerAngles;

        if (GhostRotateVal >= 1)
            RandomF = false;
        else if (GhostRotateVal <= 0)
            RandomF = true;
    }

    /// <summary> 鬼衝刺抓人 </summary>
    void GhostRun()
    {
        AudioPlayer.Instance.playByName("attack");
        ModelControl.Attack();

        if (OBTimer > Time.time)
            transform.Translate(transform.forward * Time.deltaTime * GhostSpeed, Space.World);
        else
            transform.position = Vector3.MoveTowards(transform.position, RunFinalPoint, Time.deltaTime * GhostSpeed);
    }

    void GhostMove()
    {
        TargetMoveIndex = PointsTools.Instance.GetMoveDir(transform, TargetMoveIndex, Clockwise);
        ModelControl.Move();

        int tragetIndex = Clockwise ? TargetMoveIndex + 1 : TargetMoveIndex;
        transform.position = Vector3.MoveTowards(transform.position, PointsTools.Instance.GetPointPos(tragetIndex), Time.deltaTime * GhostSpeed);
    }
    #endregion

    void OnTriggerEnter(Collider _collder)
    {
        if (NowControlType != ControlType.Run || ModelType == PlayerType.Cat || CanControl == false)
            return;

        var player = _collder.GetComponent<PlayerControl>();

        if (player != null && player.ModelType == PlayerControl.PlayerType.Cat && player.InChangeCD == false)
        {
            ClientCommand.identity.id = 3;
            ClientCommand.identity.running = transform.position;
            ClientCommand.identity.runningForward = transform.forward;
            ClientCommand.identity.gotchaId = player.ConnectionId;
            GM.services.Send();

            ClientCommand.identity.gotchaId = 0;
        }
    }
}
