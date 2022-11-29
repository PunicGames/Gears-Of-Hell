using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : MonoBehaviour
{
    public float sightRange;
    public float attackRange;
    public float attackSpeed;
    public float bulletSpeed;
    public float damage = 10;

    private bool alreadyAttacked = false;

    public GameObject bullet;

    Transform player;
    NavMeshAgent agent;
    Animator animator;
    AudioSource gunAudio;


    // Bullet colors
    [SerializeField] private Color albedo;
    [SerializeField] private Color emissive;

    private void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        gunAudio = GetComponent<AudioSource>();

    }

    private void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        transform.LookAt(player.position);

        if (distance <= attackRange)
        {
            Attack();
            animator.SetBool("Moving", false);
        }
        else if (distance <= sightRange)
        {
            Chase();
            animator.SetBool("Moving", true);
        }
    }

    private void Attack()
    {
        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {
            gunAudio.Play();
            GameObject b = Instantiate(bullet, new Vector3(transform.position.x, transform.position.y+1f, transform.position.z), Quaternion.identity);
            b.transform.LookAt(player.transform);
            Bullet bulletParams = b.GetComponent<Bullet>();
            bulletParams.SetForce(bulletSpeed);
            bulletParams.SetDamage(damage);
            bulletParams.SetLaser(false);
            bulletParams.owner = Bullet.BulletOwner.ENEMY;
            bulletParams.SetBulletColors(albedo, emissive);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), attackSpeed);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void Chase()
    {
        agent.SetDestination(player.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
