using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;

public class ImServices : MonoBehaviour
{
    public const short MSG_COMMAND = short.MaxValue;
    public const short MSG_GAME = MSG_COMMAND - 1;
    public const short MSG_GAME_END = MSG_COMMAND - 2;

    public static Dictionary<int, ClientSyncEvent> players = new Dictionary<int, ClientSyncEvent>();

    public bool isHost { get { return reqMaster; } }

    private NetworkClient client = null;
    private NetworkMatch match = null;
    private MatchInfo matchInfo = null;
    private int playGameNum = 0;
    private int playTimeMax = 0;
    private float playTimeOver = 0f;
    private int reqDomain = 0;
    private bool reqMaster = false;

    public void Connect()
    {
        if(reqMaster)
        {
            match.CreateMatch("default", (uint)playGameNum + 1, true, "", "", "", 0, reqDomain, OnMatchCreate);
        }
        else
        {
            if(client != null)
            {
                ClientCommand.identity.catId = GM.configs["貓咪"];
                ClientCommand.identity.id = 0;
                QA.Invoke<int>("Connected", ClientCommand.identity.connectionId);
                client.Send(ImServices.MSG_COMMAND, ClientCommand.identity);
            }
            else
            {
                match.ListMatches(0, 10, "", false, 0, reqDomain, OnMatchList);
            }
        }
    }

    public void Disconnect()
    {
        if(reqMaster)
        {
            if(matchInfo != null)
            {
                match.DestroyMatch(matchInfo.networkId, reqDomain, OnMatchDestroy);
                matchInfo = null;
            }
        }
        else
        {
            if(client != null)
            {
                client.Shutdown();
                client = null;
            }
        }
    }

    public void Send()
    {
        if(reqMaster == false && client != null)
        {
            client.Send(ImServices.MSG_COMMAND, ClientCommand.identity);
        }
    }

    private void Awake()
    {
        int index = 0;
        match = gameObject.AddComponent<NetworkMatch>();
        playGameNum = GM.configs.TryGetValue("地圖人數", out index) && index != 0 ? index : 1;
        playTimeMax = GM.configs.TryGetValue("地圖時間", out index) && index != 0 ? index * 60 : 30;
        reqDomain = GM.configs.TryGetValue("地圖", out index) ? index : 0;
        reqMaster = GM.configs.TryGetValue("SERVER", out index) ? index != 0 : false;
    }

    private void OnCommandFromClient(NetworkMessage netMsg)
    {
        netMsg.ReadMessage<ClientCommand>(ClientCommand.identity);
        ClientCommand.identity.Apply(ClientSyncEvent.identity);
        if(ClientCommand.identity.id == 0)
        {
            int connectionId = netMsg.conn.connectionId;
            ClientSyncEvent connectionSyncEvent = ClientSyncEvent.identity;
            ClientSyncEvent.identity = new ClientSyncEvent();
            if(playTimeOver > 0f)
            {
                foreach(KeyValuePair<int, ClientSyncEvent> pair in ImServices.players)
                {
                    int playingId = pair.Key;
                    ClientSyncEvent playingSyncEvent = pair.Value;
                    NetworkServer.SendToClient(connectionId, ImServices.MSG_COMMAND, playingSyncEvent);
                    NetworkServer.SendToClient(playingId, ImServices.MSG_COMMAND, connectionSyncEvent);
                }
                ImServices.players[connectionId] = connectionSyncEvent;
                float time = playTimeOver - Time.time;
                NetworkServer.SendToClient(connectionId, ImServices.MSG_COMMAND, connectionSyncEvent);
                NetworkServer.SendToClient(connectionId, ImServices.MSG_GAME, new IntegerMessage(time > 0f ? (int)time : 0));
            }
            else
            {
                ImServices.players[connectionId] = connectionSyncEvent;
                List<ClientSyncEvent> list = new List<ClientSyncEvent>(ImServices.players.Values);
                if(list.Count == playGameNum)
                {
                    for(int i = 0; i < list.Count; ++i)
                    {
                        NetworkServer.SendToAll(ImServices.MSG_COMMAND, list[i]);
                    }
                    NetworkServer.SendToAll(ImServices.MSG_GAME, new IntegerMessage(playTimeMax));
                    playTimeOver = Time.time + playTimeMax;
                }
                Debug.LogWarning("Player: " + list.Count);
            }
        }
        else
        {
            NetworkServer.SendToAll(ImServices.MSG_COMMAND, ClientSyncEvent.identity);
        }
    }

