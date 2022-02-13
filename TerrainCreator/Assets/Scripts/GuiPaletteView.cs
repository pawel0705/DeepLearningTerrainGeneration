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
    private Button clearButton, exportButton, exitButton, predictionButton;

    [SerializeField]
    private string SaveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    [SerializeField]
    private string FileName = "MyImage";

    private void OnEnable()
    {
        if (!Directory.Exists(SaveDirectory))
            SaveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        Swatch1.ColourChanged += OnPenColourChanged;
        Swatch2.ColourChanged += OnPenColourChanged;
        Swatch3.ColourChanged += OnPenColourChanged;
        Swatch4.ColourChanged += OnPenColourChanged;

        penWidth.onValueChanged.AddListener(OnPenWidthChanged);
        eraser.onValueChanged.AddListener(OnEraserToggled);
        clearButton.onClick.AddListener(OnClearDrawing);
        exportButton.onClick.AddListener(OnExportDrawing);
        exitButton.onClick.AddListener(OnExit);
        predictionButton.onClick.AddListener(OnPrediction);
    }

    // Start is called before the first frame update
    void Start()
    {
        OnPenWidthChanged(penWidth.value);
        OnPenColourChanged(Color.red);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        Swatch1.ColourChanged -= OnPenColourChanged;
        Swatch2.ColourChanged -= OnPenColourChanged;
        Swatch3.ColourChanged -= OnPenColourChanged;
        Swatch4.ColourChanged -= OnPenColourChanged;

        penWidth.onValueChanged.RemoveListener(OnPenWidthChanged);
        eraser.onValueChanged.RemoveListener(OnEraserToggled);
        clearButton.onClick.RemoveListener(OnClearDrawing);
        exportButton.onClick.RemoveListener(OnExportDrawing);
        exitButton.onClick.RemoveListener(OnExit);
        predictionButton.onClick.RemoveListener(OnPrediction);
    }

    private void OnPenColourChanged(Color32 colour)
    {
        MouseDrawComponent.SetPenColour(colour);
    }

    private void OnPenWidthChanged(float value)
    {
        MouseDrawComponent.SetPenRadius((int)value);
    }

    private void OnEraserToggled(bool value)
    {
        MouseDrawComponent.IsEraser = value;
    }

    private void OnClearDrawing()
    {
        MouseDrawComponent.ClearTexture();
    }

    private void OnExportDrawing()
    {
        MouseDrawComponent.ExportSketch(SaveDirectory, FileName);
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
