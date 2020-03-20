using UnityEngine;
using UnityEngine.Networking;

public class ClientCommand : MessageBase
{

    public static ClientCommand identity = new ClientCommand();

    public int catId = 0;
    public int connectionId = 0;
    public int gotchaId = 0;
    public int id = 0;
    public int mapping = 0;
    public float fVaule = 0f;
    public Vector3 running = Vector3.zero;
    public Vector3 runningForward = Vector3.zero;

    public float CatChangeCDTimer = 0f;
    const float ChangeCDTime = 3f;

    public void Apply(ClientSyncEvent syncEvent)
    {
        switch(id)
        {
            case 0:
                // initial
                syncEvent.animId = 0;
                syncEvent.catId = catId;
                syncEvent.connectionId = connectionId;
                syncEvent.isLucky = false;
                syncEvent.mapping = Random.Range(1, 5);
                syncEvent.fvaule = fVaule;
                syncEvent.running = Vector3.zero;
                syncEvent.runningForward = Vector3.zero;
                break;
            case 1:
                // idle
                syncEvent.animId = 1;
                syncEvent.catId = catId;
                syncEvent.connectionId = connectionId;
                syncEvent.isLucky = connectionId == GM.currentLuckyCatId;
                syncEvent.mapping = mapping;
                syncEvent.fvaule = fVaule;
                syncEvent.running = running;
                syncEvent.runningForward = runningForward;
                break;
            case 2:
                // running
                syncEvent.animId = 2;
                syncEvent.catId = catId;
                syncEvent.connectionId = connectionId;
                syncEvent.isLucky = connectionId == GM.currentLuckyCatId;
                syncEvent.mapping = mapping;
                syncEvent.fvaule = fVaule;
                syncEvent.running = running;
                syncEvent.runningForward = runningForward;
                break;
            case 3:
                // jumping && Run
                if(Time.time >= CatChangeCDTimer && GM.currentLuckyCatId == gotchaId)
                {
                    CatChangeCDTimer = Time.time + ChangeCDTime;

                    GM.currentLuckyCatId = connectionId;
                    syncEvent.mapping = Random.Range(1, 5);
                }
                else
                    syncEvent.mapping = mapping;

                syncEvent.animId = 3;
                syncEvent.catId = catId;
                syncEvent.connectionId = connectionId;
                syncEvent.isLucky = connectionId == GM.currentLuckyCatId;
                syncEvent.running = running;
                syncEvent.runningForward = runningForward;
                syncEvent.fvaule = fVaule;
                break;
            case 4:
                // Rotate
                syncEvent.animId = 4;
                syncEvent.mapping = mapping;
                syncEvent.catId = catId;
                syncEvent.connectionId = connectionId;
                syncEvent.isLucky = connectionId == GM.currentLuckyCatId;
                syncEvent.fvaule = fVaule;
                syncEvent.running = running;
                syncEvent.runningForward = runningForward;
                break;
            default:
                Debug.LogError("undefined " + id);
                break;
        }
    }

    public override void Deserialize(NetworkReader reader)
    {
        catId = reader.ReadInt32();
        connectionId = reader.ReadInt32();
        gotchaId = reader.ReadInt32();
        mapping = reader.ReadInt32();
        id = reader.ReadInt32();
        running = reader.ReadVector3();
        runningForward = reader.ReadVector3();
        fVaule = reader.ReadSingle();
    }

    public override void Serialize(NetworkWriter writer)
    {
        writer.Write(catId);
        writer.Write(connectionId);
        writer.Write(gotchaId);
        writer.Write(mapping);
        writer.Write(id);
        writer.Write(running);
        writer.Write(runningForward);
        writer.Write(fVaule);
    }
}
