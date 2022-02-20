using System;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

public class PredictionRequester
{
    public RequestSocket client;

    private Action<byte[]> onOutputReceived;
    private Action<Exception> onFail;

    private bool canSendNext = true;
    private int cantSendIterator = 0;
    private int cantSendMax = 25;

    private string connectionString = "tcp://localhost:5555";


    public void Intitialize()
    {
        RequestSocket client = new RequestSocket();

        this.client = client;
        client.Connect(connectionString);
    }

    public void UpdateResult()
    {
        byte[] outputBytes = new byte[0];
        bool gotMessage = false;

        if (canSendNext == false)
        {
            try
            {
                gotMessage = client.TryReceiveFrameBytes(out outputBytes);  // this returns true if it's successful
            }
            catch (Exception e)
            {
                // TODO
            }
        }

        if (gotMessage)
        {
            Debug.Log("GotMessage");
            onOutputReceived?.Invoke(outputBytes);
            canSendNext = true;
            cantSendIterator = 0;
        }
    }

    public void SendInput(byte[] input)
    {
        try
        {
            if(cantSendIterator > cantSendMax)
            {
                cantSendIterator = 0;
                canSendNext = true;
            }

            if (canSendNext == true)
            {
                canSendNext = false;
                client.SendFrame(input);
            }
            else
            {
                cantSendIterator++;
            }
        }
        catch (Exception ex)
        {
            onFail(ex);
            client.Close();
            client = new RequestSocket();
            client.Connect(connectionString);
            cantSendIterator = 0;
        }
    }

    public void SetOnTextReceivedListener(Action<byte[]> onOutputReceived, Action<Exception> fallback)
    {
        this.onOutputReceived = onOutputReceived;
        onFail = fallback;
    }
}
