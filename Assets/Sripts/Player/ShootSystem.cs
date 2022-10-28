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
    [HideInInspector]
    public bool[] availableGuns;

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
    [HideInInspector]
    public bool laserShot;
    [HideInInspector]
    public bool bigShot;
    [HideInInspector]
    public float scaleFactor = 1f;

    private void Awake()
    {
        // Guns initialization
        guns = new PlayerGuns();
        availableGuns = new bool[guns.getGuns().Length];
        // La pistola, que ocupa la primera posición, siempre podrá ser accesible.
        availableGuns[0] = true;
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
        if (readyToShoot && shooting && !reloading && guns.getGuns()[selectedGun].bulletsLeftInMagazine <= 0) Reload();

        if (ammunitionDisplay != null) {
            ammunitionDisplay.text = (guns.getGuns()[selectedGun].bulletsLeftInMagazine + " / " + guns.getGuns()[selectedGun].totalBullets);
        }
    }

    public void Shooting(Animator anim) 
    {
        // Comprueba si se puede disparar
        if (readyToShoot && shooting && !reloading && (guns.getGuns()[selectedGun].bulletsLeftInMagazine) > 0) {
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


        guns.getGuns()[selectedGun].bulletsLeftInMagazine--;
        guns.getGuns()[selectedGun].bulletsShot++;

        if (allowInvoke) {
            Invoke("ResetShot", guns.getGuns()[selectedGun].timeBetweenShooting); // Llama a la función después de timeBetweenShooting segundos
            allowInvoke = false;
        }

        // Repetimos la funcion de disparo dependiendo de bulletsPerTap
        if (guns.getGuns()[selectedGun].bulletsShot < guns.getGuns()[selectedGun].bulletsPerTap && (guns.getGuns()[selectedGun].bulletsLeftInMagazine > 0)) {
            Invoke("Shoot", guns.getGuns()[selectedGun].timeBetweenShots);
        }
        if ((guns.getGuns()[selectedGun].automaticGun && shooting && (guns.getGuns()[selectedGun].bulletsLeftInMagazine > 0)))
        { // Si es un arma automática, sigue disparando
            Invoke("Shoot", guns.getGuns()[selectedGun].timeBetweenShots);
        }
    }

    public void ResetShot() {
        readyToShoot = true;
        allowInvoke = true;
    }

    public void Reload() { // Llamar función cuando jugador pulsa R
        Debug.Log("Intenta recargar");
        if ((guns.getGuns()[selectedGun].bulletsLeftInMagazine < guns.getGuns()[selectedGun].magazineSize) && !reloading && guns.getGuns()[selectedGun].totalBullets > 0) {
            Debug.Log("Pasa");
            reloading = true;
            Invoke("ReloadFinished", guns.getGuns()[selectedGun].reloadTime);
        }
    }

    private void ReloadFinished() {

        int bulletsInMagazine = guns.getGuns()[selectedGun].bulletsLeftInMagazine;
        if (bulletsInMagazine + guns.getGuns()[selectedGun].totalBullets >= guns.getGuns()[selectedGun].magazineSize)
        {
            guns.getGuns()[selectedGun].bulletsLeftInMagazine = guns.getGuns()[selectedGun].magazineSize;
            guns.getGuns()[selectedGun].totalBullets -= (guns.getGuns()[selectedGun].magazineSize - bulletsInMagazine);
        }
        else {
            guns.getGuns()[selectedGun].bulletsLeftInMagazine = guns.getGuns()[selectedGun].totalBullets + bulletsInMagazine;
            guns.getGuns()[selectedGun].totalBullets = 0;
        }

        reloading = false;
        //Shooting(); // Llamamos a esta funcion en caso de que el jugador siga con el click de ratón pulsado, empiece a disparar
    }

    public void SwapGun() {

        // Explora las opciones de armas en orden. Hay dos bucles ya que podriamos estar posicionados en el arma número 1,
        // por lo que explora primero hacia arriba y luego da la vuelta y explora las que jerárquicamente están por debajo

        for (int i = selectedGun + 1; i < availableGuns.Length; i++) {
            if (availableGuns[i]) { 
                selectedGun = i;
                // Se resetea el disparo para quitar el enfriamiento del anterior arma y poder disparar de inmediato (aunque debería llamarse mejor al finalizar la animación)
                ResetShot();
                // AQUI VA LA ANIMACIÓN DE CAMBIO DE ARMA (TENER EN CUENTA LO QUE PONE 2 LINEAS MÁS ARRIBA)
                return;
            }
        }

        for (int i = 0; i < selectedGun; i++) {
            if (availableGuns[i])
            {
                selectedGun = i;
                ResetShot();
                // AQUI VA LA ANIMACIÓN DE CAMBIO DE ARMA
                return;
            }
        }
    }

    public void ActivateGun(int idx) {
        availableGuns[idx] = true;
    }

    public void DeactivateGun(int idx) {
        availableGuns[idx] = false;
    }
}
