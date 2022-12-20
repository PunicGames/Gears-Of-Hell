using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CP_JacobExtended : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    private SoundEmitter emitter;

    private bool isPaused = false;
    [SerializeField] Tonys_ShootSystem shoot_sys;
    [SerializeField] AudioSource crouch_steps;

    public float shootSoundRadius = 4;
    public float crouchSoundRadius = 1.5f;
    public float walkSoundRadius = 6;
    private bool isCrouching;

    void Start()
    {
        playerInputActions = GetComponent<Player>().playerInputActions;
        playerInputActions.Player.Shoot.performed += Shoot;
        playerInputActions.Player.Shoot.canceled += ResetShoot;
        playerInputActions.Player.Crouch.performed += Crouch;
        playerInputActions.Player.Crouch.canceled += Crouch;
        playerInputActions.Player.Pause.performed += PauseMenuCall;


        shoot_sys.onShootWeapon += ShootSound;
        emitter = GetComponent<SoundEmitter>();
        emitter.radius = shootSoundRadius;
    }

    private void Update()
    {
        if (GetComponent<Player>().isMoving)
        {
            if (isCrouching)

                emitter.MakeSound(crouchSoundRadius);
            else
                emitter.MakeSound(walkSoundRadius);
        }

    }
    public void PauseMenuCall(InputAction.CallbackContext context)
    {
        // Solo se puede pausar el juego si el jugador no se encuantra disparando
        if (!isPaused)
            PauseMenu.TriggerPause = true;
        else
            PauseMenu.TriggerPause = false;

    }

    private void ShootSound(bool t)
    {
        emitter.MakeSound(shootSoundRadius);

    }


    public void Shoot(InputAction.CallbackContext context)
    {
        if (!PauseMenu.GameIsPaused)
        {

            shoot_sys.shooting = true;
            shoot_sys.Shooting();

        }
    }

    public void ResetShoot(InputAction.CallbackContext context)
    {
        if (!PauseMenu.GameIsPaused)
        {

            shoot_sys.shooting = false;
        }
    }
    public void Crouch(InputAction.CallbackContext context)
    {
        if (!isCrouching)
        {
            GetComponent<Player>().footSteps.volume = 0.25f;
            GetComponent<Player>().footSteps.pitch = .69f;
            GetComponent<Player>().speed *= .5f;
            isCrouching = true;
            GetComponent<Animator>().SetBool("isCrouching", true);
        }
        else
        {
            GetComponent<Player>().footSteps.volume = 1f;
            GetComponent<Player>().footSteps.pitch = 1;
            GetComponent<Player>().speed *= 2f;
            isCrouching = false;
            GetComponent<Animator>().SetBool("isCrouching", false);
        }

    }


}
