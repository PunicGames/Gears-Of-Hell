using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class WorkerBotBehavior : MonoBehaviour
{
    //variables parametricas
    [SerializeField] int MAXGEARSCAPACITY = 10; //capacidad maxima de monedas que puede recoger
    [SerializeField] int attackDamage = 10; //daño por cada golpe
    [SerializeField] float rollSpeed = 10.0f; //velocidad a la que gira atacando
    [SerializeField] private MeleeWeaponBehaviour weaponCollider;
    [SerializeField] GameObject particleEffect;     
    [SerializeField] Collider recolectRangeCollider;

    //cosas para la explosion
    [SerializeField] private ParticleSystem explosionVfx;
    [SerializeField] private GameObject explosionColl;
    [SerializeField] private GameObject explosionRange;
    [SerializeField] private SkinnedMeshRenderer workerBotMesh;
    [SerializeField] private MeshRenderer weaponMesh;

    public float timeUntilExplosion;
    public int bombDamage;
    private bool alreadyExploding = false;

    [Space]
    public AudioClip tictac, boom;
    private AudioSource audioSource;

    //esenciales
    private Animator animator;
    private GameObject player;
    NavMeshAgent agent;

    int currentGears = 0; //monedas que lleva recogidas
    bool alreadyAttacked = false;

    private enum FSM2_states { 
        IDLE, 
        PURSUE,
        ATTACK,
    };

    private enum FSM1_states
    {
        RECOLECT,
        SEARCH,
    };

    private FSM1_states currentFSM1State = FSM1_states.SEARCH; //la fsm 1
    private FSM2_states currentFSM2State = FSM2_states.PURSUE; //la fsm 2

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = gameObject.GetComponent<AudioSource>();

        weaponCollider.player = player;
        weaponCollider.health = GetComponent<EnemyHealth>();
        weaponCollider.playerHealth = player.GetComponent<Health>();
        weaponCollider.attackDamage = attackDamage;
        weaponCollider.enabled = false;

        animator.SetBool("isMoving", true);

        GetComponent<EnemyHealth>().onDeath += Death; //me suscribo a la funcion de muerte
    }

    private void Update()
    {
        FSM_LVL_2();
        ActionFSM();
    }
    private void FSM_LVL_2() //{ IDLE, PURSUE, ATTACK }
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        agent.SetDestination(player.transform.position);

        switch (currentFSM2State)
        {
            case FSM2_states.IDLE:
                animator.SetBool("isMoving", false); //variable que este en el animator
                transform.LookAt(player.transform.position); //miramos al player

                if (distance > agent.stoppingDistance) 
                {
                    //si la distancia es mayor a la asignada a detenerse comenzamos a perseguir
                    currentFSM2State = FSM2_states.PURSUE;
                }
                break;

            case FSM2_states.PURSUE:
                animator.SetBool("isMoving", true); //variable del animator
                if (distance <= agent.stoppingDistance)
                {
                    currentFSM2State = FSM2_states.IDLE;
                }
                break;
        }
    }

    private void ActionFSM()
    {
        switch (currentFSM2State)
        {
            case FSM2_states.IDLE:
                //Do nothing
                break;

            case FSM2_states.ATTACK:
                //si no ha atacado se pone a atacar
                if (!alreadyAttacked) Attack();
                break;
        }
    }

    public void Attack()
    {
        alreadyAttacked = true;
        animator.SetBool("isAttacking", true);
        particleEffect.SetActive(true);
        particleEffect.GetComponent<ParticleSystem>().Play();

        Invoke(nameof(ResetParameters), 5);
        Invoke(nameof(StopParticleEffect), 3);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enabled)
        {
            if (other.gameObject == player)
            {
                ActivateWeaponCollider();
                currentFSM2State = FSM2_states.ATTACK;
            }
        }
    }
    public void StopParticleEffect()
    {
        particleEffect.GetComponent<ParticleSystem>().Play();
    }
    public void ResetParameters()
    {
        DeactivateWeaponCollider();
        alreadyAttacked = false;
        animator.SetBool("isAttacking", false);
        particleEffect.SetActive(false);
        print("resetParameters");
        currentFSM2State = FSM2_states.IDLE;
    }

    private void Death()
    {
        print("death function");
        if (!alreadyExploding)
        {
            alreadyExploding = true;
            TriggerExplosion();
        }
    }

    private void TriggerExplosion()
    {
        print("trigger function");
        agent.enabled = false;
        GetComponent<EnemyHealth>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;

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
        print("explode function");
        Collider[] hitColliders = Physics.OverlapSphere(explosionColl.transform.position, explosionColl.GetComponent<SphereCollider>().radius * explosionColl.transform.localScale.x * transform.localScale.x);
        foreach (var hc in hitColliders)
        {
            if (hc.tag == "Enemy")
            {
                hc.GetComponent<EnemyHealth>().TakeDamage(bombDamage);
            }
            else if (hc.tag == "Player")
            {
                hc.GetComponent<Health>().TakeDamage(bombDamage);
            }
        }
        explosionRange.SetActive(false);

        //Playing Booom
        audioSource.clip = boom;
        audioSource.loop = false;
        audioSource.Play();

        explosionVfx.Play();
        workerBotMesh.enabled = false;
        weaponMesh.enabled = false;
        Destroy(gameObject, explosionVfx.main.duration);
    }

    public void ActivateWeaponCollider()
    {
        weaponCollider.enabled = true;
    }

    public void DeactivateWeaponCollider()
    {
        weaponCollider.enabled = false;
    }

    
}
