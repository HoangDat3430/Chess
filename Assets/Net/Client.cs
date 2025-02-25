using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client Instance { get; private set; }

    private NetworkDriver driver;
    private NetworkConnection connection;
    private bool isActive;

    private Action connectionDropped;
    private void Awake()
    {
        Instance = this;
    }
    public void Init(string ip, ushort port)
    {
        if (ip == string.Empty) ip = Server.ipAddress;
        driver = NetworkDriver.Create();
        NetworkEndpoint endpoint = NetworkEndpoint.Parse(ip, port);
        connection = driver.Connect(endpoint);
        isActive = true;
        RegisterToEvent();
        Debug.Log("Client connected to server " + endpoint.Address);
    }
    public void ShutDown()
    {
        if (isActive)
        {
            UnregisterToEvent();
            driver.Dispose();
            isActive = false;
            connection = default(NetworkConnection);
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
        CheckAlive();
        UpdateMessagePump();
    }
    private void UpdateMessagePump()
    {
        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Data)
            {
                NetUtility.OnData(stream, connection);
            }
            else if(cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("Connected to server");
                SendToServer(new NetKeepAlive());
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                connection = default(NetworkConnection);
                connectionDropped?.Invoke();
                ShutDown();
            }
        }
    }
    private void CheckAlive()
    {
        if (!connection.IsCreated && isActive)
        {
            connectionDropped?.Invoke();
            ShutDown();
        }
    }
    public void SendToServer(NetworkMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }
    private void OnKeepAliveRes(NetworkMessage msg)
    {
        SendToServer(msg);
    }
    private void OnWelcomeRes(NetworkMessage msg)
    {
        NetWelcome netWelcome = msg as NetWelcome;
        Debug.Log("You are team: " + netWelcome.team);
        GameMgr.Instance.AssignTeam(netWelcome.team);
    }
    private void OnGameStartRes(NetworkMessage msg)
    {
        Debug.Log("Game Started!!!");
        GameMgr.Instance.StartGame(false);
    }
    private void OnMoveRes(NetworkMessage msg)
    {
        GameMgr.Instance.OnMoveRes(msg);
    }
    private void OnPromoteRes(NetworkMessage msg)
    {
        GameMgr.Instance.OnPromoteRes(msg);
    }
    private void OnShowResultRes(NetworkMessage msg)
    {
        GameMgr.Instance.OnShowResultRes((NetShowResult)msg);
    }
    private void OnRematchRes(NetworkMessage msg)
    {
        GameMgr.Instance.OnRematchRes();
    }
    private void RegisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE += OnKeepAliveRes;
        NetUtility.C_WELCOME += OnWelcomeRes;
        NetUtility.C_GAME_START += OnGameStartRes;
        NetUtility.C_MOVE += OnMoveRes;
        NetUtility.C_PROMOTE += OnPromoteRes;
        NetUtility.C_RESULT += OnShowResultRes;
        NetUtility.C_REMATCH += OnRematchRes;
        connectionDropped += GameMgr.Instance.OnOnlineBackButtonClick;
    }
    private void UnregisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE -= OnKeepAliveRes;
        NetUtility.C_WELCOME -= OnWelcomeRes;
        NetUtility.C_GAME_START -= OnGameStartRes;
        NetUtility.C_MOVE -= OnMoveRes;
        NetUtility.C_PROMOTE -= OnPromoteRes;
        NetUtility.C_RESULT -= OnShowResultRes;
        NetUtility.C_REMATCH -= OnRematchRes;
        connectionDropped -= GameMgr.Instance.OnOnlineBackButtonClick;
    }
}
