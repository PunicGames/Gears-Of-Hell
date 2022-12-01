using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public enum effect { HEAL, AMMO}

public class PlayerVFXsManager : MonoBehaviour
{
    [SerializeField] ParticleSystem HealVFX;
    [SerializeField] ParticleSystem AmmoVFX;
    
    [SerializeField] ParticleSystem MuzzleVFX;

    [SerializeField] ParticleSystem LaserMuzzleVFX;
   
    [SerializeField] ParticleSystem RapidFireVFX;
    [SerializeField] ParticleSystem FastBootsVFX;
    [SerializeField] ParticleSystem VestVFX;

    private void Start()
    {
        Player p = GetComponentInParent<Player>();
        p.onItemTaken += ActivateConsumableVFX;
        p.shootingSystem.onShootWeapon += ActivateMuzzleVFX;
        p.shootingSystem.onSwapWeapon += ChangeMuzzlePosition;
        
    }
    private void ActivateConsumableVFX(effect vfx)
    {
        switch (vfx)
        {
            case effect.HEAL:
                HealVFX.Play();
                break;
            case effect.AMMO:
                AmmoVFX.Play();
                break;
        }
    }
    private void ActivateMuzzleVFX(bool t)
    {
        if (!t) MuzzleVFX.Play(); else LaserMuzzleVFX.Play();
    }
   private void ChangeMuzzlePosition(Vector3 p)
    {
        MuzzleVFX.transform.localPosition = new Vector3(p.x,p.y,p.z+0.230F);
        LaserMuzzleVFX.transform.localPosition = new Vector3(p.x, p.y, p.z - 0.230F);
    }

}
