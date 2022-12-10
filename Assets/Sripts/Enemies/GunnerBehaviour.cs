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

    [Header("GRENADE")]
    public bool canLaunchGrenade = false;
    [Tooltip("The higher the less probability")]
    [SerializeField] [Range(1, 10)] int grenadeProbabilityRatio;
    public int numberOfGrenades = 3;
    public float launchDistance = 7f;

    [Header("SHIELD")]
    public bool useShield = false;
    [SerializeField] GameObject shield;
    public int shieldLife = 30;


    [Header("GENERAL")]

    public float spotDistance = 7f;

    public Transform shootOrigin;
    [SerializeField] ParticleSystem muzzleVFX;

    private int bulletsInMag;
    private int bulletsInBurst;

    private bool playerOnSight = false;

    private bool alreadyAttacked = false;
    private bool alreadyRecharged = false;
    private bool inAttackRange = false;

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
        //if (currentState == gunnerState.GRENADE)
        //{
        //    Debug.Log("Launching");
        //}
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
                    TransitionToIdle();
                    alreadyRecharged = false;
                }
                break;
            case gunnerState.GRENADE:
                transform.LookAt(player.position);
                //Callback return to idle AfterAnim
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

    public void LaunchGrenade()
    {
        /*
        //Create grenade an launch
        //Animation
        GameObject g = Instantiate(grenade, new Vector3(shootOrigin.position.x, shootOrigin.position.y, shootOrigin.position.z), Quaternion.identity);
        g.transform.LookAt(player.transform);
        Grenade grenadeParams = g.GetComponent<Grenade>();
        grenadeParams.SetForce(1);
        /*
        grenadeParams.SetDamage(damage);
        grenadeParams.SetLaser(false);
        grenadeParams.owner = Bullet.BulletOwner.ENEMY;
        grenadeParams.timeToDestroy = bulletLifetime;
        grenadeParams.SetBulletColors(albedo, emissive);
        */


        numberOfGrenades--;
        if (numberOfGrenades <= 0) canLaunchGrenade = false;

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

    private void TransitionFromIdle(bool condition)
    {

        if (!canLaunchGrenade)
        {
            AuxTransitionFromIdle(condition);
        }
        else
        {
            int number = Random.Range(0, grenadeProbabilityRatio);
            Debug.Log(number);
            if (inAttackRange && number == 0)
            {
                 Debug.Log("hA ENTRADO Y EL NUMERO VALE ;"+number);  
                animator.SetTrigger("grenade");
                currentState = gunnerState.GRENADE;
            }
            else
            {
                AuxTransitionFromIdle(condition);
            }

        }
    }
    private void AuxTransitionFromIdle(bool condition)
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
    private void TransitionFromPursue(bool condition)
    {
        if (condition)
        {
            TransitionToIdle();
        }

    }
    private void TransitionToIdle()
    {
        currentState = gunnerState.IDLE;
        animator.SetBool("Moving", false);
        agent.isStopped = true;
    }



    private void ResetAttack()
    {
        alreadyAttacked = false;
        if (currentState == gunnerState.RECHARGE)
        {
            alreadyRecharged = true;
            bulletsInBurst = bulletsPerBurst;
        }

        }

    public void GrenadeLaunched()
    {
        TransitionToIdle();
        Debug.Log("finished");
    }


}
