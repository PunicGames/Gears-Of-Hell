using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Debug_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsTextField;
    [SerializeField] private float refreshRate;
    private float timer;

    [SerializeField] private TextMeshProUGUI consoleTextField;
    [SerializeField] private static uint consoleCount = 3;
    private static Queue<string> msgQueue = new Queue<string>();

    public static void Print(string msg)
    {
        if (msgQueue.Count >= consoleCount)
        {
            msgQueue.Dequeue();
        }
        msgQueue.Enqueue(msg);
    }

    private void FillConsole()
    {
        string finalText = "";
        foreach (var msg in msgQueue)
        {
            finalText += msg + "\n";
        }
        consoleTextField.text = finalText;
    }

    void Update()
    {
        if (Time.unscaledTime > timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            fpsTextField.text = fps + " fps";
            timer = Time.unscaledTime + refreshRate;
        }

        FillConsole();
    }
}
