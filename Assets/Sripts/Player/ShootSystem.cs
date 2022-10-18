using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootSystem : MonoBehaviour
{
    // Guns
    public PlayerGuns guns;
    [Range(0, 1)]
    public int selectedGun = 0;

    // Bullet
    public GameObject bullet;
    public GameObject laserBullet;

    // Action control
    public bool shooting;
    bool readyToShoot, reloading;

    // Shoot Controller
    public bool allowInvoke = true;

    // Audio
    AudioSource gunAudio;

    // Display
    private Text ammunitionDisplay;

    //Perks Modifies
    public bool laserShot;
    public bool bigShot;

    public float scaleFactor = 1f;

    private void Awake()
    {
        // Guns initialization
        guns = new PlayerGuns();
    }

    private void Start()
    {
        // Inicializacion de variables
        readyToShoot = true;
        laserShot = false;

        // Inicializacion de componentes
        gunAudio = GetComponent<AudioSource>();

        // Display initialization
        ammunitionDisplay = GameObject.Find("Municion").GetComponent<Text>();
    }

    void Update()
    {
        // Recarga automáticamente si no quedan balas
        if (readyToShoot && shooting && !reloading && guns.getGuns()[selectedGun].bulletsLeft <= 0) Reload();

        if (ammunitionDisplay != null)
            ammunitionDisplay.text = (guns.getGuns()[selectedGun].bulletsLeft / guns.getGuns()[selectedGun].bulletsPerTap + " / " + guns.getGuns()[selectedGun].magazineSize / guns.getGuns()[selectedGun].bulletsPerTap);
    }

    public void Shooting(Animator anim) 
    {
        // Comprueba si se puede disparar
        if (readyToShoot && shooting && !reloading && (guns.getGuns()[selectedGun].bulletsLeft) > 0) {
            guns.getGuns()[selectedGun].bulletsShot = 0;
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
        float x = Random.Range(-guns.getGuns()[selectedGun].spread, guns.getGuns()[selectedGun].spread);

        // Cálculo de la nueva dirección con spread
        Vector3 directionWithSpread = direction + new Vector3(x, 0, 0);

        // Instanciación de la bala en funcion de las perks
        if (laserShot)
        {
            GameObject currentBullet = Instantiate(laserBullet, origin, Quaternion.identity);
            currentBullet.transform.forward = directionWithSpread.normalized;
            currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * guns.getGuns()[selectedGun].shootForce, ForceMode.Impulse);
            currentBullet.gameObject.GetComponent<BulletPlayer>().damage = guns.getGuns()[selectedGun].bulletDamage;
            currentBullet.gameObject.GetComponent<BulletPlayer>().laserShot = true;
            currentBullet.transform.localScale *= scaleFactor;
        }
        else
        {
            GameObject currentBullet = Instantiate(bullet, origin, Quaternion.identity);
            currentBullet.transform.forward = directionWithSpread.normalized;
            currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * guns.getGuns()[selectedGun].shootForce, ForceMode.Impulse);
            currentBullet.gameObject.GetComponent<BulletPlayer>().damage = guns.getGuns()[selectedGun].bulletDamage;
            currentBullet.gameObject.GetComponent<BulletPlayer>().laserShot = false;
            currentBullet.transform.localScale *= scaleFactor;
        }


        guns.getGuns()[selectedGun].bulletsLeft--;
        guns.getGuns()[selectedGun].bulletsShot++;

        if (allowInvoke) {
            Invoke("ResetShot", guns.getGuns()[selectedGun].timeBetweenShooting); // Llama a la función después de timeBetweenShooting segundos
            allowInvoke = false;
        }

        // Repetimos la funcion de disparo dependiendo de bulletsPerTap
        if (guns.getGuns()[selectedGun].bulletsShot < guns.getGuns()[selectedGun].bulletsPerTap && (guns.getGuns()[selectedGun].bulletsLeft > 0)) {
            Invoke("Shoot", guns.getGuns()[selectedGun].timeBetweenShots);
        }
        if ((guns.getGuns()[selectedGun].automaticGun && shooting && (guns.getGuns()[selectedGun].bulletsLeft > 0)))
        { // Si es un arma automática, sigue disparando
            Invoke("Shoot", guns.getGuns()[selectedGun].timeBetweenShots);
        }
    }

    private void ResetShot() {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload() { // Llamar función cuando jugador pulsa R
        if ((guns.getGuns()[selectedGun].bulletsLeft < guns.getGuns()[selectedGun].magazineSize) && !reloading) { 
            reloading = true;
            Invoke("ReloadFinished", guns.getGuns()[selectedGun].reloadTime);
        }
    }

    private void ReloadFinished() {
        guns.getGuns()[selectedGun].bulletsLeft = guns.getGuns()[selectedGun].magazineSize;
        reloading = false;
        //Shooting(); // Llamamos a esta funcion en caso de que el jugador siga con el click de ratón pulsado, empiece a disparar
    }
}
