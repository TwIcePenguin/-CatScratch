using UnityEngine;
using UnityEngine.Networking;
public class ClientSyncEvent : MessageBase
{
    public static ClientSyncEvent identity = new ClientSyncEvent();

    public int animId = 0;
    public int catId = 0;
    public int connectionId = 0;
    public bool isLucky = false;
    public int mapping = 0;
    public float fvaule = 0f;
    public Vector3 running = Vector3.zero;
    public Vector3 runningForward = Vector3.zero;

    public override void Deserialize(NetworkReader reader)
    {
        animId = reader.ReadInt32();
        catId = reader.ReadInt32();
        connectionId = reader.ReadInt32();
        isLucky = reader.ReadBoolean();
        mapping = reader.ReadInt32();
        fvaule = reader.ReadSingle();
        running = reader.ReadVector3();
        runningForward = reader.ReadVector3();
    }

    public override void Serialize(NetworkWriter writer)
    {
        writer.Write(animId);
        writer.Write(catId);
        writer.Write(connectionId);
        writer.Write(isLucky);
        writer.Write(mapping);
        writer.Write(fvaule);
        writer.Write(running);
        writer.Write(runningForward);
    }
}
