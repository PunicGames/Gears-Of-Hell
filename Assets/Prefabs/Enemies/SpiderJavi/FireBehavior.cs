using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBehavior : MonoBehaviour
{
    private Transform playerPos;
    [SerializeField] private GameObject rangeToEnemy;
    [SerializeField] private LayerMask m_LayerMask;
    private WorkerBehavior m_Behavior;

    private void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag(nameof(Player)).GetComponent<Transform>();
        InvokeRepeating("SendWorkerStopFire", 0.5f, 1f);
    }

    public void SendWorkerStopFire() {
        Collider[] hitColliders = Physics.OverlapSphere(rangeToEnemy.transform.position, rangeToEnemy.GetComponent<SphereCollider>().radius, m_LayerMask, QueryTriggerInteraction.Ignore);
        foreach (Collider eC in hitColliders)
        {
            EnemyHealth eH = eC.GetComponent<EnemyHealth>();
            if (eH.enemyType == EnemyHealth.EnemyType.WORKER && eC.GetComponent<FireWorker>() != null)
            {
                if (Vector3.Distance(eC.transform.position, playerPos.position) >= Vector3.Distance(eC.transform.position, this.transform.position))
                {
                    eC.GetComponent<FireWorker>().goFire = true;
                    eC.GetComponent<FireWorker>().chasingTarget = transform.position;
                    eC.GetComponent<FireWorker>().fireBehavior = this;
                    break;
                }
                else { 
                    eC.GetComponent<FireWorker>().goFire = false;
                }
            }
        }
    }

    public void Die() {
        Destroy(gameObject);
    }

}
