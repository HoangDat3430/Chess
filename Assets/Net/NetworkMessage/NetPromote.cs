using Unity.Collections;
using Unity.Networking.Transport;

public class NetPromote : NetworkMessage
{
    public int type;
    public int team;
    public NetPromote()
    {
        Code = OpCode.Promote;
    }
    public NetPromote(DataStreamReader reader)
    {
        Code = OpCode.Promote;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);
        writer.WriteInt(type);
        writer.WriteInt(team);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        type = reader.ReadInt();
        team = reader.ReadInt();
    }
    public override void ReceiveOnClient()
    {
        NetUtility.C_PROMOTE?.Invoke(this);
    }
    public override void ReceiveOnServer(NetworkConnection cnn)
    {
        NetUtility.S_PROMOTE?.Invoke(this, cnn);
    }
}
