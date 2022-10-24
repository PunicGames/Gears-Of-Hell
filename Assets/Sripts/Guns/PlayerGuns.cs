using System.Collections;
using System.Collections.Generic;

public class PlayerGuns
{
    private Gun[] existingGuns = new Gun[3];

    public PlayerGuns() {
        InitializePistol();
        InitializeSubfusil();
        InitializeSniper();
    }

    private void InitializePistol() { 
        Gun pistol = new Gun();
        pistol.bulletDamage = 10;
        pistol.magazineSize = 10;
        pistol.bulletsLeftInMagazine = pistol.magazineSize;
        pistol.maxTotalBullets = 120;
        pistol.totalBullets = pistol.maxTotalBullets - pistol.magazineSize; 
        pistol.bulletsShot = 0;
        pistol.bulletsPerTap = 1;
        pistol.shootForce = 20;
        pistol.spread = 0;
        pistol.reloadTime = 1;
        pistol.timeBetweenShots = 0.2f;
        pistol.timeBetweenShooting = 0.2f;
        pistol.automaticGun = false;
        existingGuns[0] = pistol;
    }

    private void InitializeSubfusil()
    {
        Gun subfusil = new Gun();
        subfusil.bulletDamage = 5;
        subfusil.magazineSize = 30;
        subfusil.bulletsLeftInMagazine = subfusil.magazineSize;
        subfusil.maxTotalBullets = 210;
        subfusil.totalBullets = subfusil.maxTotalBullets - subfusil.magazineSize;      
        subfusil.bulletsShot = 0;
        subfusil.bulletsPerTap = 1;
        subfusil.shootForce = 10;
        subfusil.spread = 0.1f;
        subfusil.reloadTime = 1.5f;
        subfusil.timeBetweenShots = 0.1f;
        subfusil.timeBetweenShooting = 0.2f;
        subfusil.automaticGun = true;
        existingGuns[1] = subfusil;
    }

    private void InitializeSniper()
    {
        Gun sniper = new Gun();
        sniper.bulletDamage = 50;
        sniper.magazineSize = 5;
        sniper.bulletsLeftInMagazine = sniper.magazineSize;
        sniper.maxTotalBullets = 20;
        sniper.totalBullets = sniper.maxTotalBullets - sniper.magazineSize;        
        sniper.bulletsShot = 0;
        sniper.bulletsPerTap = 1;
        sniper.shootForce = 40;
        sniper.spread = 0.0f;
        sniper.reloadTime = 2.0f;
        sniper.timeBetweenShots = 0.0f;
        sniper.timeBetweenShooting = 1.5f;
        sniper.automaticGun = false;
        existingGuns[2] = sniper;
    }

    public Gun[] getGuns() {
        return existingGuns;
    }

}
