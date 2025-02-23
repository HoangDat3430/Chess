using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;


public class NetworkMessage
{
    public OpCode Code { get; set; }

    public virtual void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
    }

    public virtual void Deserialize(DataStreamReader reader)
    {
    }
    public virtual void ReceiveOnClient()
    {
    }
    public virtual void ReceiveOnServer(NetworkConnection cnn)
    {
    }
}
