using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIToggler : MonoBehaviour
{
    [SerializeField]
    private Button paletteGUIButton, drawingGUIButton;

    [SerializeField]
    private GameObject paletteGUI, drawingGUI;


    private int togglePaletteCounter = 1;

    private int toggleDrawingCounter = 1;

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
        paletteGUIButton.onClick.AddListener(OnTogglePaletteGUI);
        drawingGUIButton.onClick.AddListener(OnToggleDrawingGUI);
    }
    private void OnDisable()
    {
        paletteGUIButton.onClick.RemoveListener(OnTogglePaletteGUI);
        drawingGUIButton.onClick.RemoveListener(OnToggleDrawingGUI);
    }

    private void OnTogglePaletteGUI()
    {
        togglePaletteCounter++;

        if (togglePaletteCounter % 2 == 0)
        {
            paletteGUI.SetActive(false);
        }
        else
        {
            paletteGUI.SetActive(true);
        }
    }

    private void OnToggleDrawingGUI()
    {
        toggleDrawingCounter++;

        if (toggleDrawingCounter % 2 == 0)
        {
            drawingGUI.SetActive(false);
        }
        else
        {
            drawingGUI.SetActive(true);
        }
    }

}
