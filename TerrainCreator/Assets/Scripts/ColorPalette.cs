using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColorPalette : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField]
    private Image Swatch;

    private Image m_image;

    private void Awake()
    {
        m_image = transform.GetComponent<Image>();
    }

    private void OnDestroy()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Triggered when user is pressing mouse or finger down.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData) => SamplePixel(eventData);


    /// <summary>
    /// Triggered when user is dragging with the mouse or finger.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData) => SamplePixel(eventData);

    /// <summary>
    /// Samples the pixel colour as the given pointer position.
    /// </summary>
    /// <param name="eventData"></param>
    private void SamplePixel(PointerEventData eventData)
    {
        Color32 col = m_image.color;
        Debug.Log(col);

        ColourChanged?.Invoke(col);
    }

    public delegate void OnColourChanged(Color32 colour);
    public event OnColourChanged ColourChanged;

}
