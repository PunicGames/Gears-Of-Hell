using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    #region inEditorVariables
    [SerializeField] public Transform eyes;
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
    [HideInInspector] public float initialSpotRadius;
    [HideInInspector] public float initialPerceptionRadius;
    private Transform target;

    //Delegates
    public delegate void OnAlert(Vector3 targetPos,bool t);
    public OnAlert onAlert;
    public delegate void OnSpot();
    public OnSpot onSpot;
    public delegate void OnLostingSight();
    public OnLostingSight onLostingSight;

    private void Start()
    {
        initialSpotRadius = spotRadius;
        initialPerceptionRadius = perceptionRadius;
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

        Collider[] rangeChecks = Physics.OverlapSphere(eyes.position, perceptionRadius, targetMask);

        if (rangeChecks.Length != 0)
        {
            target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - eyes.position).normalized;

            if (Vector3.Angle(eyes.forward, directionToTarget) < angle * .5f)
            {
                float distanceToTarget = Vector3.Distance(eyes.position, target.position);

                if (!Physics.Raycast(eyes.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    playerInReach = true;
                    if (!isSpotted)
                    {
                        if (!isAlerted)
                        {
                            onAlert?.Invoke(target.position,true);
                            isAlerted = true;

                        }

                        if (distanceToTarget <= spotRadius)
                        {

                            onSpot?.Invoke();
                            isSpotted = true;
                            isAlerted = false;
                            return;
                        }
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
        else
        {
            playerInReach = false;
            if (isSpotted)
            {
                Debug.Log("WEA");
                isAlerted = true;
                isSpotted = false;
                onAlert?.Invoke(target.position,false);
            }
        }

    }

}
