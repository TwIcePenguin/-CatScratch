using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerControl : MonoSingleton<PlayerControl>
{
    public enum PlayerType
    {
        None = -1,
        Cat = 0,
        Ghost = 1,
    }

    public enum ControlType : int
    {
        None = -1,
        /// <summary> 移動 </summary>
        Move,
        /// <summary> 旋轉 / 轉向 </summary>
        Rotate,
        /// <summary> 預備衝刺 </summary>
        ReadyRun,
        /// <summary> 衝刺 </summary>
        Run,
    }

    public ControlType NowControlType = ControlType.None;

    private PlayerType mModelType = PlayerType.None;
    public PlayerType ModelType
    {
        get { return mModelType; }
        set
        {
            switch (value)
            {
                case PlayerType.Cat:
                    Rigid.isKinematic = false;
                    Collid.isTrigger = false;
                    break;
                case PlayerType.Ghost:

                    Rigid.isKinematic = true;
                    Collid.isTrigger = true;
                    break;
            }

            mModelType = value;
        }
    }

    public bool InChangeCD { get { return ChangeCD > Time.time; } }
    public float ChangeCD = 0f;
    public bool Clockwise = false;
    public int ConnectionId = 0;
    public GameObject ControlModel;

    public InputManager InputM;

    public bool BtnAIsPress = false;
    public bool BtnBIsPress = false;

    /// <summary> 鬼確定出去抓人 在 Press false 啟動 </summary>
    public bool StartRun = false;
    /// <summary> 鬼旋轉自己的角度準備抓人 </summary>
    public bool ReadyRun = false;

    public Vector3 RunFinalPoint = Vector3.zero;
    public int TargetMoveIndex = 0;
    public bool IsInitPoint = false;
    public bool CanControl = false;
    public Rigidbody Rigid;
    public SphereCollider Collid;
    public GameObject FakeSLight;

    public bool IsAI = false;

    public float CatSpeed = 3f;
    public float GhostSpeed = 5f;
    public float RoateSpeed = 360f;
    public float SpeedupCD = 0f;

    public float GhostRotateVal = 0f;

    public Player ModelControl;

    float mDeltaTime = 0.02f;
    public bool RandomF = false;

    const float MaxRange = 1;
    const float MinRange = -1;

    public Vector3 CacheForward = Vector3.zero;
    public Vector3 CacheRight = Vector3.zero;

    //衝刺硬直時間
    private const float mRunCD = 1f;
    private float mRunCDTimer = 0f;

    public float OBTimer = -1f;
    private const float OBDelay = 1f;

    void Awake()
    {
        Rigid = GetComponent<Rigidbody>();
        Collid = GetComponent<SphereCollider>();
    }

    public void InitInput()
    {
        InputM = InputManager.Instance;

        InputM.Btn_A.onPointerUp.AddListener(() => { OnBtn_A(false); });
        InputM.Btn_A.onPointerDown.AddListener(() => { OnBtn_A(true); });

        InputM.Btn_B.onPointerUp.AddListener(() => { OnBtn_B(false); });
        InputM.Btn_B.onPointerDown.AddListener(() => { OnBtn_B(true); });
    }

    public void InitPoint(int _index)
    {
        if (ModelType == PlayerType.Ghost)
        {
            TargetMoveIndex = _index;

            //初始化位置
            transform.position = PointsTools.Instance.GetPointPos(TargetMoveIndex);

            IsInitPoint = true;
        }
    }

    void OnBtn_A(bool isPress)
    {
        BtnAIsPress = isPress;

        #region Btn A
        if (BtnAIsPress)
        {
            if (ModelType == PlayerType.Ghost && NowControlType != ControlType.Move)
                return;

            SwitchClockwise();
        }
        #endregion
    }

    void OnBtn_B(bool isPress)
    {
        BtnBIsPress = isPress;
    }

    void LateUpdate()
    {
        if (GameManager.Instance.GameStart == false)
            return;

        if (IsAI)
            AIControl();
        else
        {
            if (!CanControl)
                SyncCharData(); //非控制角色的同步
            else
            {
                switch (ModelType)
                {
                    case PlayerType.Cat:
                        CatControl();
                        break;
                    case PlayerType.Ghost:
                        GhostControl();
                        break;
                }
            }
        }
    }

    /// <summary> 非操作的人同步的行為 </summary>
    void SyncCharData()
    {
        if (IsInitPoint == false)
            return;

        switch (ModelType)
        {
            case PlayerType.Cat:
                switch (NowControlType)
                {
                    case ControlType.Move:
                        CatMove();
                        break;
                    case ControlType.Rotate:
                        CatRotate();
                        break;
                    case ControlType.Run:           //衝刺加速
                        SpeedUp.Attach(gameObject);
                        NowControlType = ControlType.Move;
                        break;
                }
                break;
            case PlayerType.Ghost:
                switch (NowControlType)
                {
                    case ControlType.Move:
                        GhostMove();
                        break;
                    case ControlType.Rotate:

                        break;
                    case ControlType.ReadyRun:
                        GhostReadyRun();
                        break;
                    case ControlType.Run:
                        GhostRun();
                        break;
                }
                break;
        }
    }

    //AI簡易 讓貓往正前方移動
    void AIControl()
    {
        if (IsAI && !CanControl && ModelType == PlayerType.Cat)
        {
            CatMove();
        }
    }

    void SwitchClockwise()
    {
        ClientCommand.identity.id = 1;
        ClientCommand.identity.running = this.transform.position;
        ClientCommand.identity.runningForward = transform.forward;
        ClientCommand.identity.mapping = !Clockwise ? 1 : 0;
        GM.services.Send();
    }
}
