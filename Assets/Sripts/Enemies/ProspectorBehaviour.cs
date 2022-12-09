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

    // Hiding variables
    [SerializeField] private bool ableToHide;
    [SerializeField] private Transform[] HidingSpots;
    private Transform targetHideSpot;
    private bool hiden = false;
    private bool hiding = false;
    [SerializeField] private LayerMask obstacleMask;

    private void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
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
                    Chase(player.position);
                }
                else {
                    animator.SetBool("isMoving", false);
                    if(ableToHide) currentState = State.HIDING;
                    else currentState = State.CASTING;
                }
                break;
            case State.HIDING:

                if (!hiding)
                {
                    targetHideSpot = CheckForAvailableHidenSpot();
                    hiding = true;
                }
                else {
                    Chase(targetHideSpot.position);

                    if (Vector3.Distance(transform.position, targetHideSpot.position)  < 0.1)
                    {
                        hiding = false;
                        currentState = State.CASTING;
                    }
                }

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

    private void Chase(Vector3 position)
    {
        agent.SetDestination(position);
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

    private Transform CheckForAvailableHidenSpot() {
        // Check distance
        float minDistanceToPlayer = 2.0f;
        float minDistanceToForeman = 1.0f;
        float distanceToPlayer = 0.0f;
        float distanceToForeman = 0.0f;
        bool hidenSpot = false;
        Transform nextPosition = HidingSpots[0]; // Se inicializa para evitar errores pero no tiene relevancia

        foreach (Transform hideSpot in HidingSpots) {
            distanceToPlayer = Vector3.Distance(hideSpot.position, player.position);
            distanceToForeman = Vector3.Distance(hideSpot.position, transform.position);
            hidenSpot = false;
            
            RaycastHit hit;
            if (Physics.Raycast(hideSpot.position, (player.position - hideSpot.position).normalized, out hit, Mathf.Infinity, obstacleMask))
            {
                Debug.Log("S�");
                hidenSpot = true;
            }
            else {
                Debug.Log("NO");
            }

            if ((distanceToPlayer >= minDistanceToPlayer) && (distanceToForeman >= minDistanceToForeman) && (hidenSpot)) {
                //Debug.Log("Escondite v�lido");
                nextPosition = hideSpot;
                //break;
            }
        }

        return nextPosition;
    }
}
