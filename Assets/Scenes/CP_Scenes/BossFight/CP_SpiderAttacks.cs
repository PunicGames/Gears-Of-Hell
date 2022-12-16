using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CP_SpiderAttacks : MonoBehaviour
{

    private GameObject player;

    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private EnemyHealth eh;
    [SerializeField] private CapsuleCollider enemyColl;
    [SerializeField] private SkinnedMeshRenderer smr;

    [SerializeField] private GameObject miniGun;
    [SerializeField] private GameObject shootOrigin;
    [SerializeField] ParticleSystem muzzleVFX;

    public float burstCadence = 1f;
    public float bulletSpeed = 2;
    public float damage = 10;
    public float bulletLifetime = 3f;

    public float timeBetweenBursts = 2f;

    [SerializeField] private int bulletsPerBurst;

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject grenade;
    [SerializeField] private GameObject missile;

    [SerializeField] Transform grenadeThrowPoint;
    public int grenadeDamage = 80;
    public float explosionRatio = 1;
    public float timeUntilExplosion = 1f;
    private float throwDistance = 4f;
    public int failOffset = 3;

    [SerializeField] private float attackRange;

    private GameObject currentTarget;

    private int bulletsInBurst;

    private bool currentlyInBurst = false;

    //Percepciones
    private bool alreadyAttacked = false;
    private bool inRange = false;
    private bool canAttack = false;

    //Estados
    private enum spiderState { IDLE, PURSUE, ATTACK, DEAD };
    private spiderState currentState = spiderState.IDLE;

    // Bullet colors
    [SerializeField] private Color albedo;
    [SerializeField] private Color emissive;

    //Audio
    [SerializeField] private AudioSource gunAudio;

    // Upgrade
    bool upgraded = false;

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

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = gameObject.GetComponent<AudioSource>();

        currentTarget = player;

        eh.onDeath += Death;

        bulletsInBurst = bulletsPerBurst;

        lastGrenadeTime = 0;

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

        switch (currentState)
        {
            case spiderState.IDLE:

                IdleAction();

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

        if (currentlyInBurst)
        {
            ShootMinigun();
            return;
        }

        US();

    }

    private void US()
    {
        float DIST = ComputeDistanceToEnemy();

        float TGRA = ComputeTimeSinceLastGrenade();

        float shootAction = DIST;

        float grenadeAction = 0.7f * DIST + 0.3f * TGRA;

        if (grenadeAction >= shootAction)
        {
            //LaunchGrenade();
            LaunchMissile();
        }
        else
        {
            //ShootMinigun();
            LaunchMissile();
        }
    }

    private void TrackMinigun()
    {
        miniGun.transform.LookAt(player.transform.position + new Vector3(0f, 1f, 0f));
    }

    private void TrackSpider()
    {
        transform.LookAt(player.transform.position);
    }

    private void ShootMinigun()
    {
        GameObject b = Instantiate(bullet, new Vector3(shootOrigin.transform.position.x, shootOrigin.transform.position.y, shootOrigin.transform.position.z), Quaternion.identity);
        b.transform.LookAt(player.transform);
        Bullet bulletParams = b.GetComponent<Bullet>();
        bulletParams.SetForce(bulletSpeed);
        bulletParams.SetDamage(damage);
        bulletParams.SetLaser(false);
        bulletParams.owner = Bullet.BulletOwner.ENEMY;
        bulletParams.timeToDestroy = bulletLifetime;
        bulletParams.SetBulletColors(albedo, emissive);

        alreadyAttacked = true;
        canAttack = false;
        currentlyInBurst = true;
        gunAudio.Play();
        muzzleVFX.Play();

        bulletsInBurst--;

        if (bulletsInBurst > 0)
            Invoke(nameof(ResetAttack), burstCadence);
        else
        {
            Invoke(nameof(ResetAttack), timeBetweenBursts);
            bulletsInBurst = bulletsPerBurst;
            currentlyInBurst = false;
            gunAudio.Stop();
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

        alreadyAttacked = true;
        canAttack = false;

        lastGrenadeTime = 0;

        Invoke(nameof(ResetAttack), timeBetweenBursts);

    }

    public void LaunchMissile()
    {
        GameObject g = Instantiate(missile, new Vector3(grenadeThrowPoint.position.x, grenadeThrowPoint.position.y, grenadeThrowPoint.position.z), Quaternion.identity);

        //Missile missileParams = missile.GetComponent<Missile>();

        //missileParams.target = player.transform.position;


        /////////////////////////////////////
        alreadyAttacked = true;
        canAttack = false;

        Invoke(nameof(ResetAttack), timeBetweenBursts);
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

    private void Death()
    {
        agent.enabled = false;
        eh.enabled = false;
        enemyColl.enabled = false;

        currentState = spiderState.DEAD;

        TriggerExplosion();

        print("dead");

        //Destroy(gameObject, 0f);
    }

    private void TriggerExplosion()
    {
        agent.enabled = false;
        eh.enabled = false;
        enemyColl.enabled = false;

        explosionRange.SetActive(true);

        //Playing 'tictac'
        audioSource.clip = tictac;
        audioSource.loop = true;
        audioSource.Play();

        Invoke("Explode", timeUntilExplosion);
        enabled = false;
    }

    private void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(explosionColl.transform.position, explosionColl.GetComponent<SphereCollider>().radius * explosionColl.transform.localScale.x * transform.localScale.x, m_LayerMask, QueryTriggerInteraction.Ignore);
        foreach (var hc in hitColliders)
        {
            if (hc.tag == "Enemy")
            {
                hc.GetComponent<EnemyHealth>().TakeDamage(grenadeDamage);
            }
            else if (hc.tag == "Player")
            {
                hc.GetComponent<Health>().TakeDamage(grenadeDamage);
            }
        }
        explosionRange.SetActive(false);

        //Playing Booom
        audioSource.clip = boom;
        audioSource.loop = false;
        audioSource.Play();

        explosionVfx.Play();
        smr.enabled = false;
        Destroy(gameObject, explosionVfx.main.duration);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void UpgradeAttackSpeed()
    {
        if (!upgraded)
        {
            burstCadence /= 2;
            timeBetweenBursts /= 2;
            // TODO: Modify variable values to get enhanced version.
            upgraded = true;
        }
    }
}
