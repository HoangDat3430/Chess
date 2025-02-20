using System;
using System.Collections;
using System.Collections.Generic;
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
        driver = NetworkDriver.Create();
        NetworkEndpoint endpoint = NetworkEndpoint.Parse(ip, port);
        connection = driver.Connect(endpoint);
        isActive = true;
        RegisterToEvent();
    }
    public void ShutDown()
    {
        if (isActive)
        {
            UnregisterToEvent();
            driver.Dispose();
            connection = default(NetworkConnection);
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
        CheckAlive();
        UpdateMessagePump();
    }
    private void CheckAlive()
    {
        if (!connection.IsCreated && isActive)
        {
            connectionDropped?.Invoke();
            ShutDown();
        }
    }
    private void OnKeepAlive(NetworkMessage msg)
    {
        SendToServer(msg);
    }
    public void SendToServer(NetworkMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        driver.EndSend(writer);
    }
    private void UpdateMessagePump()
    {
        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Data)
            {
            }
            else if(cmd == NetworkEvent.Type.Connect)
            {

            }    
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                connection = default(NetworkConnection);
                connectionDropped?.Invoke();
                ShutDown();
            }
        }
    }
    private void RegisterToEvent()
    {

    }
    private void UnregisterToEvent()
    {
    }
}
