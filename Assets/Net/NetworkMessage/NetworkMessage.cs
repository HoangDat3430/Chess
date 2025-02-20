using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public enum OpCode
{
    CheckAlive,
    Welcome,
    GameStart,
    Move,
    Remactch
}
public class NetworkMessage
{
    public OpCode Code { get; set; }

    public virtual void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
    }

}
