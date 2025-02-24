using Unity.Collections;
using Unity.Networking.Transport;

public class NetMove : NetworkMessage
{
    public int turn;
    public int oriPosX;
    public int oriPosY;
    public int desPosX;
    public int desPosY;
    public int specialMove;

    public NetMove()
    {
        Code = OpCode.Move;
    }
    public NetMove(DataStreamReader reader)
    {
        Code = OpCode.Move;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);
        writer.WriteInt(turn);
        writer.WriteInt(oriPosX);
        writer.WriteInt(oriPosY);
        writer.WriteInt(desPosX);
        writer.WriteInt(desPosY);
        writer.WriteInt(specialMove);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        turn = reader.ReadInt();
        oriPosX = reader.ReadInt();
        oriPosY = reader.ReadInt();
        desPosX = reader.ReadInt();
        desPosY = reader.ReadInt();
        specialMove = reader.ReadInt();
    }
    public override void ReceiveOnClient()
    {
        NetUtility.C_MOVE?.Invoke(this);
    }
    public override void ReceiveOnServer(NetworkConnection cnn)
    {
        turn++;
        NetUtility.S_MOVE?.Invoke(this, cnn);
    }
}
