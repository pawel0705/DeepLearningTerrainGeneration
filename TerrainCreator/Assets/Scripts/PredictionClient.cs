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
        this.InitializeServer();
    }

    void Update()
    {
        this.predictionRequester.UpdateResult();
    }

    public void InitializeServer()
    {
        this.predictionRequester = new PredictionRequester();
        this.predictionRequester.loading = loading;
        this.predictionRequester.Intitialize();
    }

    public void Predict(byte[] input, Action<byte[]> onOutputReceived, Action<Exception> fallback)
    {
        this.predictionRequester.SetOnTextReceivedListener(onOutputReceived, fallback);
        this.predictionRequester.SendInput(input);
    }

    public void ServerReconnect()
    {
        this.predictionRequester.ServerReconnect();
    }

    void OnApplicationQuit()
    {
        this.predictionRequester.ClearConnection();
    }
}
