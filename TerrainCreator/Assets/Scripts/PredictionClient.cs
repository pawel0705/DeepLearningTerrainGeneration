﻿using System;
using UnityEngine;

public class PredictionClient : MonoBehaviour
{
    private PredictionRequester predictionRequester;

    // Start is called before the first frame update
    void Start()
    {
        InitializeServer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeServer()
    {
        predictionRequester = new PredictionRequester();
        predictionRequester.Start();
    }

    public void Predict(byte[] input, Action<float[]> onOutputReceived, Action<Exception> fallback)
    {
        predictionRequester.SetOnTextReceivedListener(onOutputReceived, fallback);
        predictionRequester.SendInput(input);
    }

    private void OnDestroy()
    {
        predictionRequester.Stop();
    }
}