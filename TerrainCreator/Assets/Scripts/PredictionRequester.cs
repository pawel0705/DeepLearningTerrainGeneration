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
        this.client = new RequestSocket();
        this.client.Connect(connectionString);
    }

    public void UpdateResult()
    {
        var outputBytes = new byte[0];
        var gotMessage = false;

        try
        {
            gotMessage = client.TryReceiveFrameBytes(out outputBytes);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        if (gotMessage == true)
        {
            this.onOutputReceived?.Invoke(outputBytes);
            this.canSendNext = true;
            this.loading.SetActive(false);
            this.timeBetweenSend = 0f;
        }
        else
        {
            this.timeBetweenSend += Time.deltaTime;

            if (this.timeBetweenSend > this.maxTimeout)
            {
                this.timeBetweenSend = 0;
                this.canSendNext = true;
            }
        }
    }

    public void ServerReconnect()
    {
        this.client.Disconnect(connectionString);
        ForceDotNet.Force();
        this.client.Close();
        NetMQConfig.Cleanup(false);

        this.client = new RequestSocket();
        this.client.Connect(connectionString);
        this.loading.SetActive(false);
        this.canSendNext = true;
    }

    public void SendInput(byte[] input)
    {
        try
        {
            if (this.canSendNext == true)
            {
                this.loading.SetActive(true);
                this.client.SendFrame(input);
                this.canSendNext = false;
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
        this.onFail = fallback;
    }

    public void ClearConnection()
    {
        ForceDotNet.Force();
        this.client.Disconnect(connectionString);
        this.client.Close();
        NetMQConfig.Cleanup(false);
    }
}
