using UnityEngine;
using UnityEngine.UI;

public class PowerupPopup : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] RectTransform sliderPos;
    [SerializeField] RawImage rightArrow;
    [SerializeField] RawImage leftArrow;
    bool closing;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<RectTransform>().position = sliderPos.position;
        if (slider.value <= .5)
        {
            rightArrow.enabled = true;
            leftArrow.enabled = false;
            closing = true;
        }
        if (slider.value > .5)
        {
            rightArrow.enabled = false;
            leftArrow.enabled = true;
            closing = false;
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
            slider.value = 0;
        }
    }
}
