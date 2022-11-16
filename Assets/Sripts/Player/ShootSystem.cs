using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShootSystem : MonoBehaviour
{
    public List<Mesh> weapon_meshes;
    [SerializeField] MeshFilter meshFilter;
    // Guns
    [HideInInspector] public PlayerGuns guns;
    [SerializeField] private AudioClip[] reloadAudioClip;
    [HideInInspector]public int selectedGun = 0;
    public bool[] availableGuns;

    // Guns in Order: pistol, subfusil, rifle, sniper, shotgun


    // Bullet
    public GameObject bullet;
    public GameObject laserBullet;

    // Action control
    public bool shooting;
    bool readyToShoot, reloading;

    // Shoot Controller
    public bool allowInvoke = true;

    // Audio
    AudioManager audioManager;

    // Display
    private TextMeshProUGUI ammunitionDisplay;
    private GameObject rechargingDisplay;

    //Perks Modifies
    [HideInInspector]
    public bool laserShot;
    [HideInInspector]
    public bool bigShot;
    [HideInInspector]
    public float scaleFactor = 1f;

    // Animator reference
    private Animator anim;

    // Cursor sprites
    [SerializeField] private Texture2D[] cursorSprites;
    private Vector2 cursorHotSpot;

    private void Awake()
    {
        // Guns initialization
        guns = new PlayerGuns();
        availableGuns = new bool[guns.getGuns().Length];
        // La pistola, que ocupa la primera posici�n, siempre podr� ser accesible.
        availableGuns[0] = true;
    }

    private void Start()
    {
        
        // Inicializacion de variables
        readyToShoot = true;
        laserShot = false;

        // Inicializacion de componentes
        audioManager = transform.GetComponent<AudioManager>();
        anim = GetComponentInParent<Animator>();

        // Display initialization
        ammunitionDisplay = GameObject.Find("Municion").GetComponent<TextMeshProUGUI>();
        rechargingDisplay = GameObject.Find("Recargando");
        rechargingDisplay.SetActive(false);

        // Display cursor
        cursorHotSpot = new Vector2(cursorSprites[selectedGun].width / 2, cursorSprites[selectedGun].height / 2);
        Cursor.SetCursor(cursorSprites[selectedGun], cursorHotSpot, CursorMode.Auto);

        // Init guns in mobile
        if (Application.isMobilePlatform)
        {
            InitGunsMobile();
        }
    }

    void Update()
    {
        // Recarga autom�ticamente si no quedan balas
        if (readyToShoot && shooting && !reloading && guns.getGuns()[selectedGun].bulletsLeftInMagazine <= 0) Reload();

        if (ammunitionDisplay != null)
        {
            if (selectedGun != 0)
                ammunitionDisplay.text = (guns.getGuns()[selectedGun].bulletsLeftInMagazine + "/" + guns.getGuns()[selectedGun].totalBullets);
            else // En caso de ser la pistola
                ammunitionDisplay.text = guns.getGuns()[selectedGun].bulletsLeftInMagazine + "/9999";
        }
    }

    public void Shooting()
    {
        // Comprueba si se puede disparar
        if (readyToShoot && shooting && !reloading && (guns.getGuns()[selectedGun].bulletsLeftInMagazine) > 0)
        {
            guns.getGuns()[selectedGun].bulletsShot = 0;
            anim.SetTrigger("shoot");
            Shoot();
        }
    }

    private void Shoot()
    {
        if (shooting) { // Shooting ayuda para controlar las balas de las armas autom�ticas de la funci�n Invoke del final de este m�todo. Evita que se disparen balas indeseadas
            readyToShoot = false;
            audioManager.Play(selectedGun);

            // Se calcula la direcci�n y origen del disparo
            Vector3 origin = transform.position;
            Vector3 direction = transform.forward;


            int numBulletsAtTime = 1; // Cualquier otro arma
            if (selectedGun == 4) // Escopeta
                numBulletsAtTime = 3;

            for (int i = 0; i < numBulletsAtTime; i++) { 
                // C�lculo de spread
                float x = Random.Range(-guns.getGuns()[selectedGun].spread, guns.getGuns()[selectedGun].spread);

                // C�lculo de la nueva direcci�n con spread
                Vector3 directionWithSpread = direction + new Vector3(x, 0, 0);

                // Instanciaci�n de la bala en funcion de las perks
                if (laserShot)
                {
                    GameObject currentBullet = Instantiate(laserBullet, origin, Quaternion.identity);
                    currentBullet.transform.forward = directionWithSpread.normalized;
                    Bullet bulletParams = currentBullet.GetComponent<Bullet>();
                    bulletParams.SetForce(directionWithSpread.normalized, guns.getGuns()[selectedGun].shootForce);
                    bulletParams.SetDamage(guns.getGuns()[selectedGun].bulletDamage);
                    bulletParams.SetLaser(true);
                    bulletParams.owner = Bullet.BulletOwner.PLAYER;
                    currentBullet.transform.localScale *= scaleFactor;
                }
                else
                {
                    GameObject currentBullet = Instantiate(bullet, origin, Quaternion.identity);
                    currentBullet.transform.forward = directionWithSpread.normalized;
                    Bullet bulletParams = currentBullet.GetComponent<Bullet>();
                    bulletParams.SetForce(directionWithSpread.normalized, guns.getGuns()[selectedGun].shootForce);
                    bulletParams.SetDamage(guns.getGuns()[selectedGun].bulletDamage);
                    bulletParams.SetLaser(false);
                    bulletParams.owner = Bullet.BulletOwner.PLAYER;
                    currentBullet.transform.localScale *= scaleFactor;
                }

                guns.getGuns()[selectedGun].bulletsLeftInMagazine--;
                guns.getGuns()[selectedGun].bulletsShot++;
            }


            if (allowInvoke)
            {
                Invoke("ResetShot", guns.getGuns()[selectedGun].timeBetweenShooting); // Llama a la funci�n despu�s de timeBetweenShooting segundos
                allowInvoke = false;
            }

            // Repetimos la funcion de disparo dependiendo de bulletsPerTap
            if (guns.getGuns()[selectedGun].bulletsShot < guns.getGuns()[selectedGun].bulletsPerTap && (guns.getGuns()[selectedGun].bulletsLeftInMagazine > 0))
            {
                Invoke("Shoot", guns.getGuns()[selectedGun].timeBetweenShots);
            }
            if ((guns.getGuns()[selectedGun].automaticGun && shooting && (guns.getGuns()[selectedGun].bulletsLeftInMagazine > 0)))
            { // Si es un arma autom�tica, sigue disparando
                Invoke("Shoot", guns.getGuns()[selectedGun].timeBetweenShots);
            }
        }
    }

    public void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    public void Reload()
    { // Llamar funci�n cuando jugador pulsa R
        Debug.Log("Intenta recargar");
        if ((guns.getGuns()[selectedGun].bulletsLeftInMagazine < guns.getGuns()[selectedGun].magazineSize) && !reloading && guns.getGuns()[selectedGun].totalBullets > 0)
        {
            Debug.Log("Pasa");
            audioManager.PlaySecundary(selectedGun);
            reloading = true;
            rechargingDisplay.SetActive(true);
            Invoke("ReloadFinished", guns.getGuns()[selectedGun].reloadTime);
        }
    }

    private void ReloadFinished()
    {

        int bulletsInMagazine = guns.getGuns()[selectedGun].bulletsLeftInMagazine;
        if (bulletsInMagazine + guns.getGuns()[selectedGun].totalBullets >= guns.getGuns()[selectedGun].magazineSize)
        {
            guns.getGuns()[selectedGun].bulletsLeftInMagazine = guns.getGuns()[selectedGun].magazineSize;
            guns.getGuns()[selectedGun].totalBullets -= (guns.getGuns()[selectedGun].magazineSize - bulletsInMagazine);
        }
        else
        {
            guns.getGuns()[selectedGun].bulletsLeftInMagazine = guns.getGuns()[selectedGun].totalBullets + bulletsInMagazine;
            guns.getGuns()[selectedGun].totalBullets = 0;
        }

        reloading = false;
        rechargingDisplay.SetActive(false);
        Shooting(); // Llamamos a esta funcion en caso de que el jugador siga con el click de rat�n pulsado, empiece a disparar
    }

    public void SwapGun()
    {

        // Explora las opciones de armas en orden. Hay dos bucles ya que podriamos estar posicionados en el arma n�mero 1,
        // por lo que explora primero hacia arriba y luego da la vuelta y explora las que jer�rquicamente est�n por debajo

        for (int i = selectedGun + 1; i < availableGuns.Length; i++)
        {
            if (availableGuns[i])
            {
                meshFilter.sharedMesh = weapon_meshes[i];
                selectedGun = i;
                Cursor.SetCursor(cursorSprites[selectedGun], cursorHotSpot, CursorMode.Auto);
                // Se resetea el disparo para quitar el enfriamiento del anterior arma y poder disparar de inmediato (aunque deber�a llamarse mejor al finalizar la animaci�n)
                ResetShot();
                // AQUI VA LA ANIMACI�N DE CAMBIO DE ARMA (TENER EN CUENTA LO QUE PONE 2 LINEAS M�S ARRIBA)
                return;
            }
        }

        for (int i = 0; i < selectedGun; i++)
        {
            if (availableGuns[i])
            {
                meshFilter.sharedMesh = weapon_meshes[i];
                selectedGun = i;
                Cursor.SetCursor(cursorSprites[selectedGun], cursorHotSpot, CursorMode.Auto);
                ResetShot();
                // AQUI VA LA ANIMACI�N DE CAMBIO DE ARMA
                return;
            }
        }
    }

    public void ActivateGun(int idx)
    {
        availableGuns[idx] = true;
    }

    public void DeactivateGun(int idx)
    {
        availableGuns[idx] = false;
    }

    public void ChangeCursorBack() {
        Cursor.SetCursor(cursorSprites[selectedGun], cursorHotSpot, CursorMode.Auto);
    }

    private void InitGunsMobile() {
        for (int i = 0; i < guns.getGuns().Length; i++) {
            guns.getGuns()[i].automaticGun = false;
        }
    }
}
