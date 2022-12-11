using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class WorkerBotBehavior : MonoBehaviour
{

    [SerializeField] int MAXGEARSCAPACITY = 10; //capacidad maxima de monedas que puede recoger

    [SerializeField] int attackDamage = 10; //daño por cada golpe
    [SerializeField] float rollSpeed = 10.0f; //velocidad a la que gira atacando
    [SerializeField] private MeleeWeaponBehaviour weaponCollider;
    [SerializeField] GameObject particleEffect;     
    [SerializeField] Collider recolectRangeCollider;

    private Animator animator;
    private GameObject player;
    NavMeshAgent agent;

    int currentGears = 0; //monedas que lleva recogidas
    bool attacking = false;
    bool alreadyAttacked = false;

    private enum FSM2_states { 
        IDLE, 
        PURSUE,
        ATTACK,
    };

    private enum FSM1_states
    {
        RECOLECT,
        SEARCH,
    };

    private FSM1_states currentFSM1State = FSM1_states.SEARCH; //la fsm 1
    private FSM2_states currentFSM2State = FSM2_states.PURSUE; //la fsm 2

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        weaponCollider.player = player;
        weaponCollider.health = GetComponent<EnemyHealth>();
        weaponCollider.playerHealth = player.GetComponent<Health>();
        weaponCollider.attackDamage = attackDamage;
        weaponCollider.enabled = false;

        animator.SetBool("isMoving", true);
    }

    private void Update()
    {
        FSM_LVL_2();
        ActionFSM();
    }
    private void FSM_LVL_2() //{ IDLE, PURSUE, ATTACK }
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        agent.SetDestination(player.transform.position);

        switch (currentFSM2State)
        {
            case FSM2_states.IDLE:
                animator.SetBool("isMoving", false); //variable que este en el animator
                transform.LookAt(player.transform.position); //miramos al player

                if (distance > agent.stoppingDistance) 
                {
                    //si la distancia es mayor a la asignada a detenerse comenzamos a perseguir
                    currentFSM2State = FSM2_states.PURSUE;
                }
                break;

            case FSM2_states.PURSUE:
                animator.SetBool("isMoving", true); //variable del animator
                if (distance <= agent.stoppingDistance)
                {
                    currentFSM2State = FSM2_states.IDLE;
                }
                break;
        }
    }

    private void ActionFSM()
    {
        switch (currentFSM2State)
        {
            case FSM2_states.IDLE:
                //Do nothing
                break;

            case FSM2_states.ATTACK:
                //si no ha atacado se pone a atacar
                if (!alreadyAttacked) Attack();
                break;

        }
    }

    public void Attack()
    {
        alreadyAttacked = true;
        animator.SetBool("isAttacking", true);
        particleEffect.SetActive(true);
        particleEffect.GetComponent<ParticleSystem>().Play();
        print("particle play");

        Invoke(nameof(ResetParameters), 5);
        Invoke(nameof(StopParticleEffect), 3);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enabled)
        {
            if (other.gameObject == player)
            {
                ActivateWeaponCollider();
                currentFSM2State = FSM2_states.ATTACK;
            }
        }
    }
    public void StopParticleEffect()
    {
        particleEffect.GetComponent<ParticleSystem>().Play();
        print("prineto");
    }
    public void ResetParameters()
    {
        DeactivateWeaponCollider();
        alreadyAttacked = false;
        animator.SetBool("isAttacking", false);
        particleEffect.SetActive(false);
        print("resetParameters");
        currentFSM2State = FSM2_states.IDLE;
    }

    public void ActivateWeaponCollider()
    {
        weaponCollider.enabled = true;
    }

    public void DeactivateWeaponCollider()
    {
        weaponCollider.enabled = false;
    }

    
}
