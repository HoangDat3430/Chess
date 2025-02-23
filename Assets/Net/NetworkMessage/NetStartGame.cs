using Unity.Collections;
using Unity.Networking.Transport;

public class NetStartGame : NetworkMessage
{
    public NetStartGame()
    {
        Code = OpCode.GameStart;
    }
    public NetStartGame(DataStreamReader reader)
    {
        Code = OpCode.GameStart;
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
        NetUtility.C_GAME_START?.Invoke(this);
    }
    public override void ReceiveOnServer(NetworkConnection cnn)
    {
        NetUtility.S_GAME_START?.Invoke(this, cnn);
    }
}
