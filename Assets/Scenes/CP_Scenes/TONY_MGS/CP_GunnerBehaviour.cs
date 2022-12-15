using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CP_GunnerBehaviour : MonoBehaviour
{
    #region inEditorVariables

    [Header("GENERAL")]

    [Space]

    public float combatWalkingSpeed = 4f;
    public float patrollingWalkingSpeed = 1f;
    public float patrolIdleTime = 5f;

    public Transform shootOrigin;

    [SerializeField] List<Transform> patrolPoints = new List<Transform>();


    [SerializeField] ParticleSystem muzzleVFX;
    [SerializeField] private ParticleSystem upgradeVFX;
    [SerializeField] private GameObject alertVFX;
    [SerializeField] private GameObject spotVFX;
    [SerializeField] GameObject weapon1;
    [SerializeField] GameObject weapon2;
    [Space]
    public GameObject bullet;
    public GameObject grenade;

    [Header("BULLET COLORS")]
    // Bullet colors
    [SerializeField] private Color albedo;
    [SerializeField] private Color emissive;
    // Upgrade
    bool upgraded = false;
    [Space]
    [Header("COMBAT")]
    [Space]
    public float cadenceTime = 1f;
    public float reloadTime = 2f;
    public float bulletSpeed = 2;
    public float damage = 10;
    public int bulletsPerMag = 5;
    public int bulletsPerBurst = 5;
    public float SemiAutoTime = 2f;
    public float bulletLifetime = 3f;
    public bool hasToSeeYouToShoot = false;
    public bool canMoveWhileShooting;

    public bool enableGrenades = false;
    bool canLaunchGrenade = false;
    [Tooltip("The higher the less probability")]
    [SerializeField] [Range(1, 10)] int grenadeProbabilityRatio;
    public int numberOfGrenades = 3;
    [SerializeField] Transform grenadeThrowPoint;
    public int grenadeDamage = 30;
    public float explosionRatio = 1;
    public float timeUntilExplosion = 1f;
    public int failOffset = 3;

    [Space]

    public bool enableShotgun = false;
    public float bulletLifetimeShotGun = 0.8f;
    public float timeBetweenShotgunShots = 1.4f;
    [Range(45, 180)] public float coneApertureInDegrees = 90;

    #endregion
    private float spotDistance;
    private float shotgunSpotDistance;

    private float idleBlend = 1f;

    private int nextPatrolPoint = 0;
    private bool isCalm = true;
    private bool hasReachedPoint = true;


    private int numBulletsAtTime = 4;
    private bool isShotgun = false;
    private readonly float throwDistance = 4f;
    private int bulletsInMag;
    private int bulletsInBurst;
    private bool playerOnSight = false;
    private bool alreadyAttacked = false;
    private bool inAttackRange = false;
    private float distance;

    EnemyVision vision;
    Transform player;
    NavMeshAgent agent;
    Animator animator;
    EnemySoundManager soundManager;

    private Vector3 playerLastSeenPos;

    private int upgradeLifetime = 10;

    private enum combatState { IDLE, PURSUE, ATTACK, RECHARGE, GRENADE };
    private combatState currenCombatState = combatState.IDLE;

    private enum standardState { IDLE, PATROLLING, COMBAT, ALERTED, ALERTED_IDLE, HQ_CALL };
    private standardState currenState = standardState.IDLE;
    private float speed;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetFloat("walking_speed", patrollingWalkingSpeed);
        //animator.SetFloat("check_time", patrolIdleTime);
        animator.SetFloat("idleBlend", idleBlend);

        vision = GetComponent<EnemyVision>();

        vision.onAlert += TransitionToAlert;
        vision.onSpot += TransitionToCombat;

        spotDistance = vision.perceptionRadius;
        shotgunSpotDistance = vision.perceptionRadius * .75f;

        soundManager = GetComponent<EnemySoundManager>();

        //IF SHOTGUN CHANCE ENABLED
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
                cadenceTime = timeBetweenShotgunShots;
                reloadTime = 3f;
                SemiAutoTime = timeBetweenShotgunShots;
                bulletsPerMag = 6;
                bulletsPerBurst = 1;

            }
        }

        bulletsInMag = bulletsPerMag;
        bulletsInBurst = bulletsPerBurst;

        agent.speed = patrollingWalkingSpeed;
        speed = agent.speed;
    }

    private void Update()
    {
        StandardFSM();
    }

    private void CombatFSM()
    {
        distance = Vector3.Distance(player.position, transform.position);
        if (distance <= spotDistance)
            inAttackRange = true;
        else
            inAttackRange = false;



        switch (currenCombatState)
        {
            case combatState.IDLE:

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
            case combatState.PURSUE:
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
                            currenCombatState = combatState.GRENADE;

                        }
                    }
                    TransitionFromPursue(inAttackRange && playerOnSight);
                }
                else
                {
                    TransitionFromPursue(inAttackRange);
                }
                break;

            case combatState.ATTACK:
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
                            currenCombatState = combatState.GRENADE;
                        }
                        else
                        {
                            Attack();
                        }
                    }
                }
                break;
            case combatState.RECHARGE:
                if (inAttackRange)
                    transform.LookAt(player.position);

                break;
            case combatState.GRENADE:
                transform.LookAt(player.position);
                //Callback return to idle AfterAnim
                break;
            default:
                break;
        }



    }

    private void StandardFSM()
    {

        switch (currenState)
        {
            case standardState.IDLE:
                TransitionToPatrol(patrolIdleTime);

                break;
            case standardState.PATROLLING:
                distance = Vector3.Distance(transform.position, patrolPoints[nextPatrolPoint].position);
                //If reaches point
                if (distance <= 0.1f)
                {
                    TransitionToPatrolIdle();
                    nextPatrolPoint++;
                    nextPatrolPoint = nextPatrolPoint % patrolPoints.Count;
                }

                break;
            case standardState.COMBAT:
                CombatFSM();
                break;
            case standardState.ALERTED:
                distance = Vector3.Distance(transform.position, playerLastSeenPos);
                Debug.Log(distance);
                if (distance <= 0.1f)
                {
                    float nearest = 999f;
                    int index = 0;
                    int nearestIndex = index;
                    foreach (var p in patrolPoints)
                    {
                        float dist = Vector3.Distance(transform.position, p.position);
                        if (dist < nearest) { nearest = dist; nearestIndex = index; }
                        index++;
                    }
                    TransitionToAlertedIdle();
                    nextPatrolPoint = nearestIndex;
                }
                break;
            case standardState.ALERTED_IDLE:
                TransitionToPatrol(patrolIdleTime * 1.5f);
                break;
            case standardState.HQ_CALL:
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
            soundManager.OverlapedPlaySound("shoot");
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
            soundManager.OverlapedPlaySound("shoot2");
            animator.SetTrigger("shoot");

            float startAngle = coneApertureInDegrees * 0.5f;
            float partialAngle = coneApertureInDegrees * 0.33f; //equal as divide by 3 but faster

            for (int i = 0; i < numBulletsAtTime; i++)
            {

                Vector3 directionWithSpread = Quaternion.AngleAxis(startAngle, transform.up) * transform.forward;

                GameObject b = Instantiate(bullet, new Vector3(shootOrigin.position.x, shootOrigin.position.y, shootOrigin.position.z), Quaternion.identity);
                b.transform.localScale *= 0.9f;
                b.transform.forward = directionWithSpread.normalized;
                Bullet bulletParams = b.GetComponent<Bullet>();
                bulletParams.SetForce(directionWithSpread.normalized, bulletSpeed * 1.5f);
                bulletParams.SetDamage(8);

                bulletParams.SetLaser(false);
                bulletParams.owner = Bullet.BulletOwner.ENEMY;
                bulletParams.timeToDestroy = bulletLifetimeShotGun;
                bulletParams.SetBulletColors(albedo, emissive);
                startAngle -= partialAngle;
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
            soundManager.PlaySound("reload");
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
    #region combatTransitions
    private void TransitionFromIdle(bool condition)
    {
        if (condition)
        {
            TransitionToPursue();
        }
        else if (!alreadyAttacked)
        {
            currenCombatState = combatState.ATTACK;

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
        soundManager.PauseSound("walk");
        currenCombatState = combatState.IDLE;
        animator.SetBool("Moving", false);
        if (agent.enabled)
            agent.isStopped = true;
    }
    private void TransitionToRecharge()
    {
        animator.SetTrigger("reload");
        Invoke(nameof(ResetAttack), reloadTime);
        bulletsInMag = bulletsPerMag;
        currenCombatState = combatState.RECHARGE;
    }

    private void TransitionToPursue()
    {
        soundManager.PlaySound("walk");
        currenCombatState = combatState.PURSUE;
        animator.SetBool("Moving", true);
        agent.isStopped = false;
    }


    public void GrenadeLaunched()
    {
        TransitionToIdle();

    }
    #endregion
    #region utilityCombatFunctions
    private void ResetAttack()
    {
        alreadyAttacked = false;
        if (currenCombatState == combatState.RECHARGE)
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
        upgradeVFX.Play();
        upgradeVFX.loop = true;
    }
    #endregion
    #region standardTransitions
    void TransitionToPatrol(float time)
    {
        Invoke(nameof(PatrolToPoint), time);
    }
    void TransitionToPatrolIdle()
    {

        currenState = standardState.IDLE;
        agent.isStopped = true;
        animator.SetBool("Walking", false);
    }
    void TransitionToAlert(Vector3 lastSeenPos)
    {
        CancelInvoke(nameof(PatrolToPoint));
        animator.SetBool("Walking", true);
        currenState = standardState.ALERTED;
        agent.isStopped = false;
        agent.speed = patrollingWalkingSpeed * 1.5f;
        animator.SetFloat("walking_speed", agent.speed);
        playerLastSeenPos = lastSeenPos;
        agent.SetDestination(playerLastSeenPos);

        alertVFX.SetActive(true);
    }
    void TransitionToAlertedIdle()
    {
        currenState = standardState.ALERTED_IDLE;
        agent.isStopped = true;
        animator.SetBool("Walking", false);
        agent.speed = patrollingWalkingSpeed;
        animator.SetFloat("walking_speed", agent.speed);
    }
    void TransitionToCombat()
    {
        currenState = standardState.COMBAT;
        agent.speed = combatWalkingSpeed;
        animator.SetBool("Walking", false);
        idleBlend = 0f;
        animator.SetFloat("idleBlend", idleBlend);


        alertVFX.SetActive(false);

        spotVFX.SetActive(true);
        //Invoke(nameof(StopSpotVFX), 2f);
    }

    #endregion
    #region utilityPatrolFunctions
    void PatrolToPoint()
    {
        CancelInvoke(nameof(PatrolToPoint));

        alertVFX.SetActive(false);
        currenState = standardState.PATROLLING;
        agent.isStopped = false;
        animator.SetBool("Walking", true);
        agent.SetDestination(patrolPoints[nextPatrolPoint].position);
    }
   
    #endregion

}