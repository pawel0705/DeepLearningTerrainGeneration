using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIToggler : MonoBehaviour
{
    [SerializeField]
    private GameObject drawing, heightmap, paletteGUI;

    [SerializeField]
    private Toggle showDrawing, showHeightmap, showPaletteGUI;

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnEnable()
    {
        this.showPaletteGUI.onValueChanged.AddListener(OnTogglePaletteGUI);
        this.showDrawing.onValueChanged.AddListener(OnDrawingToggle);
        this.showHeightmap.onValueChanged.AddListener(OnHeightmapToggle);
    }
    private void OnDisable()
    {
        this.showPaletteGUI.onValueChanged.RemoveListener(OnTogglePaletteGUI);
        this.showDrawing.onValueChanged.RemoveListener(OnDrawingToggle);
        this.showHeightmap.onValueChanged.RemoveListener(OnHeightmapToggle);
    }

    private void OnTogglePaletteGUI(bool value)
    {
        this.paletteGUI.SetActive(value);
    }

    private void OnDrawingToggle(bool value)
    {
        this.drawing.SetActive(value);
    }

    private void OnHeightmapToggle(bool value)
    {
        this.heightmap.SetActive(value);
    }
}
