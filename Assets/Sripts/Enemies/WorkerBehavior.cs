using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class WorkerBehavior : MonoBehaviour
{

    [SerializeField] float timeBetweenAttacks;
    [SerializeField] int attackDamage = 10;
    [SerializeField] [Range(1, 3)] int numberOfAttacks = 1;
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

    public AudioClip attackAudioClip;
    [HideInInspector]
    private AudioSource attackAudioSource;

    //Maquina de estados
    private enum state { IDLE, PURSUE, ATTACK };
    private state currentLocomotionState = state.PURSUE;
    private state currentActionState = state.IDLE;

    private void Start()
    {

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        attackAudioSource = gameObject.GetComponent<AudioSource>();

        weaponCollider.player = player;
        weaponCollider.health = GetComponent<EnemyHealth>();
        weaponCollider.playerHealth = player.GetComponent<Health>();
        weaponCollider.attackDamage = attackDamage;
        weaponCollider.enabled = false;

        animator.SetBool("Moving", true);
        animator.SetFloat("attack_type", attackType);
    }
    private void Update()
    {
        LocomotionFSM();
        ActionFSM();
    }
    private void LocomotionFSM()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        agent.SetDestination(player.transform.position);

        switch (currentLocomotionState)
        {
            case state.IDLE:
                animator.SetBool("Moving", false);
                transform.LookAt(player.transform.position);
                if (distance > agent.stoppingDistance)
                {
                    currentLocomotionState = state.PURSUE;
                }
                break;
            case state.PURSUE:
                animator.SetBool("Moving", true);
                if (distance <= agent.stoppingDistance)
                {
                    currentLocomotionState = state.IDLE;
                }
                break;
        }

    }
    private void ActionFSM()
    {
        switch (currentActionState)
        {
            case state.IDLE:
                //Do nothing
                break;

            case state.ATTACK:
                if (!alreadyAttacked) Attack();
                break;

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (enabled)
        {
            if (other.gameObject == player)
            {
                currentActionState = state.ATTACK;

            }
        }
    }

    private void PlayHit()
    {
        attackAudioSource.clip = attackAudioClip;
        attackAudioSource.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            currentActionState = state.IDLE;
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
        PlayHit();


    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void UpgradeAttackSpeed()
    {
        attack1Speed = 2.5f;
        attack2Speed = 1.5f;
        attack3Speed = 1.2f;
    }

}