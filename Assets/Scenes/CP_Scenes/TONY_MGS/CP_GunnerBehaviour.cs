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

    public float soundReach = 15f;

    public Transform shootOrigin;


    [SerializeField] List<Transform> patrolPoints = new List<Transform>();
    [SerializeField] List<Transform> alarms = new List<Transform>();
    [SerializeField] Alarm alarmDevice;
    public bool isReinforcement = false;


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

    private bool hasSpottedPlayer = false;

    EnemyVision vision;
    Transform player;
    NavMeshAgent agent;
    Animator animator;
    EnemySoundManager soundManager;
    SoundEmitter soundEmitter;

    private Vector3 playerLastSeenPos;

    private int upgradeLifetime = 10;

    private enum combatState { IDLE, PURSUE, ATTACK, RECHARGE, GRENADE };
    private combatState currenCombatState = combatState.IDLE;

    private enum standardState { IDLE, PATROLLING, COMBAT, ALERTED, ALERTED_IDLE, SEEK_ALARM };
    private standardState currentState = standardState.IDLE;
    private float speed;
    private int targetMask;

    private void Start()
    {
        //patrollingWalkingSpeed = Random.Range(patrollingWalkingSpeed - 0.2f, patrollingWalkingSpeed + 0.2f);

        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetFloat("walking_speed", patrollingWalkingSpeed);
        animator.SetFloat("idleBlend", idleBlend);

        vision = GetComponent<EnemyVision>();
        soundEmitter = GetComponent<SoundEmitter>();

        vision.onAlert += TransitionToAlert;
        vision.onSpot += TransitionToCombat;
        //vision.onLostingSight += () => { CancelInvoke(nameof(ForgetHeSawPlayer)); Invoke(nameof(ForgetHeSawPlayer), forgetTime); Debug.Log("w"); };

        spotDistance = vision.perceptionRadius;
        shotgunSpotDistance = vision.perceptionRadius * .75f;

        soundManager = GetComponent<EnemySoundManager>();

        alarmDevice = (Alarm)FindObjectOfType(typeof(Alarm));
        alarmDevice.onStartAlarm += ReinforcePosition;
        if (isReinforcement)
        {
            alarmDevice.onStopAlarm += ReturnToPost;
        }

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

        if (!isReinforcement)
            Invoke(nameof(PatrolToPoint), 2f);
    }

    private void Update()
    {
        StandardFSM();
        Debug.Log(currentState);
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

        switch (currentState)
        {
            case standardState.IDLE:

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
                if (distance <= 0.1f)
                {
                    if (!isReinforcement)
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
                        nextPatrolPoint = nearestIndex;
                    }
                    TransitionToAlertedIdle();
                }
                break;
            case standardState.ALERTED_IDLE:
                break;

            case standardState.SEEK_ALARM:
                distance = Vector3.Distance(transform.position, agent.destination);
                if (distance <= 0.2f)
                {
                    alarmDevice.SoundAlarm(playerLastSeenPos);
                    TransitionToAlert(playerLastSeenPos, true);
                }
                break;
            default:
                break;
        }

    }

    #region actions
    void Attack()
    {
        soundEmitter.MakeSound(15f);

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
        soundManager.PauseSound("run");
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
        soundManager.PlaySound("run");
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

        currentState = standardState.IDLE;
        agent.isStopped = true;
        animator.SetBool("Walking", false);
        animator.SetFloat("check_time", 1f);
        TransitionToPatrol(patrolIdleTime);

        StartCoroutine(LerpTurnTo(1f, patrolPoints[nextPatrolPoint].forward));
    }

    public void TransitionToAlert(Vector3 lastSeenPos, bool t)
    {

        CancelInvoke(nameof(PatrolToPoint));
        CancelInvoke(nameof(TransitionToSeekAlarm));
        StopAllCoroutines();

        agent.speed = patrollingWalkingSpeed * 1.5f;
        if (t)
        {
            if (isReinforcement && alarmDevice.isActive)
            {
                animator.SetBool("Moving", true);
                animator.SetBool("Walking", false);
                agent.speed = patrollingWalkingSpeed * 2f;
            }
            else
            {
                animator.SetBool("Moving", false);
                animator.SetBool("Walking", true);
            }
            int r = Random.Range(0, 2);
            //Randomly choose sound
            switch (r)
            {
                case 0:
                    soundManager.OverlapedPlaySound("alert1");
                    break;
                case 1:
                    soundManager.OverlapedPlaySound("alert3");
                    break;
                case 2:
                    soundManager.OverlapedPlaySound("alert2");
                    break;

            }
        }
        else
        {
            animator.SetBool("Moving", true);
            animator.SetBool("Walking", false);
            hasSpottedPlayer = true;

            //Where did he go sound
        }
        idleBlend = 1f;
        animator.SetFloat("idleBlend", idleBlend);


        currentState = standardState.ALERTED;
        animator.SetFloat("walking_speed", agent.speed);
        playerLastSeenPos = lastSeenPos;
        agent.SetDestination(playerLastSeenPos);
        agent.isStopped = false;

        spotVFX.SetActive(false);
        alertVFX.SetActive(true);
    }
    void TransitionToAlertedIdle()
    {

        animator.SetFloat("check_time", 2f);
        currentState = standardState.ALERTED_IDLE;
        agent.isStopped = true;

        animator.SetBool("Walking", false);
        animator.SetBool("Moving", false);



        if (hasSpottedPlayer && !alarmDevice.isActive) //y alarma desactivada
        {
            Invoke(nameof(TransitionToSeekAlarm), patrolIdleTime);
        }
        else
        {
            agent.speed = patrollingWalkingSpeed;
            animator.SetFloat("walking_speed", agent.speed);

            if (isReinforcement && alarmDevice.isActive)
                return;
            else
                TransitionToPatrol(patrolIdleTime);

        }

    }
    void TransitionToCombat()
    {
        CancelInvoke(nameof(PatrolToPoint));
        CancelInvoke(nameof(TransitionToSeekAlarm));
        StopAllCoroutines();

        soundManager.OverlapedPlaySound("!");
        int r = Random.Range(0, 3);
        //Randomly choose sound
        switch (r)
        {
            case 0:
                soundManager.OverlapedPlaySound("intruder");
                break;
        }

        currentState = standardState.COMBAT;
        agent.speed = combatWalkingSpeed;
        animator.SetBool("Walking", false);
        idleBlend = 0f;
        animator.SetFloat("idleBlend", idleBlend);


        alertVFX.SetActive(false);
        spotVFX.SetActive(true);
    }

    void TransitionToSeekAlarm()
    {
        //si alarma desactivada
        agent.speed = patrollingWalkingSpeed * 2f;
        agent.isStopped = false;
        animator.SetFloat("walking_speed", agent.speed);
        animator.SetBool("Moving", true);
        animator.SetBool("Walking", false);
        hasSpottedPlayer = false;
        float nearest = 999f;
        int index = 0;
        int nearestIndex = index;
        foreach (var p in alarms)
        {
            float dist = Vector3.Distance(transform.position, p.position);
            if (dist < nearest) { nearest = dist; nearestIndex = index; }
            index++;
        }
        agent.SetDestination(alarms[nearestIndex].position);
        currentState = standardState.SEEK_ALARM;

    }
    #endregion
    #region utilityPatrolFunctions

    IEnumerator LerpTurnTo(float time, Vector3 lookat)
    {
        float timeElapsed = 0;
        Vector3 origin = transform.forward;
        while (timeElapsed < time)
        {
            transform.forward = Vector3.Lerp(origin, lookat, timeElapsed / time);
            timeElapsed += Time.deltaTime;
            yield return null;

        }
        transform.forward = lookat;
    }
    void PatrolToPoint()
    {
        CancelInvoke(nameof(PatrolToPoint));
        if (currentState == standardState.ALERTED_IDLE)
        {
            soundManager.OverlapedPlaySound("clear");
        }
        agent.isStopped = false;
        animator.SetBool("Walking", true);
        alertVFX.SetActive(false);
        currentState = standardState.PATROLLING;
        agent.SetDestination(patrolPoints[nextPatrolPoint].position);
    }
    public void ListenToSound(Vector3 soundOrigin)
    {
        if (currentState != standardState.COMBAT)
        {
            if (currentState != standardState.ALERTED)
                TransitionToAlert(soundOrigin, true);
            else
            {
                playerLastSeenPos = soundOrigin;
                agent.SetDestination(playerLastSeenPos);
            }
        }

    }

    private void ReinforcePosition(Vector3 lastSeenPos)
    {
        if (isReinforcement)
        {
            if (currentState != standardState.COMBAT && currentState != standardState.ALERTED)
            {
                TransitionToAlert(lastSeenPos, true);
            }
        }
        else
        {
            if (currentState == standardState.SEEK_ALARM)
            {
                TransitionToAlert(lastSeenPos, true);
            }
        }
    }
    private void ReturnToPost()
    {
        if (currentState != standardState.COMBAT && currentState != standardState.ALERTED)
        {

            PatrolToPoint();
        }

    }
    #endregion
}