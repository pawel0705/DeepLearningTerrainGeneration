using System;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

public class PredictionRequester
{
    public RequestSocket client;

    public GameObject loading;

    private Action<byte[]> onOutputReceived;
    private Action<Exception> onFail;

    private bool canSendNext = true;

    private float timeBetweenSend = 0;
    private float maxTimeout = 3;

    private string connectionString = "tcp://localhost:5555";

    public void Intitialize()
    {
        Debug.Log("Initialize Request socket");
        this.client = new RequestSocket();

        client.Connect(connectionString);
    }

    public void UpdateResult()
    {
        byte[] outputBytes = new byte[0];
        bool gotMessage = false;

        try
        {
            gotMessage = client.TryReceiveFrameBytes(out outputBytes); 
        }
        catch (Exception ex)
        {
            // TODO
        }

        if (gotMessage == true)
        {
            Debug.Log("GotMessage");
            onOutputReceived?.Invoke(outputBytes);
            canSendNext = true;
            loading.SetActive(false);
            this.timeBetweenSend = 0f;
        }
        else
        {
            this.timeBetweenSend += Time.deltaTime;
            if(this.timeBetweenSend > this.maxTimeout)
            {
                this.timeBetweenSend = 0;
                canSendNext = true;
            }
        }
    }

    public void ServerReconnect()
    {
        this.client.Disconnect(connectionString);
        this.client.Close();
        ForceDotNet.Force();
        NetMQConfig.Cleanup(false);

        this.client = new RequestSocket();
        this.client.Connect(connectionString);
        loading.SetActive(false);
        canSendNext = true;
    }

    public void SendInput(byte[] input)
    {
        try
        {
            if (canSendNext == true)
            {
                Debug.Log("CanSendNext");
                loading.SetActive(true);
                client.SendFrame(input);
                canSendNext = false;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            onFail(ex);
            this.ServerReconnect();
        }
    }

    public void SetOnTextReceivedListener(Action<byte[]> onOutputReceived, Action<Exception> fallback)
    {
        this.onOutputReceived = onOutputReceived;
        onFail = fallback;
    }

    public void ClearConnection()
    {
        this.client.Close();
        ForceDotNet.Force();
        NetMQConfig.Cleanup(false);
    }
}
