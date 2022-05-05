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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        showPaletteGUI.onValueChanged.AddListener(OnTogglePaletteGUI);
        showDrawing.onValueChanged.AddListener(OnDrawingToggle);
        showHeightmap.onValueChanged.AddListener(OnHeightmapToggle);
    }
    private void OnDisable()
    {
        showPaletteGUI.onValueChanged.RemoveListener(OnTogglePaletteGUI);
        showDrawing.onValueChanged.RemoveListener(OnDrawingToggle);
        showHeightmap.onValueChanged.RemoveListener(OnHeightmapToggle);
    }

    private void OnTogglePaletteGUI(bool value)
    {
        this.paletteGUI.SetActive(value);
    }

    private void OnDrawingToggle(bool value)
    {
        drawing.SetActive(value);
    }

    private void OnHeightmapToggle(bool value)
    {
        heightmap.SetActive(value);
    }
}
