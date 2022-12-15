using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    #region inEditorVariables
    [SerializeField] public Transform parent;
    public float perceptionRadius;
    public float spotRadius;
    [Range(0, 360)]
    public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    #endregion

    [HideInInspector] public bool playerInReach;
    private bool isAlerted = false;
    private bool isSpotted = false;

    //Delegates
    public delegate void OnAlert(Vector3 targetPos);
    public OnAlert onAlert;
    public delegate void OnSpot();
    public OnSpot onSpot;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        if (!isSpotted)
        {
            Collider[] rangeChecks = Physics.OverlapSphere(parent.position, perceptionRadius, targetMask);

            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[0].transform;
                Vector3 directionToTarget = (target.position - parent.position).normalized;

                if (Vector3.Angle(parent.forward, directionToTarget) < angle * .5f)
                {
                    float distanceToTarget = Vector3.Distance(parent.position, target.position);

                    if (!Physics.Raycast(parent.position, directionToTarget, distanceToTarget, obstructionMask))
                    {
                        playerInReach = true;
                        if (distanceToTarget <= spotRadius) { onSpot?.Invoke(); isSpotted = true; return; }
                        if (!isAlerted)
                        {
                            onAlert?.Invoke(target.position);
                            isAlerted = true;
                        }

                    }
                    else
                    {
                        isAlerted = false;
                        playerInReach = false;
                    }
                }
                else
                {
                    playerInReach = false;
                    isAlerted = false;
                }
            }
            else if (playerInReach)
                playerInReach = false;
        }
    }
}
