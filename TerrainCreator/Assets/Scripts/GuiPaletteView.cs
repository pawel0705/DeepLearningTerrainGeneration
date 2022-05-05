using System;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

public class GuiPaletteView : MonoBehaviour
{
    [SerializeField]
    private MouseDraw MouseDrawComponent;

    [SerializeField]
    private ColorPalette Swatch1, Swatch2, Swatch3, Swatch4, Swatch5, Swatch6, Swatch7, Swatch8, Swatch9, Swatch10;

    [SerializeField]
    private Slider penWidth;

    [SerializeField]
    private Toggle eraser;

    [SerializeField]
    private Toggle constantPrediction, constantTextureUpdate;

    [SerializeField]
    private Button clearButton, exportSketchButton, exportHeightmapButton, exitButton, predictionButton, updateTextureButton, btnServerReconnect;

    [SerializeField]
    private string SaveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    [SerializeField]
    private string FileName = "TerrainAI";

    void Start()
    {
        OnPenWidthChanged(penWidth.value);
        OnPenColourChanged(Color.red, 5);
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (!Directory.Exists(SaveDirectory))
        {
            SaveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        Swatch1.ColourChanged += OnPenColourChanged;
        Swatch2.ColourChanged += OnPenColourChanged;
        Swatch3.ColourChanged += OnPenColourChanged;
        Swatch4.ColourChanged += OnPenColourChanged;
        Swatch5.ColourChanged += OnPenColourChanged;
        Swatch6.ColourChanged += OnPenColourChanged;
        Swatch7.ColourChanged += OnPenColourChanged;
        Swatch8.ColourChanged += OnPenColourChanged;
        Swatch9.ColourChanged += OnPenColourChanged;
        Swatch10.ColourChanged += OnPenColourChanged;

        penWidth.onValueChanged.AddListener(OnPenWidthChanged);
        eraser.onValueChanged.AddListener(OnEraserToggled);
        constantPrediction.onValueChanged.AddListener(OnConstantPredictionToggled);
        constantTextureUpdate.onValueChanged.AddListener(OnConstantTextureUpdateToggled);
        clearButton.onClick.AddListener(OnClearDrawing);
        exportSketchButton.onClick.AddListener(OnExportSketchDrawing);
        exportHeightmapButton.onClick.AddListener(OnExportHeightmapDrawing);
        exitButton.onClick.AddListener(OnExit);
        predictionButton.onClick.AddListener(OnPrediction);
        updateTextureButton.onClick.AddListener(OnUpdateTexture);
        btnServerReconnect.onClick.AddListener(OnServerReconnect);
    }
    private void OnDisable()
    {
        Swatch1.ColourChanged -= OnPenColourChanged;
        Swatch2.ColourChanged -= OnPenColourChanged;
        Swatch3.ColourChanged -= OnPenColourChanged;
        Swatch4.ColourChanged -= OnPenColourChanged;
        Swatch5.ColourChanged -= OnPenColourChanged;
        Swatch6.ColourChanged -= OnPenColourChanged;
        Swatch7.ColourChanged -= OnPenColourChanged;
        Swatch8.ColourChanged -= OnPenColourChanged;
        Swatch9.ColourChanged -= OnPenColourChanged;
        Swatch10.ColourChanged -= OnPenColourChanged;

        penWidth.onValueChanged.RemoveListener(OnPenWidthChanged);
        eraser.onValueChanged.RemoveListener(OnEraserToggled);
        constantPrediction.onValueChanged.RemoveListener(OnConstantPredictionToggled);
        clearButton.onClick.RemoveListener(OnClearDrawing);
        exportSketchButton.onClick.RemoveListener(OnExportSketchDrawing);
        exportHeightmapButton.onClick.RemoveListener(OnExportHeightmapDrawing);
        constantTextureUpdate.onValueChanged.RemoveListener(OnConstantTextureUpdateToggled);
        exitButton.onClick.RemoveListener(OnExit);
        predictionButton.onClick.RemoveListener(OnPrediction);
        updateTextureButton.onClick.RemoveListener(OnUpdateTexture);
        btnServerReconnect.onClick.RemoveListener(OnServerReconnect);
    }

    private void OnPenColourChanged(Color32 colour, int number)
    {
        MouseDrawComponent.SetPenColour(colour, number);
        eraser.isOn = false;
        OnEraserToggled(false);
    }

    private void OnPenWidthChanged(float value)
    {
        MouseDrawComponent.SetPenRadius((int)value);
    }

    private void OnEraserToggled(bool value)
    {
        MouseDrawComponent.IsEraser = value;
    }

    private void OnConstantPredictionToggled(bool value)
    {
        MouseDrawComponent.IsConstantDrawPrediction = value;
    }

    private void OnConstantTextureUpdateToggled(bool value)
    {
        MouseDrawComponent.IsConstantTextureUpdate = value;
    }

    private void OnClearDrawing()
    {
        MouseDrawComponent.ClearTexture();
    }

    private void OnExportSketchDrawing()
    {
        MouseDrawComponent.ExportSketch(SaveDirectory, FileName);
    }

    private void OnExportHeightmapDrawing()
    {
        MouseDrawComponent.ExportHeightmap(SaveDirectory, FileName);
    }

    private void OnPrediction()
    {
        MouseDrawComponent.Predict();
    }

    private void OnServerReconnect()
    {
        MouseDrawComponent.ServerReconnect();
    }

    private void OnUpdateTexture()
    {
        MouseDrawComponent.UpdateTexture();
    }

    private void OnExit()
    {
        Application.Quit();
    }
}
