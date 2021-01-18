using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] Slider intersectionCap;
    [SerializeField] Slider extensionValue;
    [SerializeField] Slider deviationValue;
    [SerializeField] VisualizationGenerator visualization;

    void Start()
    {
    }

    public void drawCity()
    {
        SetFields();
        visualization.DrawNewCity();
    }

    private void SetFields()
    {
        visualization.intersectionCap = intersectionCap.value;
        visualization.extensionRange = extensionValue.value;
        visualization.crossingRange = deviationValue.value;
    }
}
