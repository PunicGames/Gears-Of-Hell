using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProspectorBehaviour : MonoBehaviour
{
    // TODO: Arreglar colisionadores al morir
    // TODO: Mirar si después de hacer la habilidad sigue escondido que haga otra habilidad en vez de pasar a estado CHASING.

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
    private enum State { CHASING, HIDING, CASTING, RESTING}
    private State currentState;

    // Variables
    private bool casting = false;

    // Utility system variables
    private EnemyHealth m_prospectorHealth;
    private Health m_playerHealth;
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

    // Sounds
    private EnemySoundManager enemySoundManager;

    // Habilities visuals
    [SerializeField] private ParticleSystem CureSelf;
    [SerializeField] private GameObject areaHealVisuals;

    private void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        m_prospectorHealth = GetComponent<EnemyHealth>();
        m_playerHealth = player.GetComponent<Health>();
        enemySoundManager = gameObject.GetComponent<EnemySoundManager>();

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
                    enemySoundManager.PauseSound("footsteps");

                    if (ableToHide) currentState = State.HIDING;
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
                        enemySoundManager.PauseSound("footsteps");
                    }
                }

                break;
            case State.CASTING:
                transform.LookAt(player.position);

                if (!casting) { 
                    casting = true;
                    agent.isStopped = true;
                    UtilityCasting();
                }
                
                break;

            case State.RESTING:
                transform.LookAt(player.position);
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

        // DECISION BASED ON VARIABLES
        // -------- Functions --------
        float VJ = currentPlayerHealthRate;
        int NW = numEnemiesInRange > 3 ? 1 : 0; // Have in mind the foreman adds up 1 in the numEnemiesInRange variable.
        float VP = (Mathf.Pow(m_prospectorHealth.startingHealth, 3f) - Mathf.Pow(m_prospectorHealth.currentHealth, 3f)) / (Mathf.Pow(m_prospectorHealth.startingHealth, 3f));
        float VU = 1 - enemiesHealthStatus;

        // Utility system
        float castVelocityValue = 0.3f * VJ + 0.7f * NW;
        float castOwnCure = VP;
        float castAreaCure = 0.9f * VU + 0.1f * VP;

        // ------ Utility System Values ------ Check documentation to know what these variables stand for.
        Debug.Log("VJ: " + VJ);
        Debug.Log("NW: " + NW);
        Debug.Log("VP: " + VP);
        Debug.Log("VU: " + VU);
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
            areaHealVisuals.SetActive(true);
            animator.SetTrigger("HealAround");
        }
        
    }

    private void Chase(Vector3 position)
    {
        agent.SetDestination(position);
        agent.isStopped = false;
        animator.SetBool("isMoving", true);
        enemySoundManager.PlaySound("footsteps");
    }

    public void CastVelocityUpgrade() {
        Collider[] hitColliders = m_outerRing.GetEnemiesInRange();
        foreach(Collider collider in hitColliders) {

            // TODO: Wait for more behaviours: WORKER BOT

            if (collider != null) { // In case an enemy died while receiving and checking the loop
                // Increase Attack of different enemies
                if (collider.gameObject.GetComponent<GunslingerBehaviour>() != null)
                {
                    // Gunslinger
                    collider.gameObject.GetComponent<GunslingerBehaviour>().UpgradeAttackSpeed();
                }
                else if (collider.gameObject.GetComponent<WorkerBehavior>() != null)
                {
                    // Worker
                    collider.gameObject.GetComponent<WorkerBehavior>().UpgradeAttackSpeed();
                }
                else if (collider.gameObject.GetComponent<GunnerBehaviour>() != null)
                {
                    // Gunner
                    collider.gameObject.GetComponent<GunnerBehaviour>().UpgradeAttackSpeed();
                }
                //else if (collider.gameObject.GetComponent<BigSpiderBotBehaviour>() != null)
                //{
                //    // Big Spider Bot
                //    collider.gameObject.GetComponent<BigSpiderBotBehaviour>().UpgradeAttackSpeed();
                //}
                //else if (collider.gameObject.GetComponent<BombSpiderBotBehaviour>() != null) {
                //    // Bomb Spider Bot
                //    collider.gameObject.GetComponent<BombSpiderBotBehaviour>().UpgradeAttackSpeed();
                //}
            }
        }
    }
    public void CastOwnCure() {
        CureSelf.Play();
        enemySoundManager.PlaySound("ownHeal");
        m_prospectorHealth.Heal(40);
    }
    public void CastAreaCure() {
        Collider[] hitColliders = m_outerRing.GetEnemiesInRange();
        Debug.Log("ENEMIES IN RANGE " + hitColliders.Length);
        foreach (Collider eC in hitColliders)
        {
            EnemyHealth eH = eC.gameObject.GetComponent<EnemyHealth>();
            if (eH.enemyType != EnemyHealth.EnemyType.FOREMAN) {
                eH.Heal(20);
            }
        }

        areaHealVisuals.SetActive(false);
        enemySoundManager.PlaySound("areaHeal");
    }

    public void ResetCasting() {
        //Debug.Log("Ability cast finished");
        casting = false;
        currentState = State.RESTING;
        Invoke("StopResting", 2);
    }

    private Transform CheckForAvailableHidenSpot() {
        // Check distance
        float minDistanceToPlayer = 2.0f;
        float minDistanceToForeman = 1.0f;
        float distanceToPlayer = 0.0f;
        float distanceToForeman = 0.0f;
        bool hidenSpot = false;
        Transform nextPosition = HidingSpots[0]; // Initialized to avoid errors but lacks of relevance

        // Give randomness the way hidingSpots are checked
        HashSet<int> indexes = new HashSet<int>();
        for (int i = 0; i < HidingSpots.Length; i++) {
            indexes.Add(i);
        }

        // Get new valid spot
        for (int i = 0; i < HidingSpots.Length; i++) {

            // Get random index
            int idx = Random.Range(0, indexes.Count);
            // Take it out from set
            indexes.Remove(idx);

            distanceToPlayer = Vector3.Distance(HidingSpots[idx].position, player.position);
            distanceToForeman = Vector3.Distance(HidingSpots[idx].position, transform.position);
            hidenSpot = false;

            RaycastHit hit;
            if (Physics.Raycast(HidingSpots[idx].position, (player.position - HidingSpots[idx].position).normalized, out hit, Mathf.Infinity, obstacleMask))
            {
                hidenSpot = true;
            }

            if ((distanceToPlayer >= minDistanceToPlayer) && (distanceToForeman >= minDistanceToForeman) && (hidenSpot))
            {
                //Debug.Log("Valid Hiding Spot");
                nextPosition = HidingSpots[idx];
                break;
            }

        }

        return nextPosition;
    }

    private bool CheckCanCastAgain() {
        RaycastHit hit;
        if (Physics.Raycast(targetHideSpot.position, (player.position - targetHideSpot.position).normalized, out hit, Mathf.Infinity, obstacleMask))
        {
            return true;
        }
        else {
            return false;
        }
    }

    public void PlayCastPowerUpSound() {
        enemySoundManager.PlaySound("getHim");
    }

    private void StopResting() {
        if (ableToHide)
        {
            if (CheckCanCastAgain())
                currentState = State.CASTING;
            else
                currentState = State.CHASING;
        }
        else
            currentState = State.CHASING;
    }
}
