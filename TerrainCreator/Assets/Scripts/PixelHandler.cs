using UnityEngine;
using UnityEngine.UI;

public class PixelHandler : MonoBehaviour
{
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        this.image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public float GetColor()
    {
        return image.color.r;
    }

    public void ChanePixelColor(float color)
    {
        if (color >= 1)
        {
            color = 1;
        }
        else if (color <= 0)
        {
            color = 0;
        }

        image.color = new Color(color, color, color);
    }
}
