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

    [SerializeField] private MeleeWeaponBehaviour weaponCollider; //comportamiento del arma
    [SerializeField] GameObject weaponColliderPivot;
    [SerializeField] GameObject particleEffect; //efecto de particulas usado al atacar
    [SerializeField] GameObject particleEffectPivot;
    [SerializeField] Collider recolectRangeCollider; //collider que tiene el script para detectar los objetos

    [Space]

    //cosas para la explosion
    [SerializeField] private ParticleSystem explosionVfx; //efecto de particulas de explosion al morir
    [SerializeField] private GameObject explosionColl; //gameObject de la explosion
    [SerializeField] private GameObject explosionRange; //rango de la explosion
    [SerializeField] private SkinnedMeshRenderer workerBotMesh; //modelo del workerbot
    [SerializeField] private MeshRenderer weaponMesh; //modelo de la bolsa
    [SerializeField] private GameObject smokeEffect; //efecto de particulas de vapor
    [SerializeField] private EnemySoundManager enemySoundManager; //sonidos

    [Space]

    public float timeUntilExplosion; //tiempo que tarda entre morir y explotar
    public int bombDamage; //daño que hace al todos los enemigos al explotar
    private bool alreadyExploding = false; //indica si esta explotando en ese momento

    [Space]

    public AudioClip tictac, boom; //sonidos de la explosion
    private AudioSource audioSource;

    //esenciales
    private Animator animator; //gestor de animaciones
    private GameObject player; //personaje que controlamos (el juagdor)
    NavMeshAgent agent; //navmesh que utiliza para saber por donde puede caminar

    [SerializeField] public int currentGears = 0; //monedas que lleva recogidas
    [SerializeField] public int currentAmmos = 0; //cajas de municion que lleva recogidas
    [SerializeField] public int currentHeals = 0; //curas que lleva recogidas
    bool alreadyAttacked = false; //indica si el workerbot esta atacando
    private bool dead = false; //indica si el workerbot ha muerto
    public GameObject itemObject; //variable en la que almacena el objeto que ha detectado para comenzar a dirigirse hacia el

    [SerializeField] private LayerMask m_LayerMask;

    public enum FSM2_states
    {
        IDLE, //el workerbot se detiene
        PURSUE, //el workerbot persigue al jugador
        ATTACK, //el workerbot ejecuta la funcion de atacar
    };

    public enum FSM1_states
    {
        RECOLECT, //el workerbot recolecta el objeto almacenado en intemObject y aplica la mejora correspondiente
        SEARCH, //el workerbot comienda a dirigirse a la posicion del objeto ignorando al jugador
        ATTACKFSM, //se activa la FSM_LVL_2
    };

    [Space]

    [SerializeField] public FSM1_states currentFSM1State = FSM1_states.ATTACKFSM; //estado actual de la FSM principal
    [SerializeField] public FSM2_states currentFSM2State = FSM2_states.PURSUE; //estado actual la FSM cuando no detecta objetos que recoger

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
            FSM_LVL_1(); //si no esta muerto ejecutara constantemente la FSM 1
        }
        else
        {
            animator.SetBool("isMoving", true);
            animator.SetBool("isAttacking", false);
            smokeEffect.SetActive(false);
            ResetParameters();
            if (agent.enabled)
                agent.SetDestination(player.transform.position);
        }
    }

    private void FSM_LVL_1() //{ RECOLECT, SEARCH, ATTACKFSM }
{
        switch (currentFSM1State)
        {
            case FSM1_states.RECOLECT:
                
                //dependiendo del tipo de objeto que haya almacenado en itemObject el workerbot ejecutara una u otra mejora

                enemySoundManager.PauseSound("walk");
                animator.SetBool("isMoving", false);
                transform.LookAt(itemObject.transform.position); //miramos al objeto

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

                //el workerbot en este estado se dirige hacia el objeto que este almacenado en itemObject

                if (itemObject == null)
                {
                    //esto evita que el workerbot se quede constantemente buscando el itemObject que tenia almacenado per que ha
                    //desaparecido porque lo ha recolectado en jugador. Debido a esto pasamos de nuevo al estado de atacar

                    currentFSM1State = FSM1_states.ATTACKFSM;
                }

                float distance = Vector3.Distance(itemObject.transform.position, transform.position); //distancia entre el item y el workerbot
                agent.SetDestination(itemObject.transform.position); //se dirige a por el item

                enemySoundManager.PlaySound("walk");
                animator.SetBool("isMoving", true);

                if (distance <= agent.stoppingDistance)
                {
                    //si la distancia es menor a la asignada a detenerse se detiene y recolecta

                    currentFSM1State = FSM1_states.RECOLECT;
                }

                break;

            case FSM1_states.ATTACKFSM:
                
                //este estado se ejcuta cuando no tiene ningun objeto disponible para recoger por lo
                //que ejecutara el comportamiento correspondiente a la interacion con el jugador

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

                //el workerbot se detiene 

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

                //el workerbot persigue al jugador

                enemySoundManager.PlaySound("walk");
                animator.SetBool("isMoving", true);
                if (distance <= agent.stoppingDistance)
                {
                    //si la distancia es menor a la asignada a detenerse me detengo
                    currentFSM2State = FSM2_states.IDLE;
                }
                break;

            case FSM2_states.ATTACK:

                //este estado se activa cuando el jugador entra dentro del collider preparado para ello de manera que
                //el workerbo puede serguir persiguiendo y atacando ejecutando la animacion combinada correctamente
                
                //si no ha atacado se pone a atacar

                if (!alreadyAttacked) Attack();
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //si detecta al jugador activa el collider del arma y comienza a atacar
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
        //mejora aplicada al coger una moneda

        Vector3 gearScale = new Vector3(1 + 0.2f * currentGears, 1 + 0.2f * currentGears, 1 + 0.2f * currentGears);

        weaponColliderPivot.transform.localScale = gearScale; //aumenta tamaño del collider
        weaponMesh.transform.localScale = gearScale; //aumenta tamaño del mesh
        particleEffectPivot.transform.localScale = gearScale; //aumenta el tamaño del efecto de particulas

        attackDamage = 7 + 1 * currentGears; //aumenta el daño por cada moneda recogida
        weaponCollider.attackDamage = attackDamage; //le paso el daño nuevo al script del arma
    }

    private void HealUpgrade()
    {
        //mejora aplicada al coger una cura

        Vector3 healScale = new Vector3(1 + 0.25f * currentHeals, 1 + 0.25f * currentHeals, 1 + 0.25f * currentHeals);

        transform.localScale = healScale; //aumenta el tamaño del mesh del pj

        GetComponent<EnemyHealth>().startingHealth *= 2; //duplico vida maxima actual
        GetComponent<EnemyHealth>().Heal(GetComponent<EnemyHealth>().startingHealth); //recupera toda la vida

        //activo el efecto de cura en el robot
        print(currentHeals);
    }
    private void AmmoUpgrade()
    {
        //mejora aplicada al coger una caja de municion

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
        print("ataca");

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
        print("ha muerto");
        if (!alreadyExploding)
        {
            dead = true;
            alreadyExploding = true;
            TriggerExplosion();
        }
    }
    private void TriggerExplosion()
    {
        print("activamos rango de explosion");

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
        print("workerbot explota");

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
        print("Activamos collider del arma");

        weaponCollider.enabled = true;
    }
    public void DeactivateWeaponCollider()
    {
        print("Desactivamos collider del arma");

        weaponCollider.enabled = false;
    }
}
