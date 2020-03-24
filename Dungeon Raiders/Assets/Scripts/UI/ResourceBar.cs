using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    public Image fill;

    [Range(0,1)]
    public float value = 1;

    public void SetValue(float value)
    {
        this.value = Mathf.Clamp(value,0,1);
        SetFill(this.value);
    }

    public void SetFill(float value)
    {
        fill.fillAmount = value;
    }
}
