using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SoundEmitter))]
public class SoundEmitterEditor : Editor
{
    private void OnSceneGUI()
    {
        SoundEmitter sound = (SoundEmitter)target;

        Handles.color = Color.magenta;
        Handles.DrawWireArc(sound.transform.position, sound.transform.up, sound.transform.forward, 360, sound.radius);

    }
}
