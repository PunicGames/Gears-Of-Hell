using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public Vector3 target;
    public Vector3 origin;

    public int damage;

    public float flySpeed;

    private float rotSpeed;

    private float anim;

    private bool alreadyExploding = false;

    [SerializeField] ParticleSystem vfx;
    [SerializeField] GameObject rangeObject;
    [SerializeField] GameObject missileMesh;

    public AudioClip boom;
    private AudioSource audioSource;

    [SerializeField] private LayerMask layerMask;

    void Start()
    {
        origin = transform.position;

        rotSpeed = flySpeed * 3.8f;

        target = GameObject.FindGameObjectWithTag("Player").transform.position;
        audioSource = gameObject.GetComponent<AudioSource>();

        rangeObject.transform.parent = null;
        rangeObject.transform.position = target;

        gameObject.transform.LookAt(target + new Vector3(0,30,0));
        
    }

    private void Update()
    {
        if (alreadyExploding)
        {
            return;
        }
        //rangeObject.transform.position = target;
        anim += Time.deltaTime;
        transform.position = MathParabola.Parabola(origin, target, 3.8f, anim * flySpeed);
        MissileFly();

        if (transform.position.y <= target.y)
        {
            alreadyExploding = true;
            Explode();
        }
    }

    private void MissileFly()
    {
        var dir = target - transform.position;
        Quaternion lookOnLook = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, rotSpeed * Time.deltaTime);
    }

    private void Explode()
    {
        transform.rotation = Quaternion.identity;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius * transform.localScale.x, layerMask, QueryTriggerInteraction.Ignore); ;
        foreach (var hc in hitColliders)
        {

            if (hc.tag == "Enemy")
            {
                hc.GetComponent<EnemyHealth>().TakeDamage(damage);
            }
            else if (hc.tag == "Player")
            {
                hc.GetComponent<Health>().TakeDamage(damage);
            }

        }
        rangeObject.SetActive(false);
        Destroy(rangeObject);

        missileMesh.SetActive(false);

        //Playing Booom
        audioSource.Stop();
        audioSource.clip = boom;
        audioSource.Play();

        vfx.Play();

        Destroy(gameObject, vfx.main.duration);
        
    }
}
