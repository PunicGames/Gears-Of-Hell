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

    public Animator playerAnimator;

    [HideInInspector]
    public AudioSource footSteps, hitmarker;

    private bool desktop = true;

    private PlayerInputActions playerInputActions;

    // Shop system
    private bool shopInRange;
    private bool shopActive;
    private GestorUIinGame uiGestor;

    //Camera reference
    private Transform cam;

    //Joysticks reference
    private VariableJoystick leftJoystick;
    private VariableJoystick rightJoystick;

    //Delegates
    public delegate void OnItemTaken(effect vfx);
    public OnItemTaken onItemTaken;


    private void Awake()
    {
        // Init components
        rb = GetComponent<Rigidbody>();
        shootingSystem = GetComponentInChildren<ShootSystem>();
        playerAnimator = GetComponent<Animator>();
        var audioSources = GetComponents<AudioSource>();
        footSteps = audioSources[0];
        hitmarker = audioSources[1];

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

        // Shop
        uiGestor = GameObject.Find("InGameUI").GetComponent<GestorUIinGame>();

    }
    private void Start()
    {
        //playerAnimator.SetBool("isRifle", true);
        footSteps.volume *= AudioManager.getGeneralVolume();

        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;

        if (!desktop)
        {
            leftJoystick = GameObject.Find("LeftJoystick").GetComponent<VariableJoystick>();
            rightJoystick = GameObject.Find("RightJoystick").GetComponent<VariableJoystick>();
        }
        
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

        if (!desktop)
        {
            MobileMovement(leftJoystick.input);
            CachedAimInput = rightJoystick.input;

        }

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
        if (!PauseMenu.GameIsPaused && !uiGestor.shooping) { 
            shootingSystem.shooting = true;
            shootingSystem.Shooting();
        }
    }

    public void ResetShoot(InputAction.CallbackContext context)
    {
        if (!PauseMenu.GameIsPaused && !uiGestor.shooping)
        {
            shootingSystem.shooting = false;
        }
    }

    public void Movement(InputAction.CallbackContext context)
    {
        if (!PauseMenu.GameIsPaused && !uiGestor.shooping)
        {
            playerAnimator.SetBool("isMoving", true);
            CachedMoveInput = context.ReadValue<Vector2>();
            if (!footSteps.mute && !footSteps.isPlaying )
                footSteps.Play();
        }
    }

    public void PlayHitMarker() 
    {
        hitmarker.Play();
    }

    public void MobileMovement(Vector2 context)
    {
        if (!PauseMenu.GameIsPaused && !uiGestor.shooping)
        {
            playerAnimator.SetBool("isMoving", true);
            CachedMoveInput = context;
            if (context == Vector2.zero)
                MobileResetMovement();
            if (!footSteps.mute && !footSteps.isPlaying && context!=Vector2.zero)
                footSteps.Play();
        }
    }

    public void ResetMovement(InputAction.CallbackContext context)
    {
        if (!PauseMenu.GameIsPaused && !uiGestor.shooping)
        {
            playerAnimator.SetBool("isMoving", false);
            CachedMoveInput = new Vector2(0.0f, 0.0f);
            movement = movement * speed * Time.deltaTime;
            if (footSteps.isPlaying)
                footSteps.Pause();
        }
        
    }

    public void MobileResetMovement()
    {
        if (!PauseMenu.GameIsPaused && !uiGestor.shooping)
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
        if (!PauseMenu.GameIsPaused && !uiGestor.shooping)
        {
            CachedAimInput = context.ReadValue<Vector2>();
        }
    }

    public void PauseMenuCall(InputAction.CallbackContext context)
    {
        // Solo se puede pausar el juego si el jugador no se encuantra disparando
        if (!shootingSystem.shooting && !uiGestor.shooping) { 
            PauseMenu.TriggerPause = true;
        }
    }

    private void Move()
    {
        if (!PauseMenu.GameIsPaused && !uiGestor.shooping)
        {
            movement.Set(CachedMoveInput.x, 0.0f, CachedMoveInput.y);
            movement = Quaternion.Euler(0, cam.rotation.eulerAngles.y, 0) * movement;
            movement = movement * speed * Time.deltaTime;

            rb.MovePosition(transform.position + movement);
        }
    }

    private void Aim()
    {
        if (!PauseMenu.GameIsPaused && !uiGestor.shooping)
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

    }

    private void MobileAim()
    {
        if (!PauseMenu.GameIsPaused && !uiGestor.shooping)
        {

            if (CachedAimInput != Vector2.zero)
            {
                Vector3 vec = new Vector3(CachedAimInput.x, 0f, CachedAimInput.y);
                vec = Quaternion.Euler(0, cam.rotation.eulerAngles.y, 0) * vec;
                Quaternion newPlayerRotation = Quaternion.LookRotation(vec);
                rb.MoveRotation(newPlayerRotation);
            }
        }
        
        float aux = CachedAimInput.magnitude;

        //print(aux);

        if (aux > 0.7)
        {
            shootingSystem.shooting = true;
            shootingSystem.Shooting();
        }
        else
        {
            shootingSystem.shooting = false;
        }
        
    }

    private void ResetAim(InputAction.CallbackContext context)
    {
        CachedAimInput = Vector2.zero;
    }

    private void ReloadGun(InputAction.CallbackContext context) {
        if (!PauseMenu.GameIsPaused && !uiGestor.shooping)
        {
            shootingSystem.Reload();
        }
    }

    private void SwapGun(InputAction.CallbackContext context) {
        if (!PauseMenu.GameIsPaused && !uiGestor.shooping)
        {
            shootingSystem.SwapGun();
        }
    }

    private void OpenShop(InputAction.CallbackContext context) {
        // Se puede acceder a la tienda si el jugador no se encuentra disparando
        if (!PauseMenu.GameIsPaused && shopInRange && !shootingSystem.shooting && shopActive)
        {
            // Abrir tienda
            //Debug.Log("Se abre la tienda");
            GameObject.Find("InGameMusic").GetComponent<InGameMusicManager>().PlayShopMusic();

            CachedMoveInput = new Vector2(0.0f, 0.0f);
            uiGestor.ShowShop();
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
            playerInputActions.Player.Pause.performed += PauseMenuCall;
            playerInputActions.Player.Recharge.performed += ReloadGun;
            playerInputActions.Player.SwapGun.performed += SwapGun;
            playerInputActions.Player.Shop.performed += OpenShop;
        }
        else
        {
            //desktop = false;

            //playerInputActions.Player.MobileMovement.performed += Movement;
            //playerInputActions.Player.MobileMovement.canceled += ResetMovement;
            //playerInputActions.Player.MobileAim.performed += MousePosition;
            //playerInputActions.Player.MobileAim.canceled += ResetShoot;
            //playerInputActions.Player.MobileAim.canceled += ResetAim;
            playerInputActions.Player.Recharge.performed += ReloadGun;
            playerInputActions.Player.SwapGun.performed += SwapGun;
            playerInputActions.Player.Pause.performed += PauseMenuCall;
            playerInputActions.Player.Shop.performed += OpenShop;
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
            playerInputActions.Player.Pause.performed -= PauseMenuCall;
            playerInputActions.Player.Recharge.performed -= ReloadGun;
            playerInputActions.Player.SwapGun.performed -= SwapGun;
            playerInputActions.Player.Shop.performed -= OpenShop;
        }
        else {
            //playerInputActions.Player.MobileMovement.performed -= Movement;
            //playerInputActions.Player.MobileMovement.canceled -= ResetMovement;
            //playerInputActions.Player.MobileAim.performed -= MousePosition;
            //playerInputActions.Player.MobileAim.canceled -= ResetShoot;
            //playerInputActions.Player.MobileAim.canceled -= ResetAim;
            playerInputActions.Player.Recharge.performed -= ReloadGun;
            playerInputActions.Player.SwapGun.performed -= SwapGun;
            playerInputActions.Player.Pause.performed -= PauseMenuCall;
            playerInputActions.Player.Shop.performed -= OpenShop;
        }
    }



    // Sistema de tiendas
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shop") {
            shopInRange = true;
            if (other.gameObject.GetComponent<Shop>().active) {
                shopActive = true;
                Debug.Log("Dentro de tienda");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Shop")
        {
            shopInRange = false;
            shopActive = false;
        }
    }
}
