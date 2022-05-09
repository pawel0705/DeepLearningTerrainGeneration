using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColorPalette : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public delegate void OnColourChanged(Color32 colour, int number);

    public event OnColourChanged ColourChanged;

    [SerializeField]
    private Image Swatch;

    private Image m_image;

    [SerializeField]
    public int number;

    private void Awake()
    {
        this.m_image = this.transform.GetComponent<Image>();
    }

    private void OnDestroy()
    {

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData) => SamplePixel(eventData);

    public void OnDrag(PointerEventData eventData) => SamplePixel(eventData);

    private void SamplePixel(PointerEventData eventData)
    {
        Color32 col = this.m_image.color;

        ColourChanged?.Invoke(col, number);
    }
}
