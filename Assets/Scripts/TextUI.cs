using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUI : MonoBehaviour
{
    public Text text;
    public float LerpDuration = 0.5f;

    private Vector3 targetPosition;
    private float targetAlpha = 0;
    private float currentAlpha = 0;
    private float initialAlpha = 0;
    private float startLerp;
    private bool lerpNow;

    private void Update()
    {
        if (lerpNow) { 
            float Progress = Time.time - startLerp;
            Color textcolor = text.color;
            textcolor.a = Mathf.Lerp(initialAlpha, targetAlpha, Progress / LerpDuration);
            currentAlpha = textcolor.a;
            text.color = textcolor;
            if (LerpDuration < Progress)
            {
                lerpNow = false;
            }
        }

        text.transform.position = Camera.main.WorldToScreenPoint(targetPosition);
    }

    public void EnableText(Vector3 FireplacePosition)
    {
        lerpNow = true;
        startLerp = Time.time;
        initialAlpha = currentAlpha;
        targetAlpha = 1;

        targetPosition = FireplacePosition;
    }

    public void DisableText()
    {
        lerpNow = true;
        startLerp = Time.time;
        initialAlpha = currentAlpha;
        targetAlpha = 0;
    }
}
