using Unity.Collections;
using Unity.Networking.Transport;

public class NetShowResult : NetworkMessage
{
    public int teamWin;
    public NetShowResult()
    {
        Code = OpCode.Result;
    }
    public NetShowResult(DataStreamReader reader)
    {
        Code = OpCode.Result;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);
        writer.WriteInt(teamWin);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        teamWin = reader.ReadInt();
    }
    public override void ReceiveOnClient()
    {
        NetUtility.C_RESULT?.Invoke(this);
    }
    public override void ReceiveOnServer(NetworkConnection cnn)
    {
        NetUtility.S_RESULT?.Invoke(this, cnn);
    }
}
