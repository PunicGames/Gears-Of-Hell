using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{

    public LayerMask targetMask;
    public LayerMask obstructionMask;
     public float radius = 0;

    private delegate void OnMakeSound(Vector3 pos);
    private OnMakeSound onMakeSound;

    public void MakeSound(float soundRadius)
    {
        Collider[] soundRangeChecks = Physics.OverlapSphere(transform.position, soundRadius, targetMask);

        foreach (var colleage in soundRangeChecks)
        {
            Vector3 directionToTarget = (colleage.gameObject.transform.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, colleage.gameObject.transform.position);
            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
            {
                CP_GunnerBehaviour gunner = colleage.gameObject.GetComponent<CP_GunnerBehaviour>();
                gunner?.ListenToSound(transform.position);
            }
            else
            {
                if (distanceToTarget < radius * .5f)
                {
                    CP_GunnerBehaviour gunner = colleage.gameObject.GetComponent<CP_GunnerBehaviour>();
                    gunner?.ListenToSound(transform.position);
                }
            }
        }

    }
}
