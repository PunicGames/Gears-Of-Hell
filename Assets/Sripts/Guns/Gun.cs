public class Gun
{
    public int bulletsLeftInMagazine;
    public int bulletsShot;
    public int bulletDamage;
    public int magazineSize;
    public int totalBullets;
    public int maxTotalBullets;
    public int bulletsPerTap;

    public float shootForce;
    public float spread;
    public float reloadTime;
    public float timeBetweenShots;    // Tiempo de espera al tener arma automática
    public float timeBetweenShooting; // Tiempo de espera entre disparo y disparo
    public float bulletLifeTime; 

    public bool automaticGun;

    public void AddAmmo(int ammo){
        totalBullets += ammo;
    }
}
