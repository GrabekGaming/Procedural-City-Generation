using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SliderText : MonoBehaviour
{
    private TMP_Text textComponent;

    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    public void SetSliderValue(float sliderValue)
    {
        textComponent.text = Mathf.Round(sliderValue).ToString();
    }
}
