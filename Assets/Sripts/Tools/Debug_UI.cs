using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Debug_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsTextField;
    [SerializeField] private float refreshRate;
    private float timer;

    void Update()
    {
        if (Time.unscaledTime > timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            fpsTextField.text = fps + " fps";
            timer = Time.unscaledTime + refreshRate;
        }

    }
}
