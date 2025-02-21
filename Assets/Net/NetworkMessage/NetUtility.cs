using System;
using Unity.Collections;
using Unity.Networking.Transport;

public enum OpCode
{
    KeepAlive,
    Welcome,
    GameStart,
    Move,
    Rematch
}
public static class NetUtility
{
    public static Action<NetworkMessage> C_KEEP_ALIVE;
    public static Action<NetworkMessage> C_WELCOME;
    public static Action<NetworkMessage> C_GAME_START;
    public static Action<NetworkMessage> C_MOVE;
    public static Action<NetworkMessage> C_Rematch;
    public static Action<NetworkMessage, NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetworkMessage, NetworkConnection> S_WELCOME;
    public static Action<NetworkMessage, NetworkConnection> S_GAME_START;
    public static Action<NetworkMessage, NetworkConnection> S_MOVE;
    public static Action<NetworkMessage, NetworkConnection> S_Rematch;

    public static void OnData(DataStreamReader reader, NetworkConnection cnn, Server server = null)
    {
        NetworkMessage msg = null;
        OpCode code = (OpCode)reader.ReadByte();
        switch (code)
        {
            case OpCode.KeepAlive:
                msg = new KeepAlive(reader);
                break;
            default:
                break;
        }
        if (server == null)
        {
            msg.ReceiveOnClient();
        }
        else
        {
            msg.ReceiveOnServer(cnn);
        }
    }
}
