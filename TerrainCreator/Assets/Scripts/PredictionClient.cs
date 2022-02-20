using System;
using UnityEngine;

public class PredictionClient : MonoBehaviour
{
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
        predictionRequester.Intitialize();
    }

    public void Predict(byte[] input, Action<byte[]> onOutputReceived, Action<Exception> fallback)
    {
        predictionRequester.SetOnTextReceivedListener(onOutputReceived, fallback);
        predictionRequester.SendInput(input);
    }
}
