using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public float MaxValue;
    public float CurrentValue;
    public float LerpDuration = 1f;
    private float InitialValue;
    private bool lerpNow;

    private float startLerp;

    public Image CurrentValueBar;
    public Image LerpValueBar;

    void Update()
    {
        if (lerpNow)
        {
            float Progress = Time.time - startLerp;
            LerpValueBar.fillAmount = Mathf.Lerp(InitialValue/MaxValue, CurrentValue/MaxValue, Progress / LerpDuration);
            if (LerpDuration < Progress)
            {
                lerpNow = false;
            }
        }
    }

    public void ChangeCurrentValue(float newValue)
    {
        InitialValue = LerpValueBar.fillAmount * MaxValue;
        lerpNow = true;
        startLerp = Time.time;
        CurrentValue = newValue;
        CurrentValueBar.fillAmount = CurrentValue / MaxValue;
    }
    

    public void ChangeCurrentMax(float Current, float Max)
    {
        MaxValue = Max;
        CurrentValue = Current;
        CurrentValueBar.fillAmount = Current / Max;
        LerpValueBar.fillAmount = Current / Max;
    }

}
