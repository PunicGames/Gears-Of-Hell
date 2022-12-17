using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class FireWorker : MonoBehaviour
{
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] int attackDamage = 10;
    [SerializeField][Range(1, 3)] int numberOfAttacks = 1;
    [SerializeField] float attack1Speed = 1;
    [SerializeField] float attack2Speed = 1;
    [SerializeField] float attack3Speed = 1;
    [SerializeField] private MeleeWeaponBehaviour weaponCollider;

    private Animator animator;
    private GameObject player;
    NavMeshAgent agent;
    bool canAttack = false;
    bool alreadyAttacked = false;
    [SerializeField] public float attackType = 0;
    [SerializeField] bool randomAttack = false;

    [HideInInspector] EnemySoundManager enemySoundManager;

    //Maquina de estados
    [HideInInspector] public enum WorkerFireState { IDLE, PURSUE, ATTACK };
    [HideInInspector] public WorkerFireState currentLocomotionState = WorkerFireState.PURSUE;
    [HideInInspector] public WorkerFireState currentActionState = WorkerFireState.IDLE;

    // Upgrade
    bool upgraded = false;
    [SerializeField] private ParticleSystem RaiseUnit;

    // Chase
    [HideInInspector] public Vector3 chasingTarget;
    [HideInInspector] public bool goFire;
    [HideInInspector] public FireBehavior fireBehavior;

    private void Start()
    {

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        enemySoundManager = gameObject.GetComponent<EnemySoundManager>();

        weaponCollider.player = player;
        weaponCollider.health = GetComponent<EnemyHealth>();
        weaponCollider.playerHealth = player.GetComponent<Health>();
        weaponCollider.attackDamage = attackDamage;
        weaponCollider.enabled = false;

        animator.SetBool("Moving", true);
        animator.SetFloat("attack_type", attackType);

        chasingTarget = player.transform.position;
    }
    private void Update()
    {
        LocomotionFSM();
        ActionFSM();
    }
    private void LocomotionFSM()
    {
        if (!goFire) { 
            chasingTarget = player.transform.position;
        }

        float distance;
        distance = Vector3.Distance(chasingTarget, transform.position);
        agent.SetDestination(chasingTarget);

        switch (currentLocomotionState)
        {
            case WorkerFireState.IDLE:
                animator.SetBool("Moving", false);
                enemySoundManager.PauseSound("walk");
                transform.LookAt(chasingTarget);
                if (distance > agent.stoppingDistance)
                {
                    currentLocomotionState = WorkerFireState.PURSUE;
                }
                break;
            case WorkerFireState.PURSUE:
                animator.SetBool("Moving", true);
                enemySoundManager.PlaySound("walk");
                if (distance <= agent.stoppingDistance)
                {
                    currentLocomotionState = WorkerFireState.IDLE;
                }
                break;
        }



    }
    private void ActionFSM()
    {
        switch (currentActionState)
        {
            case WorkerFireState.IDLE:
                //Do nothing
                break;

            case WorkerFireState.ATTACK:
                if (!alreadyAttacked) {
                    Attack();
                    currentActionState = WorkerFireState.IDLE;
                } 
                break;

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (enabled)
        {
            if (other.gameObject == player)
            {
                currentActionState = WorkerFireState.ATTACK;
            } else if (other.gameObject.tag == "Fire") {
                goFire = false;
                currentActionState = WorkerFireState.ATTACK;
                if (fireBehavior != null) { 
                    fireBehavior.Die();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            currentActionState = WorkerFireState.IDLE;
        }
    }
    public void ActivateWeaponCollider()
    {
        weaponCollider.enabled = true;
    }

    public void DeactivateWeaponCollider()
    {
        weaponCollider.enabled = false;
    }
    public void ResetParameters()
    {
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }
    public void Attack()
    {
        if (randomAttack)
        {
            int r = Random.Range(0, numberOfAttacks);
            //Randomly choose death animation type
            switch (r)
            {
                case 0:
                    animator.SetFloat("attack_speed", attack1Speed);
                    animator.SetFloat("attack_type", 0);
                    break;
                case 1:
                    animator.SetFloat("attack_speed", attack2Speed);
                    animator.SetFloat("attack_type", 0.5f);
                    break;
                case 2:
                    animator.SetFloat("attack_speed", attack3Speed);
                    animator.SetFloat("attack_type", 1);
                    break;

            }
        }
        else
            animator.SetFloat("attack_speed", attack1Speed);

        alreadyAttacked = true;
        animator.SetTrigger("Attack");
        enemySoundManager.PlaySound("hit");

    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void UpgradeAttackSpeed()
    {
        if (!upgraded)
        {
            upgraded = true;
            RaiseUnit.Play();
            RaiseUnit.loop = true;

            attackDamage = (int)(attackDamage * 1.2f);
        }
    }
}
