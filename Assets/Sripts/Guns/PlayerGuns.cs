using System.Collections;
using System.Collections.Generic;

public class PlayerGuns
{
    private Gun[] existingGuns = new Gun[2];

    public PlayerGuns() {
        InitializePistol();
        InitializeSubfusil();
    }

    private void InitializePistol() { 
        Gun pistol = new Gun();
        pistol.bulletDamage = 10;
        pistol.magazineSize = 10;
        pistol.bulletsLeft = pistol.magazineSize;
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
        subfusil.bulletsLeft = subfusil.magazineSize;
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

    public Gun[] getGuns() {
        return existingGuns;
    }

}
