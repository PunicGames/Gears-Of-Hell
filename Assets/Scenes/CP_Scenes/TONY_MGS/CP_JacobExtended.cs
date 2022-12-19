using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_JacobExtended : MonoBehaviour
{
    [SerializeField] ShootSystem shoot_sys;

    public float shootSoundRadio = 5;

    private SoundEmitter emitter;

    void Start()
    {
        shoot_sys.onShootWeapon += ShootSound;
        emitter = GetComponent<SoundEmitter>();
        emitter.radius = shootSoundRadio;
    }

    private void ShootSound(bool t)
    {
        emitter.MakeSound(shootSoundRadio);

    }
    private void Walk()
    {

        //make sound
        // depending if crouched or not
    }

}
