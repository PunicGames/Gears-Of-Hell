using System.Collections;
using System.Collections.Generic;

public class PlayerGuns
{
    private Gun[] existingGuns = new Gun[5];

    public PlayerGuns()
    {
        InitializePistol();
        InitializeSubfusil();
        InitializeRifle();
        InitializeSniper();
        InitializeShotgun();
    }

    private void InitializePistol()
    {
        Gun pistol = new Gun();
        pistol.bulletDamage = 10;
        pistol.magazineSize = 10;
        pistol.bulletsLeftInMagazine = pistol.magazineSize;
        pistol.maxTotalBullets = 99999;
        pistol.totalBullets = pistol.maxTotalBullets - pistol.magazineSize;
        pistol.bulletsShot = 0;
        pistol.bulletsPerTap = 1;
        pistol.shootForce = 20;
        pistol.spread = 0;
        pistol.reloadTime = 1;
        pistol.timeBetweenShots = 0.01f;
        pistol.timeBetweenShooting = 0.2f;
        pistol.automaticGun = false;
        pistol.bulletLifeTime = 2f;
        existingGuns[0] = pistol;
    }

    private void InitializeSubfusil()
    {
        Gun subfusil = new Gun();
        subfusil.bulletDamage = 8;
        subfusil.magazineSize = 50;
        subfusil.bulletsLeftInMagazine = subfusil.magazineSize;
        subfusil.maxTotalBullets = 350;
        subfusil.totalBullets = subfusil.maxTotalBullets - subfusil.magazineSize;
        subfusil.bulletsShot = 0;
        subfusil.bulletsPerTap = 1;
        subfusil.shootForce = 10;
        subfusil.spread = 0.0f;
        subfusil.reloadTime = 1.5f;
        subfusil.timeBetweenShots = 0.1f;
        subfusil.timeBetweenShooting = 0.1f;
        subfusil.automaticGun = true;
        subfusil.bulletLifeTime = 1.5f;
        existingGuns[1] = subfusil;
    }

    private void InitializeRifle()
    {
        Gun rifle = new Gun();
        rifle.bulletDamage = 15;
        rifle.magazineSize = 25;
        rifle.bulletsLeftInMagazine = rifle.magazineSize;
        rifle.maxTotalBullets = 250;
        rifle.totalBullets = rifle.maxTotalBullets - rifle.magazineSize;
        rifle.bulletsShot = 0;
        rifle.bulletsPerTap = 1;
        rifle.shootForce = 25;
        rifle.spread = 0;
        rifle.reloadTime = 1;
        rifle.timeBetweenShots = 0.2f;
        rifle.timeBetweenShooting = 0.2f;
        rifle.automaticGun = true;
        rifle.bulletLifeTime = 3f;
        existingGuns[2] = rifle;
    }

    private void InitializeSniper()
    {
        Gun sniper = new Gun();
        sniper.bulletDamage = 60;
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
        sniper.timeBetweenShooting = 2.0f;
        sniper.automaticGun = false;
        sniper.bulletLifeTime = 4f;
        existingGuns[3] = sniper;
    }

    private void InitializeShotgun()
    {
        Gun shotgun = new Gun();
        shotgun.bulletDamage = 8;
        shotgun.magazineSize = 10;
        shotgun.bulletsLeftInMagazine = shotgun.magazineSize;
        shotgun.maxTotalBullets = shotgun.magazineSize * 5;
        shotgun.totalBullets = shotgun.maxTotalBullets - shotgun.magazineSize;
        shotgun.bulletsShot = 0;
        shotgun.bulletsPerTap = 1;
        shotgun.shootForce = 25;
        shotgun.spread = 0.25f;
        shotgun.reloadTime = 2.0f;
        shotgun.timeBetweenShots = 0.0f;
        shotgun.timeBetweenShooting = 1.0f;
        shotgun.automaticGun = false;
        shotgun.bulletLifeTime = 0.3f;
        existingGuns[4] = shotgun;
    }

    public Gun[] getGuns()
    {
        return existingGuns;
    }

    public void AddAmmo(int index, int ammo)
    {
        getGuns()[index].AddAmmo(ammo);
    }

}

