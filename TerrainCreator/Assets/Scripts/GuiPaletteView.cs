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
        this.OnPenWidthChanged(penWidth.value);
        this.OnPenColourChanged(Color.red, 5);
    }

    void Update()
    {

    }

    private void OnEnable()
    {
        if (!Directory.Exists(this.SaveDirectory))
        {
            this.SaveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        this.Swatch1.ColourChanged += this.OnPenColourChanged;
        this.Swatch2.ColourChanged += this.OnPenColourChanged;
        this.Swatch3.ColourChanged += this.OnPenColourChanged;
        this.Swatch4.ColourChanged += this.OnPenColourChanged;
        this.Swatch5.ColourChanged += this.OnPenColourChanged;
        this.Swatch6.ColourChanged += this.OnPenColourChanged;
        this.Swatch7.ColourChanged += this.OnPenColourChanged;
        this.Swatch8.ColourChanged += this.OnPenColourChanged;
        this.Swatch9.ColourChanged += this.OnPenColourChanged;
        this.Swatch10.ColourChanged += this.OnPenColourChanged;

        this.penWidth.onValueChanged.AddListener(this.OnPenWidthChanged);
        this.eraser.onValueChanged.AddListener(this.OnEraserToggled);
        this.constantPrediction.onValueChanged.AddListener(this.OnConstantPredictionToggled);
        this.constantTextureUpdate.onValueChanged.AddListener(this.OnConstantTextureUpdateToggled);
        this.clearButton.onClick.AddListener(this.OnClearDrawing);
        this.exportSketchButton.onClick.AddListener(this.OnExportSketchDrawing);
        this.exportHeightmapButton.onClick.AddListener(this.OnExportHeightmapDrawing);
        this.exitButton.onClick.AddListener(this.OnExit);
        this.predictionButton.onClick.AddListener(this.OnPrediction);
        this.updateTextureButton.onClick.AddListener(this.OnUpdateTexture);
        this.btnServerReconnect.onClick.AddListener(this.OnServerReconnect);
    }
    private void OnDisable()
    {
        this.Swatch1.ColourChanged -= this.OnPenColourChanged;
        this.Swatch2.ColourChanged -= this.OnPenColourChanged;
        this.Swatch3.ColourChanged -= this.OnPenColourChanged;
        this.Swatch4.ColourChanged -= this.OnPenColourChanged;
        this.Swatch5.ColourChanged -= this.OnPenColourChanged;
        this.Swatch6.ColourChanged -= this.OnPenColourChanged;
        this.Swatch7.ColourChanged -= this.OnPenColourChanged;
        this.Swatch8.ColourChanged -= this.OnPenColourChanged;
        this.Swatch9.ColourChanged -= this.OnPenColourChanged;
        this.Swatch10.ColourChanged -= this.OnPenColourChanged;

        this.penWidth.onValueChanged.RemoveListener(this.OnPenWidthChanged);
        this.eraser.onValueChanged.RemoveListener(this.OnEraserToggled);
        this.constantPrediction.onValueChanged.RemoveListener(this.OnConstantPredictionToggled);
        this.clearButton.onClick.RemoveListener(this.OnClearDrawing);
        this.exportSketchButton.onClick.RemoveListener(this.OnExportSketchDrawing);
        this.exportHeightmapButton.onClick.RemoveListener(this.OnExportHeightmapDrawing);
        this.constantTextureUpdate.onValueChanged.RemoveListener(this.OnConstantTextureUpdateToggled);
        this.exitButton.onClick.RemoveListener(this.OnExit);
        this.predictionButton.onClick.RemoveListener(this.OnPrediction);
        this.updateTextureButton.onClick.RemoveListener(this.OnUpdateTexture);
        this.btnServerReconnect.onClick.RemoveListener(this.OnServerReconnect);
    }

    private void OnPenColourChanged(Color32 colour, int number)
    {
        this.MouseDrawComponent.SetPenColour(colour, number);
        this.eraser.isOn = false;
        this.OnEraserToggled(false);
    }

    private void OnPenWidthChanged(float value)
    {
        this.MouseDrawComponent.SetPenRadius((int)value);
    }

    private void OnEraserToggled(bool value)
    {
        this.MouseDrawComponent.IsEraser = value;
    }

    private void OnConstantPredictionToggled(bool value)
    {
        this.MouseDrawComponent.IsConstantDrawPrediction = value;
    }

    private void OnConstantTextureUpdateToggled(bool value)
    {
        this.MouseDrawComponent.IsConstantTextureUpdate = value;
    }

    private void OnClearDrawing()
    {
        this.MouseDrawComponent.ClearTexture();
    }

    private void OnExportSketchDrawing()
    {
        this.MouseDrawComponent.ExportSketch(SaveDirectory, FileName);
    }

    private void OnExportHeightmapDrawing()
    {
        this.MouseDrawComponent.ExportHeightmap(SaveDirectory, FileName);
    }

    private void OnPrediction()
    {
        this.MouseDrawComponent.Predict();
    }

    private void OnServerReconnect()
    {
        this.MouseDrawComponent.ServerReconnect();
    }

    private void OnUpdateTexture()
    {
        this.MouseDrawComponent.UpdateTexture();
    }

    private void OnExit()
    {
        Application.Quit();
    }
}
