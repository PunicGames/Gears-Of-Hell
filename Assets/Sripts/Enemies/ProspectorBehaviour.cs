using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProspectorBehaviour : MonoBehaviour
{
    // TODO: Arreglar colisionadores al morir


    Transform player;
    NavMeshAgent agent;
    Animator animator;

    // Ring variables
    [HideInInspector] public bool insideOuterRing;
    [HideInInspector] public bool insideInnerRing;
    [HideInInspector] public bool fromInside = false;
    [HideInInspector] public bool fromOutside = true;
    [SerializeField] private outerRing m_outerRing;

    // FSM STATES
    private enum State { CHASING, HIDING, CASTING}
    private State currentState;

    // Variables
    private bool casting = false;

    // Utility system variables
    private EnemyHealth m_prospectorHealth;
    private Health m_playerHealth;
    private float currentProspectorHealthRate;
    private float enemiesHealthStatus;
    private float currentPlayerHealthRate;
    private int numEnemiesInRange;



    private void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        m_prospectorHealth = GetComponent<EnemyHealth>();
        m_playerHealth = player.GetComponent<Health>();

        currentState = State.CHASING;
    }

    private void FixedUpdate()
    {
        basic_FSM();
    }


    private void basic_FSM() {
        switch (currentState)
        {
            case State.CHASING:
                if (!insideOuterRing || (insideOuterRing && !insideInnerRing && fromOutside))
                {
                    Chase();
                }
                else {
                    animator.SetBool("isMoving", false);
                    currentState = State.CASTING;
                }
                break;
            case State.HIDING:
                break;
            case State.CASTING:
                transform.LookAt(player.position);

                if (!casting) { 
                    casting = true;
                    UtilityCasting();
                }
                
                break;
            default:
                break;
        }
    }

    private void CP_FSM()
    {
        switch (currentState)
        {
            case State.CHASING:
                if (!insideOuterRing || (insideOuterRing && !insideInnerRing && fromOutside))
                {
                    Chase();
                }
                else
                {
                    animator.SetBool("isMoving", false);
                    currentState = State.HIDING;
                }
                break;
            case State.HIDING:
                break;
            case State.CASTING:
                break;
            default:
                break;
        }
    }

    private void UtilityCasting() {

        // Variables to make a decision
        numEnemiesInRange = m_outerRing.checkNumEnemiesInRange();
        enemiesHealthStatus = m_outerRing.checkEnemiesHealthStatus();
        currentPlayerHealthRate = (float)m_playerHealth.currentHealth / (float)m_playerHealth.maxHealth;
        currentProspectorHealthRate = m_prospectorHealth.startingHealth / m_prospectorHealth.startingHealth; // No se usa. Probablemente hay que borrarla.

        // DECISION BASED ON VARIABLES
        // -------- Functions --------
        float VJ = currentPlayerHealthRate;
        int NW = numEnemiesInRange > 3 ? 1 : 0; // Have in mind the foreman adds up 1 in the numEnemiesInRange variable.
        float VP = (Mathf.Pow(m_prospectorHealth.startingHealth, 3f) - Mathf.Pow(m_prospectorHealth.currentHealth, 3f)) / (Mathf.Pow(m_prospectorHealth.startingHealth, 3f));
        float VU = 1 - enemiesHealthStatus;

        Debug.Log("VJ: " + VJ);
        Debug.Log("NW: " + NW);
        Debug.Log("VP: " + VP);
        Debug.Log("VU: " + VU);

        // Utility system
        float castVelocityValue = 0.3f * VJ + 0.7f * NW;
        float castOwnCure = VP;
        float castAreaCure = 0.85f * VU + 0.15f * VP;

        Debug.Log("CastVelocityValue: " + castVelocityValue);
        Debug.Log("CastOwnCure: " + castOwnCure);
        Debug.Log("CastAreaCure: " + castAreaCure);

        // Decision maker
        if (castVelocityValue >= castOwnCure && castVelocityValue >= castAreaCure)
        {
            animator.SetTrigger("IncreaseAttackSpeed");
        }
        else if (castOwnCure >= castAreaCure)
        {
            animator.SetTrigger("Heal");
        }
        else {
            animator.SetTrigger("HealAround");
        }
        
    }

    private void Chase()
    {
        agent.SetDestination(player.position);
        animator.SetBool("isMoving", true);
    }

    public void CastVelocityUpgrade() {
        Collider[] hitColliders = m_outerRing.GetEnemiesInRange();
        foreach(Collider collider in hitColliders) {

            // TODO: Wait for more behaviours

            if (collider != null) { // In case an enemy died while receiving and checking the loop
                // Increase Attack of different enemies
                if (collider.gameObject.GetComponent<RangedEnemy>() != null) {
                    // RangedEnemy
                    collider.gameObject.GetComponent<RangedEnemy>().UpgradeAttackSpeed();
                } else if (collider.gameObject.GetComponent<WorkerBehavior>() != null) {
                    // Worker
                    collider.gameObject.GetComponent<WorkerBehavior>().UpgradeAttackSpeed();
                }
            }
        }
    }
    public void CastOwnCure() {
        m_prospectorHealth.Heal(100);
    }
    public void CastAreaCure() {
        Collider[] hitColliders = m_outerRing.GetEnemiesInRange();
        foreach (Collider eC in hitColliders)
        {
            EnemyHealth eH = eC.gameObject.GetComponent<EnemyHealth>();
            if (eH.enemyType != EnemyHealth.EnemyType.FOREMAN) {
                eH.Heal(20);
            }
        }
    }

    public void ResetCasting() {
        Debug.Log("LLEGA");
        casting = false;
        currentState = State.CHASING;
    }

}
