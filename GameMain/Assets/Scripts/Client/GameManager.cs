using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
    public Action<ClientSyncEvent> OnServerEvent;
    public Action<int> OnConnected;
    public int MyID = -1;
    public bool GameStart = false;

    public PlayerControl PlayerControlPrefab;

    public Dictionary<int, Player> AllPlayer = new Dictionary<int, Player>();
    public Dictionary<int, PlayerControl> AllPlayerControl = new Dictionary<int, PlayerControl>();

    public PlayerControl C_PlayerControl;
    public PlayerLoader Loader;

    public PlayerControl AI_Control;

    public Text Timer;

    public GameObject FinalResultPanel;
    public Image WinImage;
    public Image LoseImage;
    public Button GotoLobbyBtn;
    public Transform WinModlePos;

    void Awake()
    {
        QA.Add<ClientSyncEvent>("Event", ServerEvent);
        QA.Add("Game", GameEvent);
        QA.Add<int>("GameEnd", GameEndEvent);

        if (GM.services.isHost == false)
        {
            QA.Add<int>("Connected", Connected);
            QA.Add("Disconnected", Disconnected);
        }

        GM.services.Connect();
    }

    void Update()
    {
        Timer.text = GM.time.ToString("F2");
    }

    void Start()
    {
        InitAI();
    }

    protected override void OnDestroy()
    {
        AllPlayerControl.Clear();

        QA.Remove<ClientSyncEvent>("Event", ServerEvent);
        QA.Remove<int>("Connected", Connected);
        QA.Remove("Game", GameEvent);
        QA.Remove<int>("GameEnd", GameEndEvent);

        QA.Remove<int>("Connected", Connected);
        QA.Remove("Disconnected", Disconnected);

        base.OnDestroy();
    }

    void InitAI()
    {
        var catModel = PlayerLoader.Instance.CreatePlayer(1);
        AI_Control.ControlModel = catModel;
        catModel.transform.SetParent(AI_Control.transform, false);
        catModel.transform.localPosition = Vector3.zero;

        AI_Control.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        AI_Control.IsAI = true;
        AI_Control.ConnectionId = -1;
        AI_Control.ModelType = PlayerControl.PlayerType.Cat;
        var player = catModel.GetComponent<Player>();
        player.ChangeModel(PlayerControl.PlayerType.Cat);

        AllPlayerControl.Add(AI_Control.ConnectionId, AI_Control);
    }

    void GameEvent()
    {
        Debug.LogWarning("Game Start GameEvent");
        //只有自己可以控制       
        foreach (var item in AllPlayerControl)
        {
            Debug.Log("Item : " + item.Key);
        }
        Debug.Log(" MyID : " + MyID);
        Debug.Log(" AllPlayerControl[MyID].FakeSLight is Null " + (AllPlayerControl[MyID].FakeSLight == null));

        if (AllPlayerControl.ContainsKey(MyID))
        {
            AllPlayerControl[MyID].CanControl = true;
            AllPlayerControl[MyID].FakeSLight.SetActive(true);
            AllPlayerControl[MyID].NowControlType = PlayerControl.ControlType.Move;

            GameStart = true;
            Timer.enabled = true;
        }
        else
            Debug.LogError("沒有建立自己ID的貓 !!" + MyID);
    }

    void ServerEvent(ClientSyncEvent _event)
    {
        //Debug.Log("animId : " + _event.animId + "  送出的 ID : " + _event.connectionId + " MYID : " + MyID + " : " + AllPlayerControl.Count);
        if (_event.animId == 0)
        {
            #region 建立角色
            PlayerControl control = null;

            if (AllPlayerControl.ContainsKey(_event.connectionId))
                control = AllPlayerControl[_event.connectionId];
            else
            {
                control = Instantiate<PlayerControl>(PlayerControlPrefab);
                control.gameObject.name = "PlayerID : " + _event.connectionId;

                if (_event.connectionId == MyID)
                    control.InitInput();

                control.ModelType = _event.isLucky ? PlayerControl.PlayerType.Cat : PlayerControl.PlayerType.Ghost;

                control.InitPoint(_event.mapping);

                AllPlayerControl.Add(_event.connectionId, control);
            }

            //建立 場上還沒出來的角色
            GameObject model = null;

            if (AllPlayer.ContainsKey(_event.connectionId))
                model = AllPlayer[_event.connectionId].gameObject;
            else
                model = PlayerLoader.Instance.CreatePlayer(_event.catId);

            if (model != null && control != null)
            {
                control.ConnectionId = _event.connectionId;
                control.ControlModel = model;
                control.ModelType = _event.isLucky ? PlayerControl.PlayerType.Cat : PlayerControl.PlayerType.Ghost;
                control.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

                model.transform.SetParent(control.transform, false);
                model.transform.localPosition = Vector3.zero;

                var player = model.GetComponent<Player>();
                control.ModelControl = player;
                control.NowControlType = PlayerControl.ControlType.Move;

                if (AllPlayer.ContainsKey(_event.connectionId) == false)
                    AllPlayer.Add(_event.connectionId, player);

                player.ChangeModel(control.ModelType);
            }
            #endregion
        }

        //每次 Send 都會檢查
        if (_event.animId != 0 && AllPlayerControl.ContainsKey(_event.connectionId) && AllPlayer.ContainsKey(_event.connectionId))
        {
            int nowCatID = -1;

            #region 檢查 交替
            //尋找目前場上的貓
            foreach (var player in AllPlayerControl)
            {
                if (player.Value.ModelType == PlayerControl.PlayerType.Cat)
                {
                    nowCatID = player.Key;
                    break;
                }
            }

            int newCatID = _event.isLucky ? _event.connectionId : nowCatID;

            bool isChange = nowCatID != newCatID;

            //if (isChange)
            //    Debug.Log("目前的貓ID : " + nowCatID + "  新貓ID : " + _event.connectionId + " MYID : " + MyID);

            if (isChange)
            {
                PlayerControl newCatControl = AllPlayerControl[_event.connectionId];
                PlayerControl nowCatControl = AllPlayerControl[nowCatID];

                {
                    #region 鬼 => 貓
                    //讓貓繼承 原本的路徑方向 繼續跑
                    Vector3 catPos = nowCatControl.transform.position;
                    Vector3 catforward = nowCatControl.transform.forward;

                    if (nowCatControl.IsAI && nowCatControl.gameObject.activeInHierarchy)
                    {
                        nowCatControl.gameObject.SetActive(false);
                        AllPlayerControl.Remove(-1);
                    }

                    //讓鬼 接著貓的路徑繼續奔跑
                    newCatControl.ModelType = PlayerControl.PlayerType.Cat;
                    newCatControl.transform.position = catPos;
                    newCatControl.transform.forward = catforward;
                    newCatControl.ChangeCD = Time.time + 3f;

                    AllPlayer[newCatID].ChangeModel(PlayerControl.PlayerType.Cat);

                    ShowDieEffect.Instance.CatShowDieEffect(newCatControl.transform);
                    #endregion
                }

                {
                    #region 貓 => 鬼
                    if (nowCatControl.IsAI == false)
                    {
                        nowCatControl.transform.position = _event.running;
                        nowCatControl.transform.forward = _event.runningForward;

                        nowCatControl.ModelType = PlayerControl.PlayerType.Ghost;
                        nowCatControl.InitPoint(_event.mapping - 1);
                        nowCatControl.NowControlType = PlayerControl.ControlType.Move;

                        AllPlayer[nowCatID].ChangeModel(PlayerControl.PlayerType.Ghost);
                    }
                    #endregion
                }

            #endregion
            }
        }

        if (_event.animId == 1)
        {
            if (AllPlayerControl.ContainsKey(_event.connectionId) && AllPlayer.ContainsKey(_event.connectionId))
            {
                AllPlayer[_event.connectionId].Turn();

                AllPlayerControl[_event.connectionId].NowControlType = PlayerControl.ControlType.Move;
                AllPlayerControl[_event.connectionId].Clockwise = _event.mapping == 1;

                if (_event.runningForward != Vector3.zero)
                    AllPlayerControl[_event.connectionId].transform.forward = _event.runningForward;
            }
        }

        if (_event.animId == 2)
        {
            if (AllPlayerControl.ContainsKey(_event.connectionId) && AllPlayer.ContainsKey(_event.connectionId))
            {
                AllPlayer[_event.connectionId].Move();

                if (_event.connectionId != MyID)
                    AllPlayerControl[_event.connectionId].NowControlType = PlayerControl.ControlType.Rotate;
            }
        }

        if (_event.animId == 3)
        {
            if (AllPlayerControl.ContainsKey(_event.connectionId) && AllPlayer.ContainsKey(_event.connectionId))
            {
                if (_event.isLucky)
                {
                    AllPlayerControl[_event.connectionId].transform.position = _event.running;
                    AllPlayerControl[_event.connectionId].transform.forward = _event.runningForward;

                    SpeedUp.Attach(AllPlayerControl[_event.connectionId].gameObject);
                }
                else
                {
                    if (_event.connectionId != MyID)
                    {
                        AllPlayer[_event.connectionId].Attack();

                        AllPlayerControl[_event.connectionId].OBTimer = _event.fvaule;
                        AllPlayerControl[_event.connectionId].RunFinalPoint = _event.running;
                        AllPlayerControl[_event.connectionId].NowControlType = PlayerControl.ControlType.Run;
                        AllPlayerControl[_event.connectionId].TargetMoveIndex = _event.mapping;
                    }
                }
            }
        }

        if (_event.animId == 4)
        {
            //鬼的判定
            if (_event.isLucky == false)
            {
                AllPlayerControl[_event.connectionId].NowControlType = PlayerControl.ControlType.ReadyRun;
            }
            else
            {
                AllPlayerControl[_event.connectionId].NowControlType = PlayerControl.ControlType.Rotate;
            }
            
            AllPlayerControl[_event.connectionId].GhostRotateVal = _event.fvaule;

            if (_event.running != Vector3.zero)
                AllPlayerControl[_event.connectionId].transform.position = _event.running;

            if (_event.runningForward != Vector3.zero)
                AllPlayerControl[_event.connectionId].transform.forward = _event.runningForward;

            AllPlayerControl[_event.connectionId].RandomF = _event.mapping == 1;

            AllPlayerControl[_event.connectionId].CacheForward = AllPlayerControl[_event.connectionId].transform.forward;
            AllPlayerControl[_event.connectionId].CacheRight = AllPlayerControl[_event.connectionId].transform.right;
        }
    }

    void Connected(int _int)
    {
        //只要將自己的ID確認就好
        MyID = _int;
    }

    void GameEndEvent(int _winID)
    {
        FinalResultPanel.SetActive(true);

        if (MyID == _winID)
        {
            WinImage.gameObject.SetActive(true);
            AllPlayer[MyID].Win();
            AudioPlayer.Instance.playByName("victory");
        }
        else
        {
            LoseImage.gameObject.SetActive(true);
            AudioPlayer.Instance.playByName("fail");
        }

        foreach (var player in AllPlayerControl)
        {
            if (player.Key != MyID)
                player.Value.gameObject.SetActive(false);
        }

        AllPlayerControl[MyID].enabled = false;
        AllPlayerControl[MyID].transform.SetParent(WinModlePos);
        AllPlayerControl[MyID].transform.localPosition = Vector3.zero;
        AllPlayerControl[MyID].transform.localEulerAngles = Vector3.zero;

        Camera.main.orthographicSize = 1.5f;
    }

    public void GotoLobby()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(2);
    }

    void Disconnected()
    {
        Application.Quit();
    }
}
