using Unity.Collections;
using Unity.Networking.Transport;

public class NetRematch : NetworkMessage
{
    public NetRematch()
    {
        Code = OpCode.Rematch;
    }
    public NetRematch(DataStreamReader reader)
    {
        Code = OpCode.Rematch;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);
    }
    public override void Deserialize(DataStreamReader reader)
    {
    }
    public override void ReceiveOnClient()
    {
        NetUtility.C_REMATCH?.Invoke(this);
    }
    public override void ReceiveOnServer(NetworkConnection cnn)
    {
        NetUtility.S_REMATCH?.Invoke(this, cnn);
    }
}
