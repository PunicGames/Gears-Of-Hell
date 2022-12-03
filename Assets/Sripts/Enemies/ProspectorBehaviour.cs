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

                currentState = State.CHASING;
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
        float VU = enemiesHealthStatus;

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
        if (castVelocityValue >= castOwnCure && castVelocityValue >= castAreaCure) { 
            
        }
        

        // BORRAR Y TRASLADAR A FUNCIONES DE CASTEO
        casting = false;
    }

    private void Chase()
    {
        agent.SetDestination(player.position);
        animator.SetBool("isMoving", true);
    }

    private void CastVelocityUpgrade() { 

    }
    private void CastOwnCure() { 

    }
    private void CastAreaCure() { 

    }

}
