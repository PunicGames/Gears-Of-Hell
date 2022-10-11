using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootSystem : MonoBehaviour
{

    // Bullet
    public GameObject bullet;

    // Bullet Force
    public float shootForce;
    public int bulletDamage = 20;

    // Gun Stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    int bulletsLeft, bulletsShot;

    // Action control
    public bool shooting;
    bool readyToShoot, reloading;
    public bool automaticGun;

    // Bug fixing
    public bool allowInvoke = true;

    // Audio
    AudioSource gunAudio;

    // Display
    private Text ammunitionDisplay;

    private void Awake()
    {
        // Inicializacion del variables
        bulletsLeft = magazineSize;
        readyToShoot = true;
        gunAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ammunitionDisplay = GameObject.Find("Municion").GetComponent<Text>();
    }

    void Update()
    {
        // Recarga automáticamente si no quedan balas
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        if (ammunitionDisplay != null)
            ammunitionDisplay.text = (bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
    }

    public void Shooting(Animator anim) 
    {
        // Comprueba si se puede disparar
        if (readyToShoot && shooting && !reloading && (bulletsLeft) > 0) {
            bulletsShot = 0;
            anim.SetTrigger("shoot");
            Shoot();
        }
    }

    private void Shoot() 
    {
        readyToShoot = false;
        gunAudio.Play();

        // Se calcula la dirección y origen del disparo
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        // Cálculo de spread
        float x = Random.Range(-spread, spread);

        // Cálculo de la nueva dirección con spread
        Vector3 directionWithSpread = direction + new Vector3(x, 0, 0);

        // Instanciación de la bala
        GameObject currentBullet = Instantiate(bullet, origin, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.gameObject.GetComponent<Bullet>().damage = bulletDamage;

        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke) {
            Invoke("ResetShot", timeBetweenShooting); // Llama a la función después de timeBetweenShooting segundos
            allowInvoke = false;
        }

        // Repetimos la funcion de disparo dependiendo de bulletsPerTap
        if (bulletsShot < bulletsPerTap && (bulletsLeft > 0)) {
            Invoke("Shoot", timeBetweenShots);
        }
        if ((automaticGun && shooting && (bulletsLeft > 0)))
        { // Si es un arma automática, sigue disparando
            Debug.Log("Magazine Size: " + magazineSize + ". BulletsLeft: " + bulletsLeft + ". BulletsShot: " + bulletsShot);
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot() {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload() { // Llamar función cuando jugador pulsa R
        if ((bulletsLeft < magazineSize) && !reloading) { 
            reloading = true;
            Invoke("ReloadFinished", reloadTime);
        }
    }

    private void ReloadFinished() {
        bulletsLeft = magazineSize;
        reloading = false;
        //Shooting(); // Llamamos a esta funcion en caso de que el jugador siga con el click de ratón pulsado, empiece a disparar
    }
}
