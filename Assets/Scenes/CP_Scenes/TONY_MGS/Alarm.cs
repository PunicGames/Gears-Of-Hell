using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    public float alertTime = 60f;

    [HideInInspector] public bool isActive = false;

    public delegate void OnStartAlarm(Vector3 lastSeenPos);
    public OnStartAlarm onStartAlarm;

    public delegate void OnStopAlarm();
    public OnStopAlarm onStopAlarm;


    public void SoundAlarm(Vector3 playerLastSeenPos)
    {
        if (!isActive)
        {
            isActive = true;
            GetComponent<AudioSource>().Play();
            onStartAlarm?.Invoke(playerLastSeenPos);
            Invoke(nameof(StopAlarm), alertTime);

        }

    }
    private void StopAlarm()
    {
        if (isActive)
        {
            isActive = false;
            GetComponent<AudioSource>().Stop();
            onStopAlarm?.Invoke();
        }



    }
}
