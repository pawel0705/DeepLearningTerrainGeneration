using NetMQ;
using System;
using UnityEngine;

public class PredictionClient : MonoBehaviour
{
    [SerializeField]
    private GameObject loading;

    private PredictionRequester predictionRequester;

    void Start()
    {
        InitializeServer();
    }

    void Update()
    {
        predictionRequester.UpdateResult();
    }

    public void InitializeServer()
    {
        predictionRequester = new PredictionRequester();
        predictionRequester.loading = loading;
        predictionRequester.Intitialize();
    }

    public void Predict(byte[] input, Action<byte[]> onOutputReceived, Action<Exception> fallback)
    {
        predictionRequester.SetOnTextReceivedListener(onOutputReceived, fallback);
        predictionRequester.SendInput(input);
    }


    public void ServerReconnect()
    {
        predictionRequester.ServerReconnect();
    }

    void OnApplicationQuit()
    {
        this.predictionRequester.ClearConnection();
    }
}
