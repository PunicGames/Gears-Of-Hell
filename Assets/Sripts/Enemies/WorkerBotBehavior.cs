using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class WorkerBotBehavior : MonoBehaviour
{
    //variables parametricas
    public int MAXGEARSCAPACITY = 4; //capacidad maxima de monedas que puede recoger
    public int MAXHEALSCAPACITY = 2; //capacidad maxima de curas que puede recoger
    public int MAXAMMOSCAPACITY = 2; //capacidad maxima de ammos que puede recoger

    [SerializeField] int attackDamage = 7; //daño por cada golpe
    [SerializeField] float rollSpeed = 0.8f; //velocidad a la que gira atacando

    [Space]

    [SerializeField] private MeleeWeaponBehaviour weaponCollider;
    [SerializeField] GameObject weaponColliderPivot;
    [SerializeField] GameObject particleEffect;
    [SerializeField] GameObject particleEffectPivot;
    [SerializeField] Collider recolectRangeCollider;

    [Space]

    //cosas para la explosion
    [SerializeField] private ParticleSystem explosionVfx;
    [SerializeField] private GameObject explosionColl;
    [SerializeField] private GameObject explosionRange;
    [SerializeField] private SkinnedMeshRenderer workerBotMesh;
    [SerializeField] private MeshRenderer weaponMesh;
    [SerializeField] private GameObject smokeEffect;
    [SerializeField] private EnemySoundManager enemySoundManager;

    [Space]

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

    [SerializeField] public int currentGears = 0; //monedas que lleva recogidas
    [SerializeField] public int currentAmmos = 0; //ammos que lleva recogidas
    [SerializeField] public int currentHeals = 0; //heals que lleva recogidas
    bool alreadyAttacked = false;
    public GameObject itemObject;

    [SerializeField] private LayerMask m_LayerMask;

    public enum FSM2_states
    {
        IDLE,
        PURSUE,
        ATTACK,
    };

    public enum FSM1_states
    {
        RECOLECT,
        SEARCH,
        ATTACKFSM,
    };

    [Space]

    [SerializeField] public FSM1_states currentFSM1State = FSM1_states.ATTACKFSM; //la fsm 1
    [SerializeField] public FSM2_states currentFSM2State = FSM2_states.PURSUE; //la fsm 2
    private bool dead = false;

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
        enemySoundManager = gameObject.GetComponent<EnemySoundManager>();
        weaponCollider.attackDamage = attackDamage;
        weaponCollider.enabled = false;

        animator.SetBool("isMoving", true);

        GetComponent<EnemyHealth>().onDeath += Death; //me suscribo a la funcion de muerte
    }

    private void Update()
    {
        if (!dead)
        {
            FSM_LVL_1();
            //ActionFSM();
        }
        else
        {
            animator.SetBool("isMoving", true); //variable que este en el animator
            animator.SetBool("isAttacking", false);
            smokeEffect.SetActive(false);
            ResetParameters();
            if (agent.enabled)
                agent.SetDestination(player.transform.position);
        }
    }

    private void FSM_LVL_1()
    {
        switch (currentFSM1State)
        {
            case FSM1_states.RECOLECT:
                enemySoundManager.PauseSound("walk");
                animator.SetBool("isMoving", false);
                transform.LookAt(itemObject.transform.position); //miramos al player

                if (itemObject.CompareTag("Gear"))
                {
                    print("recolectamos gear");

                    currentGears += 1;
                    GearUpgrade(); //mejoras al coger moneda
                    Destroy(itemObject);
                }
                if (itemObject.CompareTag("Heal"))
                {
                    print("recolectamos heal");

                    currentHeals += 1;
                    HealUpgrade(); //mejoras al coger moneda
                    Destroy(itemObject);
                }
                if (itemObject.CompareTag("Ammo"))
                {
                    print("recolectamos ammo");

                    currentAmmos += 1;
                    AmmoUpgrade(); //mejoras al coger moneda
                    Destroy(itemObject);
                }

                currentFSM1State = FSM1_states.ATTACKFSM;

                break;

            case FSM1_states.SEARCH:
                float distance = Vector3.Distance(itemObject.transform.position, transform.position); //distancia entre el item y el workerbot
                agent.SetDestination(itemObject.transform.position); //se dirige a por el item

                enemySoundManager.PlaySound("walk");
                animator.SetBool("isMoving", true);

                if (distance <= agent.stoppingDistance)
                {
                    //si la distancia es menor a la asignada a detenerse me detengo
                    currentFSM1State = FSM1_states.RECOLECT;
                }

                break;

            case FSM1_states.ATTACKFSM:
                FSM_LVL_2();
                break;
        }
    }

    private void FSM_LVL_2() //{ IDLE, PURSUE, ATTACK }
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        agent.SetDestination(player.transform.position);

        switch (currentFSM2State)
        {
            case FSM2_states.IDLE:
                enemySoundManager.PauseSound("walk");
                animator.SetBool("isMoving", false);
                transform.LookAt(player.transform.position); //miramos al player

                if (distance > agent.stoppingDistance)
                {
                    //si la distancia es mayor a la asignada a detenerse comenzamos a perseguir
                    currentFSM2State = FSM2_states.PURSUE;
                }
                break;

            case FSM2_states.PURSUE:
                enemySoundManager.PlaySound("walk");
                animator.SetBool("isMoving", true);
                if (distance <= agent.stoppingDistance)
                {
                    //si la distancia es menor a la asignada a detenerse me detengo
                    currentFSM2State = FSM2_states.IDLE;
                }
                break;
            case FSM2_states.ATTACK:
                //si no ha atacado se pone a atacar
                if (!alreadyAttacked) Attack();
                break;
        }
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

    private void GearUpgrade()
    {
        Vector3 gearScale = new Vector3(1 + 0.2f * currentGears, 1 + 0.2f * currentGears, 1 + 0.2f * currentGears);

        weaponColliderPivot.transform.localScale = gearScale; //aumenta tamaño del collider
        weaponMesh.transform.localScale = gearScale; //aumenta tamaño del mesh
        particleEffectPivot.transform.localScale = gearScale; //aumenta el tamaño del efecto de particulas

        attackDamage = 7 + 1 * currentGears; //aumenta el daño por cada moneda recogida
        weaponCollider.attackDamage = attackDamage; //le paso el daño nuevo al script del arma

        //activo el efecto de particulas de brillantitos, mas cantidad por cada moneda que tenga
        print(currentGears);
    }
    private void HealUpgrade()
    {
        Vector3 healScale = new Vector3(1 + 0.25f * currentHeals, 1 + 0.25f * currentHeals, 1 + 0.25f * currentHeals);

        transform.localScale = healScale; //aumenta el tamaño del mesh del pj

        GetComponent<EnemyHealth>().startingHealth *= 2; //duplico vida maxima actual
        GetComponent<EnemyHealth>().currentHealth = GetComponent<EnemyHealth>().startingHealth; //recupera toda la vida

        //activo el efecto de cura en el robot
        print(currentHeals);
    }
    private void AmmoUpgrade()
    {
        agent.speed += 0.6f; //aumenta la velocidad de movimiento
        rollSpeed = 0.8f + currentAmmos * 0.4f; //aumenta la velocidad de ataque
        animator.SetFloat("rollSpeed", rollSpeed);

        smokeEffect.transform.localScale *= 2f; //aumenta el tamaño del efecto de smoke
        smokeEffect.GetComponent<ParticleSystem>().playbackSpeed += 5; //aumenta la velocidad del efecto de smoke

        //activo efecto de particulas de algo para inficar que ahora tiene esta mejora
        print(currentAmmos);
    }

    public void Attack()
    {
        alreadyAttacked = true;
        enemySoundManager.PlaySound("attack");
        animator.SetBool("isAttacking", true);
        particleEffect.SetActive(true);
        particleEffect.GetComponent<ParticleSystem>().Play();

        Invoke(nameof(ResetParameters), 5);
        Invoke(nameof(StopParticleEffect), 3);
    }
    public void StopParticleEffect()
    {
        particleEffect.GetComponent<ParticleSystem>().Play();
    }
    public void ResetParameters()
    {
        DeactivateWeaponCollider();
        enemySoundManager.PauseSound("attack");
        alreadyAttacked = false;
        animator.SetBool("isAttacking", false);
        particleEffect.SetActive(false);

        currentFSM2State = FSM2_states.IDLE;
    }
    private void Death()
    {
        if (!alreadyExploding)
        {
            dead = true;
            alreadyExploding = true;
            TriggerExplosion();
        }
    }
    private void TriggerExplosion()
    {

        if (enemySoundManager != null)
            enemySoundManager.PauseSound("attack");
        explosionRange.SetActive(true);

        agent.speed *= 1.2f;
        //Playing 'tictac'
        audioSource.clip = tictac;
        audioSource.loop = true;
        audioSource.Play();

        Invoke("Explode", timeUntilExplosion);
        //enabled = false;
    }
    private void Explode()
    {
        weaponCollider.gameObject.SetActive(false);
        GetComponent<EnemyHealth>().DropItems();
        GetComponent<EnemyHealth>().enabled = false;

        Collider[] hitColliders = Physics.OverlapSphere(explosionColl.transform.position, explosionColl.GetComponent<SphereCollider>().radius * explosionColl.transform.localScale.x * transform.localScale.x, m_LayerMask, QueryTriggerInteraction.Ignore);
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

        agent.enabled = false;

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
