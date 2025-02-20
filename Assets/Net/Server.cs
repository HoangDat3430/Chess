using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System.Net;
using System;

public class Server : MonoBehaviour
{
    public static Server Instance { get; private set; }
    private NetworkDriver driver;
    private NativeList<NetworkConnection> connections;
    private bool isActive;

    private Action connectionDropped;
    private void Awake()
    {
        Instance = this;
    }
    public void Init(ushort port)
    {
        driver = NetworkDriver.Create();
        NetworkEndpoint endpoint = NetworkEndpoint.AnyIpv4;
        if (driver.Bind(endpoint) != 0)
        {
            Debug.Log("Failed to bind to port " + port);
        }
        else
        {
            driver.Listen();
        }
        connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
        isActive = true;
    }
    public void ShutDown()
    {
        if (isActive)
        {
            driver.Dispose();
            connections.Dispose();
            isActive = false;
        }
    }
    public void OnDestroy()
    {
        ShutDown();
    }
    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        driver.ScheduleUpdate().Complete();
        CleanUpConnections();
        AcceptNewConnections();
        UpdateMessagePump();
    }
    private void CleanUpConnections()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }
    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
        }
    }
    private void UpdateMessagePump()
    {
        DataStreamReader stream;
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                continue;
            }
            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    connections[i] = default(NetworkConnection);
                    connectionDropped?.Invoke();
                    ShutDown();
                }
            }
        }
    }
    public void SendToClient(NetworkConnection connection, NetworkMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        driver.EndSend(writer);
    }
    public void BroadCast(NetworkMessage msg)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i].IsCreated)
            {
                SendToClient(connections[i], msg);
            }
        }
    }
}
