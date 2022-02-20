using System;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

public class GuiPaletteView : MonoBehaviour
{
    [SerializeField]
    private MouseDraw MouseDrawComponent;

    [SerializeField]
    private ColorPalette Swatch1, Swatch2, Swatch3, Swatch4;

    [SerializeField]
    private Slider penWidth;

    [SerializeField]
    private Toggle eraser;

    [SerializeField]
    private Toggle constantPrediction;

    [SerializeField]
    private Button clearButton, exportSketchButton, exportHeightmapButton, exitButton, predictionButton;

    [SerializeField]
    private string SaveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    [SerializeField]
    private string FileName = "TerrainAI";

    void Start()
    {
        OnPenWidthChanged(penWidth.value);
        OnPenColourChanged(Color.red);
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

        penWidth.onValueChanged.AddListener(OnPenWidthChanged);
        eraser.onValueChanged.AddListener(OnEraserToggled);
        constantPrediction.onValueChanged.AddListener(OnConstantPredictionToggled);
        clearButton.onClick.AddListener(OnClearDrawing);
        exportSketchButton.onClick.AddListener(OnExportSketchDrawing);
        exportHeightmapButton.onClick.AddListener(OnExportHeightmapDrawing);
        exitButton.onClick.AddListener(OnExit);
        predictionButton.onClick.AddListener(OnPrediction);
    }
    private void OnDisable()
    {
        Swatch1.ColourChanged -= OnPenColourChanged;
        Swatch2.ColourChanged -= OnPenColourChanged;
        Swatch3.ColourChanged -= OnPenColourChanged;
        Swatch4.ColourChanged -= OnPenColourChanged;

        penWidth.onValueChanged.RemoveListener(OnPenWidthChanged);
        eraser.onValueChanged.RemoveListener(OnEraserToggled);
        constantPrediction.onValueChanged.RemoveListener(OnConstantPredictionToggled);
        clearButton.onClick.RemoveListener(OnClearDrawing);
        exportSketchButton.onClick.RemoveListener(OnExportSketchDrawing);
        exportHeightmapButton.onClick.RemoveListener(OnExportHeightmapDrawing);
        exitButton.onClick.RemoveListener(OnExit);
        predictionButton.onClick.RemoveListener(OnPrediction);
    }

    private void OnPenColourChanged(Color32 colour)
    {
        MouseDrawComponent.SetPenColour(colour);
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

    private void OnExit()
    {
        Application.Quit();
    }
}
