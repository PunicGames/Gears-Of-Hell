using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GunnerBehaviour : MonoBehaviour
{
    public float cadenceTime = 1f;
    public float reloadTime = 2f;
    public float bulletSpeed = 2;
    public float damage = 10;
    public int bulletsPerMag = 5;
    public bool SemiAuto = false;
    public int bulletsPerBurst = 5;
    public float SemiAutoTime = 2f;
    private int bulletsInMag;
    private int bulletsInBurst;


    private bool alreadyAttacked = false;
    private bool inAttackRange = false;

    public GameObject bullet;

    Transform player;
    NavMeshAgent agent;
    Animator animator;
    AudioSource gunAudio;

    private void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        gunAudio = GetComponent<AudioSource>();
        bulletsInMag = bulletsPerMag;
        bulletsInBurst = bulletsPerBurst;
    }

    private void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        transform.LookAt(player.position);

        if (distance <= agent.stoppingDistance)
        {
            if (!alreadyAttacked)
            {
                Attack();

            }
            animator.SetBool("Moving", false);
        }
        else
        {
            inAttackRange = false;
            Chase();
            animator.SetBool("Moving", true);
        }
    }
    void Attack()
    {

        gunAudio.Play();
        GameObject b = Instantiate(bullet, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
        b.transform.LookAt(player.transform);
        Bullet bulletParams = b.GetComponent<Bullet>();
        bulletParams.SetForce(bulletSpeed);
        bulletParams.SetDamage(damage);
        bulletParams.SetLaser(false);
        bulletParams.owner = Bullet.BulletOwner.ENEMY;
        bulletsInMag--;
        alreadyAttacked = true;
        if (SemiAuto)
        {
            bulletsInBurst--;
            if (bulletsInMag > 0)
                if (bulletsInBurst > 0)
                    Invoke(nameof(ResetAttack), cadenceTime);
                else
                {
                    Invoke(nameof(ResetAttack), SemiAutoTime);
                    bulletsInBurst = bulletsPerBurst;
                }
            else
            {
                animator.SetTrigger("reload");
                Invoke(nameof(ResetAttack), reloadTime);
                bulletsInMag = bulletsPerMag;
            }
        }
        else
        {

            if (bulletsInMag > 0)
                Invoke(nameof(ResetAttack), cadenceTime);
            else
            {
                animator.SetTrigger("reload");
                Invoke(nameof(ResetAttack), reloadTime);
                bulletsInMag = bulletsPerMag;
            }
        }






    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void Chase()
    {
        agent.SetDestination(player.position);
    }

    //IEnumerator Attack()
    //{
    //    while (inAttackRange)
    //    {
    //        int count = bulletsPerMag;
    //        while (count > 0)
    //        {
    //            gunAudio.Play();
    //            GameObject b = Instantiate(bullet, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
    //            b.transform.LookAt(player.transform);
    //            Bullet bulletParams = b.GetComponent<Bullet>();
    //            bulletParams.SetForce(bulletSpeed);
    //            bulletParams.SetDamage(damage);
    //            bulletParams.SetLaser(false);
    //            bulletParams.owner = Bullet.BulletOwner.ENEMY;
    //            count--;
    //            yield return new WaitForSeconds(cadenceTime);
    //        }

    //        yield return new WaitForSeconds(reloadTime);
    //    }
    //    alreadyAttack = false;
    //    yield return 0;
    //}

}
