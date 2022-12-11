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
    public bool canMoveWhileShooting;

    [Header("GRENADE")]
    public bool enableGrenades = false;
    bool canLaunchGrenade = false;
    [Tooltip("The higher the less probability")]
    [SerializeField] [Range(1, 10)] int grenadeProbabilityRatio;
    public int numberOfGrenades = 3;
    [SerializeField] Transform grenadeThrowPoint;
    public int grenadeDamage = 30;
    public float explosionRatio = 1;
    public float timeUntilExplosion = 1f;
    private float throwDistance = 4f;

    public int failOffset = 3;



    [Header("GENERAL")]

    private bool isShotgun = false;
    public bool enableShotgun = false;
    public int numBulletsAtTime = 4;
    [SerializeField] [Range(0, 10)] private float spread = 0.1f;
    public float spotDistance = 7f;
    public float shotgunSpotDistance = 4f;
    public float bulletLifetimeShotGun = 0.8f;

    public Transform shootOrigin;
    [SerializeField] ParticleSystem muzzleVFX;
    [SerializeField] GameObject weapon1;
    [SerializeField] GameObject weapon2;

    private int bulletsInMag;
    private int bulletsInBurst;

    private bool playerOnSight = false;

    private bool alreadyAttacked = false;
    private bool inAttackRange = false;

    private float distance;
    private float speed;

    public GameObject bullet;
    public GameObject grenade;

    Transform player;
    NavMeshAgent agent;
    Animator animator;
    AudioSource gunAudio;
    AudioSource shotgunAudio;
    AudioSource reloadAudio;
    AudioSource walkAudio;

    private int upgradeLifetime = 10;

    private enum gunnerState { IDLE, PURSUE, ATTACK, RECHARGE, GRENADE };
    private gunnerState currentState = gunnerState.IDLE;


    // Bullet colors
    [SerializeField] private Color albedo;
    [SerializeField] private Color emissive;

    // Upgrade
    bool upgraded = false;
    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        var sources = GetComponents<AudioSource>();
        gunAudio = sources[0];
        reloadAudio = sources[1];
        walkAudio = sources[2];
        shotgunAudio = sources[3];

        if (enableShotgun)
        {
            if (Random.Range(0, 3) == 0)
            {
                isShotgun = true;
            }

            if (isShotgun)
            {
                spotDistance = shotgunSpotDistance;
                weapon1.SetActive(false);
                weapon2.SetActive(true);
                cadenceTime = 1.8f;
                reloadTime = 3f;
                SemiAutoTime *= 1.8f;
                bulletsPerMag = 6;
                bulletsInBurst = 1;

            }
        }
        bulletsInMag = bulletsPerMag;
        bulletsInBurst = bulletsPerBurst;

        speed = agent.speed;
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
                    if (enableGrenades)
                    {
                        if (!playerOnSight && canLaunchGrenade)
                        {
                            animator.SetTrigger("grenade");
                            canLaunchGrenade = false;
                            animator.SetBool("Moving", false);
                            agent.isStopped = true;
                            currentState = gunnerState.GRENADE;

                        }
                    }
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
        if (!isShotgun)
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
        }
        else
        {
            shotgunAudio.Play();
            animator.SetTrigger("shoot");


            for (int i = 0; i < numBulletsAtTime; i++)
            {
                // Cálculo de spread
                float spreadRatio = Random.Range(-spread, spread);
                // Cálculo de la nueva dirección con spread
                Vector3 directionWithSpread = transform.forward + new Vector3(spreadRatio, spreadRatio * 0.5f, 0);
                GameObject b = Instantiate(bullet, new Vector3(shootOrigin.position.x, shootOrigin.position.y, shootOrigin.position.z), Quaternion.identity);
                b.transform.localScale *= 0.9f;
                b.transform.forward = directionWithSpread.normalized;
                Bullet bulletParams = b.GetComponent<Bullet>();
                bulletParams.SetForce(directionWithSpread.normalized, bulletSpeed * 1.5f);
                bulletParams.SetDamage(10);

                bulletParams.SetLaser(false);
                bulletParams.owner = Bullet.BulletOwner.ENEMY;
                bulletParams.timeToDestroy = bulletLifetimeShotGun;
                bulletParams.SetBulletColors(albedo, emissive);
            }
        }

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
                    if (Random.Range(0, grenadeProbabilityRatio) == 0 && numberOfGrenades > 0 && distance > throwDistance)
                    {
                        if (!hasToSeeYouToShoot)
                        {
                            CheckPlayerSighted();
                            if (!playerOnSight)
                            {
                                playerOnSight = true;
                                canLaunchGrenade = true;
                            }
                        }
                        else
                        {
                            canLaunchGrenade = true;
                        }
                    }
                TransitionToIdle();
            }
        else
        {
            reloadAudio.Play();
            TransitionToRecharge();
        }



    }

    public void LaunchGrenade()
    {

        GameObject g = Instantiate(grenade, new Vector3(grenadeThrowPoint.position.x, grenadeThrowPoint.position.y, grenadeThrowPoint.position.z), Quaternion.identity);

        Grenade grenadeParams = g.GetComponent<Grenade>();
        //Compose target
        Vector3 target = new Vector3(Random.Range(player.transform.position.x - failOffset, player.transform.position.x + failOffset), player.transform.position.y, Random.Range(player.transform.position.z - failOffset, player.transform.position.z + failOffset));
        grenadeParams.target = target;
        grenadeParams.damage = grenadeDamage;
        grenadeParams.setExplosionRatio(explosionRatio);
        grenadeParams.timeUntilExplosion = timeUntilExplosion;


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
        walkAudio.Pause();
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
        walkAudio.Play();
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

    public void UpgradeAttackSpeed()
    {
        if (!upgraded)
        {
            cadenceTime *= 0.5f;
            reloadTime *= 0.5f;
            SemiAutoTime *= 0.5f;

            agent.speed *= 1.5f;

            upgraded = true;
            Invoke(nameof(ResetAttackSpeed), upgradeLifetime);
        }
    }
    private void ResetAttackSpeed()
    {
        cadenceTime *= 2;
        reloadTime *= 2;
        SemiAutoTime *= 2;

        agent.speed = speed;
        upgraded = false;
    }

}