    private void OnCommandFromServer(NetworkMessage netMsg)
    {
        netMsg.ReadMessage<ClientSyncEvent>(ClientSyncEvent.identity);
        QA.Invoke<ClientSyncEvent>("Event", ClientSyncEvent.identity);
    }

    private void OnConnected(NetworkMessage netMsg)
    {
        Debug.LogWarning("Connected !");
        if(reqMaster)
        {
        }
        else
        {
            QA.Invoke<int>("Connected", ClientCommand.identity.connectionId);
            client.Send(ImServices.MSG_COMMAND, ClientCommand.identity);
        }
    }

    private void OnDisonnected(NetworkMessage netMsg)
    {
        Debug.LogWarning("Disconnected !");
        if(reqMaster)
        {
            ImServices.players.Remove(netMsg.conn.connectionId);
        }
        else
        {
            if(client != null)
            {
                client = null;
            }
            QA.Invoke("Disconnected");
            Invoke("Connect", 1f);
        }
    }

    private void OnGameFromServer(NetworkMessage netMsg)
    {
        GM.time = netMsg.ReadMessage<IntegerMessage>().value;
        QA.Invoke("Game");
    }

    private void OnGameEndFromServer(NetworkMessage netMsg)
    {
        QA.Invoke<int>("GameEnd", netMsg.ReadMessage<IntegerMessage>().value);
    }

    private void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if(success)
        {
            Debug.LogWarning("Services !");
            this.matchInfo = matchInfo;
            NetworkServer.Listen(matchInfo, 443);
            NetworkServer.RegisterHandler(MsgType.Connect, OnConnected);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnDisonnected);
            NetworkServer.RegisterHandler(ImServices.MSG_COMMAND, OnCommandFromClient);
            Utility.SetAccessTokenForNetwork(matchInfo.networkId, matchInfo.accessToken);
        }
        else
        {
            Debug.LogError(extendedInfo);
        }
    }

    private void OnMatchCreateJoin(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if(success)
        {
            this.matchInfo = matchInfo;
            ClientCommand.identity.catId = GM.configs["貓咪"];
            ClientCommand.identity.connectionId = (int)matchInfo.nodeId;
            ClientCommand.identity.id = 0;
            Utility.SetAccessTokenForNetwork(matchInfo.networkId, matchInfo.accessToken);
            client = new NetworkClient();
            client.RegisterHandler(MsgType.Connect, OnConnected);
            client.RegisterHandler(MsgType.Disconnect, OnDisonnected);
            client.RegisterHandler(ImServices.MSG_COMMAND, OnCommandFromServer);
            client.RegisterHandler(ImServices.MSG_GAME, OnGameFromServer);
            client.RegisterHandler(ImServices.MSG_GAME_END, OnGameEndFromServer);
            client.Connect(matchInfo);
        }
        else
        {
            Debug.LogError(extendedInfo);
        }
    }

    private void OnMatchDestroy(bool success, string extendedInfo)
    {
        if(success)
        {
            Debug.LogWarning("Destroyed !");
            GM.currentLuckyCatId = -1;
            ImServices.players.Clear();
            playTimeOver = 0f;
        }
        else
        {
            Debug.LogError(extendedInfo);
        }
    }

    private void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        if(success)
        {
            bool isMatch = false;
            if(matchList.Count != 0)
            {
                for(int i = 0; i < matchList.Count; ++i)
                {
                    if(matchList[i].currentSize < matchList[i].maxSize)
                    {
                        isMatch = true;
                        match.JoinMatch(matchList[i].networkId, "", "", "", 0, reqDomain, OnMatchCreateJoin);
                    }
                }
            }
            if(isMatch == false)
            {
                Invoke("Connect", 1f);
            }
        }
        else
        {
            Debug.LogError(extendedInfo);
            Invoke("Connect", 3f);
        }
    }

    private void Update()
    {
        if(reqMaster)
        {
            if(playTimeOver > 0f)
            {
                if(Time.time > playTimeOver && GM.currentLuckyCatId > 0)
                {
                    NetworkServer.SendToAll(ImServices.MSG_GAME_END, new IntegerMessage(GM.currentLuckyCatId));
                    GM.currentLuckyCatId = -1;
                    ImServices.players.Clear();
                    playTimeOver = 0f;
                }
            }
        }
        else
        {
            float deltaTime = Time.deltaTime;
            GM.time = GM.time > deltaTime ? GM.time - deltaTime : 0f;
        }
    }
}
