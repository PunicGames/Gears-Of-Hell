using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    Vector3 movement;
    int floorMask;
    public float speed = 6f;
    float camRayLength = 100f;

    private Rigidbody rb;
    private ShootSystem shootingSystem;
    private Vector2 CachedMoveInput { get; set; }
    private Vector2 CachedAimInput { get; set; }

    private Animator playerAnimator;

    private AudioSource footSteps;

    public GameObject mobileUI;
    private bool desktop = true;

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        // Init components
        rb = GetComponent<Rigidbody>();
        shootingSystem = GetComponentInChildren<ShootSystem>();
        playerAnimator = GetComponent<Animator>();
        footSteps = GetComponent<AudioSource>();

        // Others
        floorMask = LayerMask.GetMask("Floor");

        if (Application.isMobilePlatform)
        {
            desktop = false;
        }
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            desktop = true;
        }

        //desktop = false;

        // Input actions
        playerInputActions = new PlayerInputActions();

    }
    private void Start()
    {
        playerAnimator.SetBool("isRifle", true);
    }
    private void Update()
    {
        float angle = Vector3.Angle(transform.forward, movement);

        if (angle > 40)
        {
            float signedAngle = Vector3.SignedAngle(transform.forward, movement, transform.up);
            if (signedAngle < 140 && signedAngle > 0)
                playerAnimator.SetFloat("VelX", Mathf.Lerp(playerAnimator.GetFloat("VelX"), 1.5f, Time.deltaTime * 10));
            else if (signedAngle < 0 && signedAngle > -140)
                playerAnimator.SetFloat("VelX", Mathf.Lerp(playerAnimator.GetFloat("VelX"), 2f, Time.deltaTime * 10));

            //if (angle < 140) //playerAnimator.SetFloat("VelX", 1.5f); 
            //    playerAnimator.SetFloat("VelX", Mathf.Lerp(playerAnimator.GetFloat("VelX"), 1.5f, Time.deltaTime * 15));
            else
                playerAnimator.SetFloat("VelX", Mathf.Lerp(playerAnimator.GetFloat("VelX"), 1, Time.deltaTime * 10));
            //Activa animación hacia atras
            //playerAnimator.GetFloat("VelX");
            //playerAnimator.SetFloat("VelX", 1);
        }
        else
            //Activa animación hacia delante
            //playerAnimator.SetFloat("VelX", 0);
            playerAnimator.SetFloat("VelX", Mathf.Lerp(playerAnimator.GetFloat("VelX"), 0, Time.deltaTime * 10));
    }

    private void FixedUpdate()
    {

        // En principio cuando está el menu de pausa timeScale es 0 y el FixedUpdate no se ejecuta. Aun así, comprobamos por si acaso.
        if (PauseMenu.GameIsPaused) return;

        Move();
        if (desktop)
        {
            Aim();
        }
        else
        {
            MobileAim();
        }

    }


    public void Shoot(InputAction.CallbackContext context)
    {
        if (!PauseMenu.GameIsPaused) { 
            shootingSystem.shooting = true;
            shootingSystem.Shooting(playerAnimator);
        }
    }

    public void ResetShoot(InputAction.CallbackContext context)
    {
        if (!PauseMenu.GameIsPaused)
        {
            shootingSystem.shooting = false;
        }
    }

    public void Movement(InputAction.CallbackContext context)
    {
        if (!PauseMenu.GameIsPaused)
        {
            playerAnimator.SetBool("isMoving", true);
            CachedMoveInput = context.ReadValue<Vector2>();
            if (!footSteps.isPlaying)
                footSteps.Play();
        }
    }
    public void ResetMovement(InputAction.CallbackContext context)
    {
        if (!PauseMenu.GameIsPaused)
        {
            playerAnimator.SetBool("isMoving", false);
            CachedMoveInput = new Vector2(0.0f, 0.0f);
            movement = movement * speed * Time.deltaTime;
            if (footSteps.isPlaying)
                footSteps.Pause();
        }
        
    }
    public void MousePosition(InputAction.CallbackContext context)
    {
        if (!PauseMenu.GameIsPaused)
        {
            CachedAimInput = context.ReadValue<Vector2>();
        }
    }

    public void PauseMenuCall(InputAction.CallbackContext context)
    {
        PauseMenu.TriggerPause = true;
    }

    private void Move()
    {
        movement.Set(CachedMoveInput.x, 0.0f, CachedMoveInput.y);
        movement = movement * speed * Time.deltaTime;
        
            rb.MovePosition(transform.position + movement);
    }

    private void Aim()
    {

        Ray camRay = Camera.main.ScreenPointToRay(CachedAimInput);

        RaycastHit floorHit;
        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        { // En caso de colisionar...
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0f;

            Quaternion newPlayerRotation = Quaternion.LookRotation(playerToMouse);
            rb.MoveRotation(newPlayerRotation);
        }

    }

    private void MobileAim()
    {
        Vector3 vec = new Vector3(CachedAimInput.x, 0f, CachedAimInput.y);
        Quaternion newPlayerRotation = Quaternion.LookRotation(vec);
        rb.MoveRotation(newPlayerRotation);
    }

    private void ReloadGun(InputAction.CallbackContext context) {
        if (!PauseMenu.GameIsPaused)
        {
            shootingSystem.Reload();
        }
    }

    private void SwapGun(InputAction.CallbackContext context) {
        if (!PauseMenu.GameIsPaused)
        {
            shootingSystem.SwapGun();
        }
    }

    private void OnEnable()
    {

        playerInputActions.Player.Enable();

        if (desktop)
        {
            //desktop = true;
            playerInputActions.Player.Shoot.performed += Shoot;
            playerInputActions.Player.Shoot.canceled += ResetShoot;
            playerInputActions.Player.Movement.performed += Movement;
            playerInputActions.Player.Movement.canceled += ResetMovement;
            playerInputActions.Player.Aim.performed += MousePosition;
            playerInputActions.Player.Esc.performed += PauseMenuCall;
            playerInputActions.Player.Recharge.performed += ReloadGun;
            playerInputActions.Player.SwapGun.performed += SwapGun;
        }
        else
        {
            //desktop = false;
            mobileUI.SetActive(true);

            playerInputActions.Player.MobileMovement.performed += Movement;
            playerInputActions.Player.MobileMovement.canceled += ResetMovement;
            playerInputActions.Player.MobileAim.performed += MousePosition;
            playerInputActions.Player.MobileAim.performed += Shoot;
            playerInputActions.Player.MobileAim.canceled += ResetShoot;
            // PONER LA RECARGA CON EL MOVIL
            // PONER CAMBIO DE ARMA CON EL MOVIL
        }
    }

    private void OnDisable()
    {
        if (desktop)
        {
            playerInputActions.Player.Shoot.performed -= Shoot;
            playerInputActions.Player.Shoot.canceled -= ResetShoot;
            playerInputActions.Player.Movement.performed -= Movement;
            playerInputActions.Player.Movement.canceled -= ResetMovement;
            playerInputActions.Player.Aim.performed -= MousePosition;
            playerInputActions.Player.Esc.performed -= PauseMenuCall;
            playerInputActions.Player.Recharge.performed -= ReloadGun;
            playerInputActions.Player.SwapGun.performed -= SwapGun;
        }
        else {
            playerInputActions.Player.MobileMovement.performed -= Movement;
            playerInputActions.Player.MobileMovement.canceled -= ResetMovement;
            playerInputActions.Player.MobileAim.performed -= MousePosition;
            playerInputActions.Player.MobileAim.performed -= Shoot;
            playerInputActions.Player.MobileAim.canceled -= ResetShoot;
            // PONER LA RECARGA CON EL MOVIL
            // PONER CAMBIO DE ARMA CON EL MOVIL

        }
    }
}
