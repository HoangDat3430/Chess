using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System;

public class Server : MonoBehaviour
{
    public static Server Instance { get; private set; }
    private NetworkDriver driver;
    private NativeList<NetworkConnection> connections;

    private const float KEEP_ALIVE_INTERVAL = 20.0f;
    private float lastKeepAlive;
    private bool isActive;

    public const ushort serverPort = 7777;
    public const string ipAddress = "127.0.0.1";

    private Action connectionDropped;

    private int rematchRequest = 0;
    private void Awake()
    {
        Instance = this;
    }
    public void Init()
    {
        driver = NetworkDriver.Create();
        NetworkEndpoint endpoint = NetworkEndpoint.AnyIpv4.WithPort(serverPort);
        if (driver.Bind(endpoint) != 0)
        {
            Debug.Log("Failed to bind to port " + serverPort);
        }
        else
        {
            driver.Listen();
        }
        connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
        isActive = true;
        RegisterToEvent();
        Debug.Log("Server started on port " + serverPort); 
    }
    public void ShutDown()
    {
        if (isActive)
        {
            UnRegisterToEvent();
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
        KeepAlive(); 

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
            NetWelcome msg = new NetWelcome();
            msg.team = connections.Length-1;
            SendToClient(c, msg);
            if(connections.Length == 2)
            {
                BroadCast(new NetStartGame());
            }
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
                    NetUtility.OnData(stream, connections[i], this);
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
    private void KeepAlive()
    {
        if(Time.time - lastKeepAlive > KEEP_ALIVE_INTERVAL)
        {
            lastKeepAlive = Time.time;
            BroadCast(new NetKeepAlive());
        }
    }
    public void SendToClient(NetworkConnection connection, NetworkMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);  
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
    private void OnMoveReq(NetworkMessage msg, NetworkConnection cnn)
    {
        BroadCast(msg);
    }
    private void OnPromoteReq(NetworkMessage msg, NetworkConnection cnn)
    {
        BroadCast(msg);
    }
    private void OnShowResultReq(NetworkMessage msg, NetworkConnection cnn)
    {
        BroadCast(msg);
    }
    private void OnRematchReq(NetworkMessage msg, NetworkConnection cnn)
    {
        rematchRequest++;
        if(rematchRequest == 2)
        {
            BroadCast(msg);
            rematchRequest = 0;
        }
    }
    private void RegisterToEvent()
    {
        NetUtility.S_MOVE += OnMoveReq;
        NetUtility.S_PROMOTE += OnPromoteReq;
        NetUtility.S_RESULT += OnShowResultReq;
        NetUtility.S_REMATCH += OnRematchReq;
    }
    private void UnRegisterToEvent()
    {
        NetUtility.S_MOVE -= OnMoveReq;
        NetUtility.S_PROMOTE -= OnPromoteReq;
        NetUtility.S_RESULT -= OnShowResultReq;
        NetUtility.S_REMATCH -= OnRematchReq;
    }
}
