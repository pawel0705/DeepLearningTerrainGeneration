using System;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

public class PredictionRequester : RunAbleThread
{
    public RequestSocket client;

    private Action<byte[]> onOutputReceived;
    private Action<Exception> onFail;

    protected override void Run()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet

        
        using (RequestSocket client = new RequestSocket())
        {
            this.client = client;
            client.Connect("tcp://localhost:5555");

            while (Running)
            {
                byte[] outputBytes = new byte[0];
                bool gotMessage = false;

                while (Running)
                {

                    try
                    {
                        gotMessage = client.TryReceiveFrameBytes(out outputBytes);  // this returns true if it's successful

                        if (gotMessage)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                       // Debug.Log(e.Message);
                    }

                }

                if (gotMessage)
                {
                    onOutputReceived?.Invoke(outputBytes);
                }
            }
        }
        
        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }

    public void SendInput(byte[] input)
    {
        try
        {
           // var byteArray = new byte[input.Length * 4];
           // Buffer.BlockCopy(input, 0, byteArray, 0, byteArray.Length);
            client.SendFrame(input);
        }
        catch (Exception ex)
        {
            onFail(ex);
        }
    }

    public void SetOnTextReceivedListener(Action<byte[]> onOutputReceived, Action<Exception> fallback)
    {
        this.onOutputReceived = onOutputReceived;
        onFail = fallback;
    }
}
