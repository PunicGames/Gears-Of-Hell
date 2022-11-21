using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum effect { HEAL, AMMO, RAPIDFIRE, FAST, VEST }

public class PlayerVFXsManager : MonoBehaviour
{
    [SerializeField] ParticleSystem HealVFX;
    [SerializeField] ParticleSystem AmmoVFX;
    [SerializeField] ParticleSystem RapidFireVFX;
    [SerializeField] ParticleSystem FastBootsVFX;
    [SerializeField] ParticleSystem VestVFX;

    private void Start()
    {
        GetComponentInParent<Player>().onItemTaken += ActivateVFX;
    }
    public void ActivateVFX(effect vfx)
    {
        switch (vfx)
        {
            case effect.HEAL:
                HealVFX.Play();
                break;
            case effect.AMMO:
                AmmoVFX.Play();
                break;
            case effect.RAPIDFIRE:
                RapidFireVFX.Play();
                break;
            case effect.FAST:
                FastBootsVFX.Play();
                break;
            case effect.VEST:
                VestVFX.Play();
                break;
            default:
                break;
        }
    }
    public void DeactivateVFX(effect vfx)
    {
        switch (vfx)
        {

            case effect.RAPIDFIRE:
                RapidFireVFX.Stop();
                break;
            case effect.FAST:
                FastBootsVFX.Stop();
                break;
            case effect.VEST:
                VestVFX.Stop();
                break;
            default:
                break;
        }
    }
}
