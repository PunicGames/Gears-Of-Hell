using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GunslingerBehaviour : MonoBehaviour
{
    // Params
    public float sightRange;
    public float attackRange;
    public float attackSpeed;
    public float bulletSpeed;
    public float damage = 10;

    public GameObject bullet;
    private bool alive = true;
    Transform player;
    NavMeshAgent agent;
    Animator animator;
    EnemySoundManager enemySoundManager;
    public Transform shootOrigin;
    [SerializeField] ParticleSystem muzzleVFX;
    EnemyHealth enemyHealth;

    // Bullet colors
    [SerializeField] private Color albedo;
    [SerializeField] private Color emissive;

    // Upgrade
    bool upgraded = false;
    [SerializeField] private ParticleSystem RaiseUnit;

    #region MonoBehabiour
    private void Start()
    {
        currentState = GunslingerState.IDLE;
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemySoundManager = gameObject.GetComponent<EnemySoundManager>();
        enemyHealth = gameObject.GetComponent<EnemyHealth>();
        enemyHealth.onDeath += Death;
    }

    private void Update()
    {
        FSM();
    }
    #endregion

    private enum GunslingerState { IDLE, PURSUE, SHOOT, DEAD };
    private GunslingerState currentState = GunslingerState.IDLE;

    #region Perceptions

    // Ha atacado
    private bool alreadyAttacked = false;

    // Está a rango
    private bool InRange => Vector3.Distance(player.position, transform.position) <= attackRange;

    // Puede atacar
    private bool IsAGoodShoot()
    {
        Ray ray = new Ray(transform.position, (player.position - transform.position).normalized);
        if (Physics.Raycast(ray, out RaycastHit hit))
            return hit.collider.gameObject.tag == "Player";
        return false;
    }

    #endregion

    #region Actions
    private void Shoot()
    {
        agent.SetDestination(transform.position);
        if (!alreadyAttacked)
        {
            enemySoundManager.PlaySound("shoot");
            GameObject b = Instantiate(bullet, new Vector3(shootOrigin.position.x, shootOrigin.position.y, shootOrigin.position.z), Quaternion.identity);
                b.transform.LookAt(player.transform);
            Bullet bulletParams = b.GetComponent<Bullet>();
                bulletParams.SetForce(bulletSpeed);
                bulletParams.SetDamage(damage);
                bulletParams.SetLaser(false);
                bulletParams.owner = Bullet.BulletOwner.ENEMY;
                bulletParams.SetBulletColors(albedo, emissive);
            alreadyAttacked = true;
            muzzleVFX.Play();
            Invoke(nameof(ResetAttack), attackSpeed);
        }
    }

    // Iddle

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void UpgradeAttackSpeed()
    {
        if (!upgraded)
        {
            // TODO: Modify variable values to get enhanced version.
            upgraded = true;
            RaiseUnit.Play();
            RaiseUnit.loop = true;
        }
    }
    #endregion

    #region transitions

    private void TransitionToShoot()
    {
        currentState = GunslingerState.SHOOT;
        animator.SetBool("Moving", false);
        enemySoundManager.PauseSound("walk");
    }

    private void TransitionToPursue()
    {
        currentState = GunslingerState.PURSUE;
        animator.SetBool("Moving", true);
        agent.isStopped = false;
        enemySoundManager.PlaySound("walk");
    }
    private void TransitionToIdle()
    {
        currentState = GunslingerState.IDLE;
        animator.SetBool("Moving", false);
        agent.isStopped = true;
        enemySoundManager.PauseSound("walk");
    }

    #endregion

    #region FSM
    private void FSM()
    {
        switch (currentState)
        {
            case GunslingerState.IDLE:
                // Action
                transform.LookAt(player.position);

                if (InRange && IsAGoodShoot() && !alreadyAttacked)
                    TransitionToShoot();
                else if (!InRange || !IsAGoodShoot())
                    TransitionToPursue();
                break;

            case GunslingerState.SHOOT:
                //Action
                Shoot();

                if (!InRange || !IsAGoodShoot())
                    TransitionToPursue();
                else if (InRange && IsAGoodShoot() && alreadyAttacked)
                    TransitionToIdle();
                break;

            case GunslingerState.PURSUE:
                // Action
                agent.SetDestination(player.position);

                if (InRange && IsAGoodShoot())
                    TransitionToShoot();
                else if (InRange && IsAGoodShoot() && !alreadyAttacked)
                    TransitionToShoot();
                break;

            case GunslingerState.DEAD:
                break;

        }
    }
    #endregion

    public void Death()
    { 
        currentState = GunslingerState.DEAD;
    }
}
