using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GunnerBehaviour : MonoBehaviour
{
    [Header("SHOOTING")]
    public float cadenceTime = 1f;
    public float reloadTime = 2f;
    public float bulletSpeed = 2;
    public float damage = 10;
    public int bulletsPerMag = 5;
    public int bulletsPerBurst = 5;
    public float SemiAutoTime = 2f;
    public float bulletLifetime = 3f;
    [Space]
    public bool hasToSeeYouToShoot = false;
    public bool SemiAuto = false;
    public bool canMoveWhileShooting;

    [Header("GRENADE")]
    public bool enableGrenades = false;
    bool canLaunchGrenade = false;
    [Tooltip("The higher the less probability")]
    [SerializeField] [Range(1, 10)] int grenadeProbabilityRatio;
    public int numberOfGrenades = 3;
    [SerializeField] Transform grenadeThrowPoint;
    public int grenadeDamage = 30;



    [Header("GENERAL")]

    public float spotDistance = 7f;

    public Transform shootOrigin;
    [SerializeField] ParticleSystem muzzleVFX;

    private int bulletsInMag;
    private int bulletsInBurst;

    private bool playerOnSight = false;

    private bool alreadyAttacked = false;
    private bool inAttackRange = false;

    private bool canComputeGrenadeChance = true;

    private float distance;

    public GameObject bullet;
    public GameObject grenade;

    Transform player;
    NavMeshAgent agent;
    Animator animator;
    AudioSource gunAudio;

    private enum gunnerState { IDLE, PURSUE, ATTACK, RECHARGE, GRENADE };
    private gunnerState currentState = gunnerState.IDLE;


    // Bullet colors
    [SerializeField] private Color albedo;
    [SerializeField] private Color emissive;



    private void Start()
    {
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
        distance = Vector3.Distance(player.position, transform.position);
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
                    TransitionFromIdle(!playerOnSight || !inAttackRange);
                }
                else
                {
                    TransitionFromIdle(!inAttackRange);
                }

                break;
            case gunnerState.PURSUE:
                agent.SetDestination(player.position);
                transform.LookAt(player.position);
                if (hasToSeeYouToShoot)
                {
                    CheckPlayerSighted();
                    TransitionFromPursue(inAttackRange && playerOnSight);
                }
                else
                {
                    TransitionFromPursue(inAttackRange);
                }
                break;

            case gunnerState.ATTACK:
                transform.LookAt(player.position);
                if (!alreadyAttacked)
                {
                    if (!enableGrenades)
                    {
                        Attack();
                    }
                    else
                    {
                        if (canLaunchGrenade)
                        {
                            animator.SetTrigger("grenade");
                            canLaunchGrenade = false;
                            currentState = gunnerState.GRENADE;
                        }
                        else
                        {
                            Attack();
                        }
                    }
                }
                break;
            case gunnerState.RECHARGE:
                if (inAttackRange)
                    transform.LookAt(player.position);

                break;
            case gunnerState.GRENADE:
                transform.LookAt(player.position);
                //Callback return to idle AfterAnim
                break;
            default:
                break;
        }



    }

    #region actions
    void Attack()
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

        bulletsInBurst--;
        if (bulletsInMag > 0)
            if (bulletsInBurst > 0)
                Invoke(nameof(ResetAttack), cadenceTime);
            else
            {
                Invoke(nameof(ResetAttack), SemiAutoTime);
                bulletsInBurst = bulletsPerBurst;
                if (enableGrenades)
                    if (Random.Range(0, grenadeProbabilityRatio) == 0 && numberOfGrenades > 0)
                    {
                        canLaunchGrenade = true;
                    }
                TransitionToIdle();
            }
        else
        {
            TransitionToRecharge();
        }



    }

    public void LaunchGrenade()
    {

        GameObject g = Instantiate(grenade, new Vector3(grenadeThrowPoint.position.x, grenadeThrowPoint.position.y, grenadeThrowPoint.position.z), Quaternion.identity);

        Grenade grenadeParams = g.GetComponent<Grenade>();
        grenadeParams.target = player.transform.position;
        grenadeParams.damage = grenadeDamage;


        Invoke(nameof(ResetAttack), SemiAutoTime);

        numberOfGrenades--;


    }
    private void CheckPlayerSighted()
    {
        Ray ray = new Ray(transform.position, (player.position - transform.position).normalized);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.tag == "Player")
                playerOnSight = true;
            else
                playerOnSight = false;
        }
    }
    #endregion
    #region transitions
    private void TransitionFromIdle(bool condition)
    {
        if (condition)
        {
            TransitionToPursue();
        }
        else if (!alreadyAttacked)
        {
            currentState = gunnerState.ATTACK;

        }

    }

    private void TransitionFromPursue(bool condition)
    {
        if (condition)
        {
            TransitionToIdle();
        }

    }
    //private void TransitionToAttack();
    private void TransitionToIdle()
    {
        currentState = gunnerState.IDLE;
        animator.SetBool("Moving", false);
        agent.isStopped = true;
    }
    private void TransitionToRecharge()
    {
        animator.SetTrigger("reload");
        Invoke(nameof(ResetAttack), reloadTime);
        bulletsInMag = bulletsPerMag;
        currentState = gunnerState.RECHARGE;
    }

    private void TransitionToPursue()
    {
        currentState = gunnerState.PURSUE;
        animator.SetBool("Moving", true);
        agent.isStopped = false;
    }


    public void GrenadeLaunched()
    {
        TransitionToIdle();

    }
    #endregion
    private void ResetAttack()
    {
        alreadyAttacked = false;
        if (currentState == gunnerState.RECHARGE)
        {
            TransitionToIdle();
            bulletsInBurst = bulletsPerBurst;

        }

    }

    private int ComputeGrenadeChance()
    {
        return Random.Range(0, grenadeProbabilityRatio);
    }

}
