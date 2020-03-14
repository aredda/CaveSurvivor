using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar 
    : MonoBehaviour 
{
    [Header("Composites")]
    public Image barPanel;
    public Image barIcon;
    public Image barFiller;

    private float maxValue;
    private float maxWidth;

    public void Setup(float barMaxValue)
    {
        this.maxValue = barMaxValue;
        this.maxWidth = this.barFiller.rectTransform.sizeDelta.x;
    }

    public void Setup(Color barPanelColor, Color barFillerColor, Sprite barIcon, float barMaxValue)
    {
        this.barPanel.color = barPanelColor;
        this.barFiller.color = barFillerColor;
        this.barIcon.sprite = barIcon;

        this.Setup(barMaxValue);
    }

    public void Refresh(float value)
    {
        if (value > this.maxValue)
            value = this.maxValue;
        // Calculate the matching width of this value
        float matchingWidth = value * this.maxWidth / this.maxValue;
        // Configure the size
        Vector2 size = this.barFiller.rectTransform.sizeDelta;
        size.x = matchingWidth;
        // Update size
        this.barFiller.rectTransform.sizeDelta = size;
    }
}
