using Unity.Collections;
using Unity.Networking.Transport;
public class KeepAlive : NetworkMessage
{
    public KeepAlive()
    {
        Code = OpCode.KeepAlive;
    }
    public KeepAlive(DataStreamReader reader)
    {
        Code = OpCode.KeepAlive;
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
        NetUtility.C_KEEP_ALIVE?.Invoke(this);
    }
    public override void ReceiveOnServer(NetworkConnection cnn)
    {
        NetUtility.S_KEEP_ALIVE?.Invoke(this, cnn);
    }
}
