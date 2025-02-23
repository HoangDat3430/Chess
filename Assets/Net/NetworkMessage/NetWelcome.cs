using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetWelcome : NetworkMessage
{
    public int team;
    public NetWelcome()
    {
        Code = OpCode.Welcome;
    }
    public NetWelcome(DataStreamReader reader)
    {
        Code = OpCode.Welcome;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);
        writer.WriteInt(team);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        team = reader.ReadInt();
    }
    public override void ReceiveOnClient()
    {
        NetUtility.C_WELCOME?.Invoke(this);
    }
    public override void ReceiveOnServer(NetworkConnection cnn)
    {
        NetUtility.S_WELCOME?.Invoke(this, cnn);
    }
}
