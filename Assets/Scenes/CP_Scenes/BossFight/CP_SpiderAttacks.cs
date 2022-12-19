using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CP_SpiderAttacks : MonoBehaviour
{

    private GameObject player;

    [Header("Components")]

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyHealth eh;
    [SerializeField] private BossFightManager bfm;
    [SerializeField] private CapsuleCollider enemyColl;
    [SerializeField] private SkinnedMeshRenderer smr;
    [SerializeField] private GameObject miniGun;
    [SerializeField] private GameObject shootOrigin;
    [SerializeField] ParticleSystem muzzleVFX;

    [Header("Stats")]

    public float timeBetweenAttacks;
    public float attackRange;

    [Header("Minigun")]

    public int mg_damage;
    public float mg_burstCadence;
    public float mg_bulletSpeed;
    public int mg_bulletsPerBurst;
    [SerializeField] private GameObject bullet;
    // Bullet colors
    [SerializeField] private Color albedo;
    [SerializeField] private Color emissive;
    private float bulletLifetime = 3f;
    //Audio
    [SerializeField] private AudioSource gunAudio;

    [Header("Grenade")]

    public int gr_damage;
    public float gr_burstCadence;
    public int gr_bulletsPerBurst;
    public float gr_timeUntilExplosion;
    public int gr_failOffset;
    private float explosionRatio = 1;
    [SerializeField] private GameObject grenade;
    [SerializeField] Transform gr_ThrowPoint;

    [Header("Missile")]

    public int mi_damage;
    public float mi_burstCadence;
    public float mi_bulletSpeed;
    public int mi_bulletsPerBurst;
    public int mi_failOffset;
    [SerializeField] private GameObject missile;
    [SerializeField] Transform mi_ThrowPoint;


    private GameObject currentTarget;

    private int bulletsInMgBurst;
    private int bulletsInGrBurst;
    private int bulletsInMiBurst;

    private bool currentlyInMgBurst = false;
    private bool currentlyInGrBurst = false;
    private bool currentlyInMiBurst = false;

    //Percepciones
    private bool alreadyAttacked = false;
    private bool inRange = false;
    private bool canAttack = false;

    //Estados
    private enum spiderState { IDLE, PURSUE, ATTACK, DEAD };
    private spiderState currentState = spiderState.IDLE;

    
    

    

    // Upgrade
    bool upgraded = false;

    [Header("Death")]

    //DeathExplosion
    [SerializeField] private ParticleSystem explosionVfx;
    [SerializeField] private GameObject explosionColl;
    [SerializeField] private GameObject explosionRange;

    [Space]
    public AudioClip tictac, boom;
    private AudioSource audioSource;

    [SerializeField] private LayerMask m_LayerMask;

    private float distance;
    private float lastGrenadeTime;
    private float maxGrenadesTime = 7f;
    private float lastMissileTime;
    private float maxMissileTime = 30f;
    private float playerOnSight = 0f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = gameObject.GetComponent<AudioSource>();

        currentTarget = player;

        eh.onDeath += Death;

        Phase1();

        bulletsInMgBurst = mg_bulletsPerBurst;
        bulletsInGrBurst = gr_bulletsPerBurst;
        bulletsInMiBurst = mi_bulletsPerBurst;

        lastGrenadeTime = 0f;
        lastMissileTime = 0f;

        
    }

    private void Update()
    {
        FSM();
    }

    private void FSM()
    {

        distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= attackRange)
            inRange = true;
        else
            inRange = false;

        if (inRange && !alreadyAttacked)
            canAttack = true;

        lastGrenadeTime += Time.deltaTime;
        lastMissileTime += Time.deltaTime;

        CheckPlayerSighted();

        switch (currentState)
        {
            case spiderState.IDLE:

                IdleAction();

                if(currentlyInMgBurst || currentlyInGrBurst || currentlyInMiBurst)
                {
                    if (!alreadyAttacked)
                    {
                        currentState = spiderState.ATTACK;
                    }
                    break;
                }

                if (canAttack && inRange)
                {
                    currentState = spiderState.ATTACK;
                }
                else if (!inRange)
                {
                    currentState = spiderState.PURSUE;
                }

                break;

            case spiderState.PURSUE:

                PursueAction();

                if (!canAttack && inRange)
                {
                    currentState = spiderState.IDLE;
                }
                else if (inRange && canAttack)
                {
                    currentState = spiderState.ATTACK;
                }

                break;

            case spiderState.ATTACK:

                AttackAction();

                if (currentlyInMgBurst || currentlyInGrBurst || currentlyInMiBurst)
                {
                    currentState = spiderState.IDLE;
                    break;
                }

                if (alreadyAttacked && inRange)
                {
                    currentState = spiderState.IDLE;
                }
                else if (alreadyAttacked && !inRange)
                {
                    currentState = spiderState.PURSUE;
                }

                break;

            case spiderState.DEAD:

                break;
        }
    }

    private void IdleAction()
    {
        TrackSpider();
        TrackMinigun();
    }

    private void PursueAction()
    {
        agent.isStopped = false;
        agent.SetDestination(currentTarget.transform.position);
        TrackSpider();
        TrackMinigun();
    }

    private void AttackAction()
    {
        agent.isStopped = true;

        if (currentlyInMgBurst)
        {
            ShootMinigun();
            return;
        }
        else if (currentlyInGrBurst)
        {
            LaunchGrenade();
            return;
        }
        else if (currentlyInMiBurst)
        {
            LaunchMissile();
            return;
        }

        US();

    }

    private void US()
    {
        float SGHT = playerOnSight;

        float DIST = ComputeDistanceToEnemy();

        float TGRA = ComputeTimeSinceLastGrenade();

        float TMIS = ComputeTimeSinceLastMissile();

        float shootAction = DIST * SGHT;

        float grenadeAction = 0.95f * DIST + 0.05f * TGRA;

        float missileAction = 0.95f * DIST + 0.05f * TMIS;

        if (missileAction >= shootAction && missileAction >= grenadeAction)
        {
            LaunchMissile();
        }
        else if(grenadeAction >= shootAction && grenadeAction >= missileAction)
        {
            LaunchGrenade();
        }
        else
        {
            ShootMinigun();
        }

    }

    private void TrackMinigun()
    {
        miniGun.transform.LookAt(player.transform.position + Vector3.up * 0.5f);
    }

    private void TrackSpider()
    {
        transform.LookAt(player.transform.position);
    }

    private void ShootMinigun()
    {
        CreateBullet();

        alreadyAttacked = true;
        canAttack = false;
        currentlyInMgBurst = true;

        gunAudio.Play();
        muzzleVFX.Play();

        bulletsInMgBurst--;

        if (bulletsInMgBurst > 0)
            Invoke(nameof(ResetAttack), mg_burstCadence);
        else
        {
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            bulletsInMgBurst = mg_bulletsPerBurst;
            currentlyInMgBurst = false;
            gunAudio.Stop();
        }
    }

    private void CreateBullet()
    {
        GameObject b = Instantiate(bullet, new Vector3(shootOrigin.transform.position.x, shootOrigin.transform.position.y, shootOrigin.transform.position.z), Quaternion.identity);
        b.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        b.transform.forward = miniGun.transform.forward;
        Bullet bulletParams = b.GetComponent<Bullet>();
        bulletParams.SetForce(mg_bulletSpeed);
        bulletParams.SetDamage(mg_damage);
        bulletParams.SetLaser(false);
        bulletParams.owner = Bullet.BulletOwner.ENEMY;
        bulletParams.timeToDestroy = bulletLifetime;
        bulletParams.SetBulletColors(albedo, emissive);
    }

    public void LaunchGrenade()
    {
        CreateGrenade();

        alreadyAttacked = true;
        canAttack = false;
        currentlyInGrBurst = true;

        lastGrenadeTime = 0f;

        bulletsInGrBurst--;

        if (bulletsInGrBurst > 0)
            Invoke(nameof(ResetAttack), gr_burstCadence);
        else
        {
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            bulletsInGrBurst = gr_bulletsPerBurst;
            currentlyInGrBurst = false;
        }
    }

    private void CreateGrenade()
    {
        GameObject g = Instantiate(grenade, new Vector3(gr_ThrowPoint.position.x, gr_ThrowPoint.position.y, gr_ThrowPoint.position.z), Quaternion.identity);

        Grenade grenadeParams = g.GetComponent<Grenade>();
        //Compose target
        Vector3 target = new Vector3(Random.Range(player.transform.position.x - gr_failOffset, player.transform.position.x + gr_failOffset), player.transform.position.y, Random.Range(player.transform.position.z - gr_failOffset, player.transform.position.z + gr_failOffset));
        grenadeParams.target = target;
        grenadeParams.damage = gr_damage;
        grenadeParams.setExplosionRatio(explosionRatio);
        grenadeParams.timeUntilExplosion = gr_timeUntilExplosion;
    }

    public void LaunchMissile()
    {
        CreateMissile();

        alreadyAttacked = true;
        canAttack = false;
        currentlyInMiBurst = true;

        lastMissileTime = 0f;

        bulletsInMiBurst--;

        if (bulletsInMiBurst > 0)
            Invoke(nameof(ResetAttack), mi_burstCadence);
        else
        {
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            bulletsInMiBurst = mi_bulletsPerBurst;
            currentlyInMiBurst = false;
        }

        //Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    private void CreateMissile()
    {
        GameObject g = Instantiate(missile, new Vector3(mi_ThrowPoint.position.x, mi_ThrowPoint.position.y, mi_ThrowPoint.position.z), Quaternion.identity);

        Missile missileParams = g.GetComponent<Missile>();
        //Compose target
        Vector3 target = new Vector3(Random.Range(player.transform.position.x - mi_failOffset, player.transform.position.x + mi_failOffset), player.transform.position.y, Random.Range(player.transform.position.z - mi_failOffset, player.transform.position.z + mi_failOffset));
        missileParams.target = target;
        missileParams.damage = mi_damage;
        missileParams.flySpeed = mi_bulletSpeed;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private float ComputeDistanceToEnemy()
    {
        float x = Mathf.InverseLerp(0, attackRange, distance);

        float y = 1 / (1 + Mathf.Pow((0.9f / x) - 1, -5));

        return y;
    }

    private float ComputeTimeSinceLastGrenade()
    {
        float x = Mathf.InverseLerp(0, maxGrenadesTime, lastGrenadeTime);

        float y = Mathf.Pow(x, 8);

        return y;
    }

    private float ComputeTimeSinceLastMissile()
    {
        float x = Mathf.InverseLerp(0, maxMissileTime, lastMissileTime);

        float y = Mathf.Pow(x, 8);

        return y;
    }

    private void Death()
    {
        agent.enabled = false;
        eh.enabled = false;
        enemyColl.enabled = false;

        currentState = spiderState.DEAD;

        bfm.WinLevel();

        Destroy(gameObject, 1f);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void CheckPlayerSighted()
    {
        Ray ray = new Ray(transform.position, (player.transform.position - transform.position).normalized);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.tag == "Player")
                playerOnSight = 1f;
            else
                playerOnSight = 0f;
        }
    }

    public void SetPhase(int i)
    {
        switch (i)
        {
            case 0:
                Phase1();
                break;
            case 1:
                Phase2();
                break;
            case 2:
                Phase3();
                break;
        }
            
    }
    
    private void Phase1()
    {
        timeBetweenAttacks = 2f;

        //minigun
        mg_damage = 2;
        mg_burstCadence = 0.2f;
        mg_bulletSpeed = 20;
        mg_bulletsPerBurst = 10;

        //granadas
        gr_damage = 5;
        gr_burstCadence = 1;
        gr_bulletsPerBurst = 1;
        gr_timeUntilExplosion = 1.5f;
        gr_failOffset = 4;

        //misiles
        mi_damage = 10;
        mi_burstCadence = 1;
        mi_bulletSpeed = 0.5f;
        mi_bulletsPerBurst = 1;
        mi_failOffset = 3;
    }
    private void Phase2()
    {
        timeBetweenAttacks = 1.5f;

        //minigun
        mg_damage = 3;
        mg_burstCadence = 0.1f;
        mg_bulletSpeed = 30;
        mg_bulletsPerBurst = 20;

        //granadas
        gr_damage = 10;
        gr_burstCadence = 0.6f;
        gr_bulletsPerBurst = 3;
        gr_timeUntilExplosion = 1f;
        gr_failOffset = 3;

        //misiles
        mi_damage = 15;
        mi_burstCadence = 0.6f;
        mi_bulletSpeed = 0.8f;
        mi_bulletsPerBurst = 2;
        mi_failOffset = 2;
    }
    private void Phase3()
    {
        timeBetweenAttacks = 1f;

        //minigun
        mg_damage = 5;
        mg_burstCadence = 0.05f;
        mg_bulletSpeed = 50;
        mg_bulletsPerBurst = 30;

        //granadas
        gr_damage = 15;
        gr_burstCadence = 0.3f;
        gr_bulletsPerBurst = 7;
        gr_timeUntilExplosion = 0.5f;
        gr_failOffset = 1;

        //misiles
        mi_damage = 20;
        mi_burstCadence = 0.2f;
        mi_bulletSpeed = 1.2f;
        mi_bulletsPerBurst = 5;
        mi_failOffset = 0;
    }

}
