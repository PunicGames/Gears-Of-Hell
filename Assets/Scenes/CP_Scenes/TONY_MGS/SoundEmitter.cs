using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{

    public LayerMask targetMask;
    [HideInInspector] public float radius = 0;

    private delegate void OnMakeSound(Vector3 pos);
    private OnMakeSound onMakeSound;

    public void MakeSound(float soundRadius)
    {
        Collider[] soundRangeChecks = Physics.OverlapSphere(transform.position, soundRadius, targetMask);

        foreach (var colleage in soundRangeChecks)
        {
            CP_GunnerBehaviour gunner = colleage.gameObject.GetComponent<CP_GunnerBehaviour>();
            gunner?.ListenToSound(transform.position);
        }

    }
}
