using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GunnerBehaviour : MonoBehaviour
{
    public float cadenceTime = 1f;
    public float reloadTime = 2f;
    public float bulletSpeed = 2;
    public float damage = 10;
    public int bulletsPerMag = 5;
    public bool SemiAuto = false;
    public int bulletsPerBurst = 5;
    public float SemiAutoTime = 2f;
    public float bulletLifetime = 3f;

    public bool hasToSeeYouToShoot = false;
    public bool canLaunchGrenade = false;
    public float spotDistance = 57f;

    public Transform shootOrigin;
    [SerializeField] ParticleSystem muzzleVFX;

    private int bulletsInMag;
    private int bulletsInBurst;

    private bool playerOnSight = false;

    private bool alreadyAttacked = false;
    private bool alreadyRecharged = false;
    private bool inAttackRange = false;

    public GameObject bullet;

    Transform player;
    NavMeshAgent agent;
    Animator animator;
    AudioSource gunAudio;

    private enum gunnerState { IDLE, PURSUE, ATTACK, RECHARGE };
    private gunnerState currentState = gunnerState.IDLE;


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
        bulletsInMag = bulletsPerMag;
        bulletsInBurst = bulletsPerBurst;

    }

    private void Update()
    {
        FSM();
    }

    private void FSM()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= spotDistance)
            inAttackRange = true;
        else
            inAttackRange = false;



        switch (currentState)
        {
            case gunnerState.IDLE:
                transform.LookAt(player.position);
                if (hasToSeeYouToShoot)
                {
                    CheckPlayerSighted();
                    transitionFromIdle(!playerOnSight || !inAttackRange);

                }
                else
                {
                    transitionFromIdle(!inAttackRange);
                }

                break;
            case gunnerState.PURSUE:
                agent.SetDestination(player.position);
                if (inAttackRange)
                    transform.LookAt(player.position);
                if (hasToSeeYouToShoot)
                {
                    CheckPlayerSighted();
                    transitionFromPursue(inAttackRange && playerOnSight);
                }
                else
                {
                    transitionFromPursue(inAttackRange);
                }
                break;

            case gunnerState.ATTACK:
                transform.LookAt(player.position);
                if (Attack())
                {
                    animator.SetTrigger("reload");
                    Invoke(nameof(ResetAttack), reloadTime);
                    bulletsInMag = bulletsPerMag;
                    currentState = gunnerState.RECHARGE;
                }
                else
                {
                    currentState = gunnerState.IDLE;

                }
                break;
            case gunnerState.RECHARGE:
                if (inAttackRange)
                    transform.LookAt(player.position);
                //Recargando
                if (alreadyRecharged)
                {
                    currentState = gunnerState.IDLE;
                    alreadyRecharged = false;
                }
                break;
            default:
                break;
        }



    }
    bool Attack()
    {

        gunAudio.Play();
        animator.SetTrigger("shoot");
        GameObject b = Instantiate(bullet, new Vector3(shootOrigin.position.x, shootOrigin.position.y, shootOrigin.position.z), Quaternion.identity);
        b.transform.LookAt(player.transform);
        Bullet bulletParams = b.GetComponent<Bullet>();
        bulletParams.SetForce(bulletSpeed);
        bulletParams.SetDamage(damage);
        bulletParams.SetLaser(false);
        bulletParams.owner = Bullet.BulletOwner.ENEMY;
        bulletParams.timeToDestroy = bulletLifetime;
        bulletParams.SetBulletColors(albedo, emissive);


        bulletsInMag--;
        alreadyAttacked = true;
        muzzleVFX.Play();
        if (SemiAuto)
        {
            bulletsInBurst--;
            if (bulletsInMag > 0)
                if (bulletsInBurst > 0)
                    Invoke(nameof(ResetAttack), cadenceTime);
                else
                {
                    Invoke(nameof(ResetAttack), SemiAutoTime);
                    bulletsInBurst = bulletsPerBurst;
                }
            else
            {
                return true;
            }
        }
        else
        {
            if (bulletsInMag > 0)
                Invoke(nameof(ResetAttack), cadenceTime);
            else
            {
                return true;
            }
        }

        return false;


    }
    private void CheckPlayerSighted()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, (player.position - transform.position).normalized);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "Player")
                playerOnSight = true;
            else
                playerOnSight = false;
        }
    }

    private void transitionFromIdle(bool condition)
    {
        if (condition)
        {
            currentState = gunnerState.PURSUE;
            animator.SetBool("Moving", true);
            agent.isStopped = false;
        }
        else if (!alreadyAttacked)
            currentState = gunnerState.ATTACK;
    }
    private void transitionFromPursue(bool condition)
    {
        if (condition)
        {
            currentState = gunnerState.IDLE;
            animator.SetBool("Moving", false);
            agent.isStopped = true;
        }

    }


    private void ResetAttack()
    {
        alreadyAttacked = false;
        if (currentState == gunnerState.RECHARGE) alreadyRecharged = true;

    }



}
