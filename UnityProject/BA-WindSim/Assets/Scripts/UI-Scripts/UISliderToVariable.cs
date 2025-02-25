using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISliderToVariable : MonoBehaviour
{
    public Slider mainSlider;
    public float sliderValue;
    public float sMax;
    public float sMin;

    public Fluid fluid;

    void Start()
    {
        mainSlider.maxValue = sMax;
        mainSlider.minValue = sMin;
        mainSlider.value = sliderValue;

        mainSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void OnSliderValueChanged(float value)
    {
        sliderValue = value;
        fluid.globalWindStrenght = value;
    }
}
