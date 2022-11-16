using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class WorkerBehavior : MonoBehaviour
{

    [SerializeField] float timeBetweenAttacks;
    [SerializeField] int attackDamage = 10;
    [SerializeField] private MeleeWeaponBehaviour weaponCollider;

    private Animator animator;
    private GameObject player;
    NavMeshAgent agent;
    bool canAttack = true;
    bool attackRange = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        weaponCollider.player = player;
        weaponCollider.health = GetComponent<EnemyHealth>();
        weaponCollider.playerHealth = player.GetComponent<Health>();
        weaponCollider.attackDamage = attackDamage;
        weaponCollider.enabled = false;

        animator.SetBool("Moving", true);
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


    IEnumerator AttackCooldown()
    {
        while (attackRange)
        {

            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
        canAttack = true;
        yield return 0;

    }

}
