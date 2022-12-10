using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public enum effect { HEAL, AMMO, MUZZLE,LASER,RAPID,FAST,VEST,MEDIC,REPLENISH,BIG}

public class PlayerVFXsManager : MonoBehaviour
{
    [SerializeField] ParticleSystem HealVFX;
    [SerializeField] ParticleSystem AmmoVFX;
    
    [SerializeField] ParticleSystem MuzzleVFX;

    [SerializeField] ParticleSystem LaserMuzzleVFX;
   
    [SerializeField] ParticleSystem RapidFireVFX;
    [SerializeField] ParticleSystem FastBootsRVFX;
    [SerializeField] ParticleSystem FastBootsLVFX;
    [SerializeField] ParticleSystem VestVFX;
    [SerializeField] ParticleSystem MedicVFX;
    [SerializeField] ParticleSystem AmmoReplenishmentVFX;
    [SerializeField] ParticleSystem BigShootVFX;

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
        LaserMuzzleVFX.transform.localPosition = new Vector3(p.x, p.y, p.z+0.230F);
    }
    public void startVFX(effect type)
    {
        switch (type)
        {
            case effect.HEAL:
                HealVFX.Play();
                break;
            case effect.AMMO:
                AmmoVFX.Play();
                break;
            case effect.MUZZLE:
                MuzzleVFX.Play();
                break;
            case effect.LASER:
                LaserMuzzleVFX.Play();
                break;
            case effect.RAPID:
                RapidFireVFX.Play();
                break;
            case effect.FAST:
                FastBootsRVFX.Play();
                FastBootsLVFX.Play();
                break;
            case effect.VEST:
                VestVFX.Play();
                break;
            case effect.MEDIC:
                MedicVFX.Play();
                break;
            case effect.REPLENISH:
                AmmoReplenishmentVFX.Play();
                break;
            case effect.BIG:
                BigShootVFX.Play();
                break;
            default:
                break;
        }
    }
    public void stopVFX(effect type)
    {
        switch (type)
        {
            case effect.HEAL:
                HealVFX.Stop();
                break;
            case effect.AMMO:
                AmmoVFX.Stop();
                break;
            case effect.MUZZLE:
                MuzzleVFX.Stop();
                break;
            case effect.LASER:
                LaserMuzzleVFX.Stop();
                break;
            case effect.RAPID:
                RapidFireVFX.Stop();
                break;
            case effect.FAST:
                FastBootsRVFX.Stop();
                FastBootsLVFX.Stop();
                break;
            case effect.VEST:
                VestVFX.Stop();
                break;
            case effect.MEDIC:
                break;
            case effect.REPLENISH:
                MedicVFX.Stop();
                break;
            default:
                break;
        }
    }

}
