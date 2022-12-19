using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vigil : MonoBehaviour
{
    [SerializeField] Alarm alarm;


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<EnemyVision>().onSpot += () => { if (!alarm.isActive) alarm.SoundAlarm(transform.forward * 5); };
    }

}
