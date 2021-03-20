using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    public Image fillImage;
    public Image tintImage;

    [Range(0,1)]
    public float value = 1;

    DoTweenImageFiller fillFiller;
    DoTweenImageFiller tintFiller;

    private void Awake()
    {
        if (fillImage)
            fillFiller = new DoTweenImageFiller(fillImage);
        if (tintImage)
            tintFiller = new DoTweenImageFiller(tintImage);
    }

    private void OnDestroy()
    {
        fillFiller?.Stop();
        tintFiller?.Stop();
    }

    public void SetValue(float value)
    {
        this.value = Mathf.Clamp01(value);

        if (fillImage)
            fillImage.fillAmount = value;
        if (tintImage)
            tintImage.fillAmount = value;
    }

    public void SetValue(float value, float fillTime, float tintTime)
    {
        this.value = Mathf.Clamp01(value);

        fillFiller?.Fill(fillImage.fillAmount, value, fillTime);
        tintFiller?.Fill(tintImage.fillAmount, value, tintTime);
    }
}
