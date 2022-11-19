using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class WorkerBehavior : MonoBehaviour
{

    [SerializeField] float timeBetweenAttacks;
    [SerializeField] int attackDamage = 10;
    [SerializeField][Range(1,3)] int numberOfAttacks = 1;
    [SerializeField] float attack1Speed = 1;
    [SerializeField] float attack2Speed = 1;
    [SerializeField] float attack3Speed = 1;
    [SerializeField] private MeleeWeaponBehaviour weaponCollider;

    private Animator animator;
    private GameObject player;
    NavMeshAgent agent;
    bool canAttack = true;
    bool attackRange = false;
    [SerializeField] public float attackType = 0;
    [SerializeField] bool randomAttack = false;

    public AudioClip attackAudioClip;
    [HideInInspector]
    private AudioSource attackAudioSource;

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

        float distance = Vector3.Distance(player.transform.position, transform.position);

        transform.LookAt(player.transform.position);

        if (distance > agent.stoppingDistance)
        {
            animator.SetBool("Moving", true);
        }
        else
        {
            animator.SetBool("Moving", false);

        }

        Chase();
    }
    private void Chase()
    {
        agent.SetDestination(player.transform.position);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (enabled)
        {
            if (other.gameObject == player)
            {
                attackRange = true;
                if (canAttack)
                {
                    canAttack = false;
                    //animator.SetTrigger("Attack");
                    StartCoroutine(AttackCooldown());
                }

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
            attackRange = false;
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
    public void ResetAnimatorSpeed()
    {
        animator.speed = 1;
    }

    IEnumerator AttackCooldown()
    {
        while (attackRange)
        {
            if (randomAttack)
            {
                int r = Random.Range(0, numberOfAttacks);
                //Randomly choose death animation type
                switch (r)
                {
                    case 0:
                        animator.speed = attack1Speed;
                        animator.SetFloat("attack_type", 0);
                        break;
                    case 1:
                        animator.speed = attack2Speed;
                        animator.SetFloat("attack_type", 0.5f);
                        break;
                    case 2:
                        animator.speed = attack3Speed;
                        animator.SetFloat("attack_type", 1);
                        break;

                }
            }
            else
                animator.speed = attack1Speed;
            animator.SetTrigger("Attack");
            PlayHit();
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
        canAttack = true;
        yield return 0;

    }

}
