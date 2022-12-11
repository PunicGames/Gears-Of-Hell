using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BigSpiderBotBehaviour : MonoBehaviour
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

    public float timeBetweenBursts = 5f;

    [SerializeField] private int bulletsPerBurst;

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject grenade;

    [SerializeField] private float attackRange;

    private GameObject currentTarget;

    private int bulletsInBurst;

    private bool currentlyInBurst = false;

    //Percepciones
    private bool alreadyAttacked = false;
    private bool inRange = false;
    private bool canAttack = false;

    //Estados
    private enum spiderState { IDLE, PURSUE, ATTACK };
    private spiderState currentState = spiderState.IDLE;

    // Bullet colors
    [SerializeField] private Color albedo;
    [SerializeField] private Color emissive;

    //Audio
    [SerializeField] private AudioSource gunAudio;

    // Upgrade
    bool upgraded = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        currentTarget = player;

        eh.onDeath += Death;

        bulletsInBurst = bulletsPerBurst;

    }

    private void Update()
    {
        FSM();
    }

    private void FSM()
    {

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= attackRange)
            inRange = true;
        else
            inRange = false;

        if (inRange && !alreadyAttacked)
            canAttack = true;


        switch (currentState)
        {
            case spiderState.IDLE:

                IdleAction();

                if(canAttack && inRange)
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

                if(!canAttack && inRange)
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

                if(alreadyAttacked && inRange)
                {
                    currentState = spiderState.IDLE;
                }
                else if (alreadyAttacked && !inRange)
                {
                    currentState = spiderState.PURSUE;
                }

                break;
        }
    }

    private void IdleAction()
    {
        if (!currentlyInBurst)
        {
            TrackSpider();
        }
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
        ShootMinigun();
    }

    private void TrackMinigun()
    {
        miniGun.transform.LookAt(player.transform.position + new Vector3(0f,1f,0f));
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

    private void ResetAttack()
    {
        alreadyAttacked = false;
        //if (currentState == gunnerState.RECHARGE) alreadyRecharged = true;
    }

    private void Death()
    {
        //agent.enabled = false;
        //eh.enabled = false;
        //enemyColl.enabled = false;
        //smr.enabled = false;
        Destroy(gameObject, 0f);
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
            // TODO: Modify variable values to get enhanced version.
            upgraded = true;
        }
    }
}
