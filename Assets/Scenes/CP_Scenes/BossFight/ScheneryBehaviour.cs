using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheneryBehaviour: MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] Color[] colors;

    [SerializeField] bool transitionTrigger = false;
    bool lerping = false;
    [SerializeField] float duration = 1.0f;
    private float speed = 0.02f;
    private float value = 0;

    private void Awake()
    {
        SetColor(colors[0]);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (transitionTrigger)
        {
            transitionTrigger = false;
            lerping = true;
            value = 0;
        }

        if (lerping)
            FadeColor(0, 1);
    }

    private void FadeColor(int from, int to)
    {
        if (value <= 1)
        {
            SetColor(Color.Lerp(colors[from], colors[to], value));
            value += speed / duration;
        }
        else
            lerping = false; 
        
    }

    private void SetColor(Color color)
    {
        material.SetColor("_EmissionColor", color);
    }
}
