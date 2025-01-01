using UnityEngine;
using UnityEngine.UI;

public class PowerupPopup : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] RectTransform sliderPos;
    [SerializeField] RawImage rightArrow;
    [SerializeField] RawImage leftArrow;
    [SerializeField] float extendRadius = .74f;
    [SerializeField] Transform realButtonPos;
    bool closing;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider.value = .74f;
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<RectTransform>().position =   realButtonPos.position;
        if (slider.value <= (1-extendRadius) / 2 + extendRadius)
        {
            /*rightArrow.enabled = true;
            leftArrow.enabled = false;*/
            closing = true;
        }
        if (slider.value > (1-extendRadius) / 2 + extendRadius)
        {
            /*rightArrow.enabled = false;
            leftArrow.enabled = true;*/
            closing = false;
        }
        if (slider.value < extendRadius)
        {
            slider.value = extendRadius;
        }
    }

    public void CloseMenu()
    {
        if (closing)
        {
            slider.value = 1;
        }
        else
        {
            //Change slider value if there are more powerups...
            slider.value = extendRadius;
        }
    }
}
