using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    Vector3 movement;
    int floorMask;
    private float speed = 6f;
    float camRayLength = 100f;

    private Rigidbody rb;
    private ShootSystem shootingSystem;
    private Vector2 CachedMoveInput { get; set; }
    private Vector2 CachedAimInput { get; set; }

    private Animator playerAnimator;

    private void Awake()
    {
        // Init components
        rb = GetComponent<Rigidbody>();
        shootingSystem = GetComponentInChildren<ShootSystem>();
        playerAnimator = GetComponent<Animator>();

        // Create Input System
        PlayerInputActions playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Shoot.performed += Shoot;
        playerInputActions.Player.Shoot.canceled += ResetShoot;
        playerInputActions.Player.Movement.performed += Movement;
        playerInputActions.Player.Movement.canceled += ResetMovement;
        playerInputActions.Player.Aim.performed += MousePosition;

        // Others
        floorMask = LayerMask.GetMask("Floor");
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
        Move();
        Aim();

    }


    public void Shoot(InputAction.CallbackContext context)
    {

        shootingSystem.shooting = true;
        shootingSystem.Shooting(playerAnimator);
    }

    public void ResetShoot(InputAction.CallbackContext context)
    {
        shootingSystem.shooting = false;
    }

    public void Movement(InputAction.CallbackContext context)
    {
        //Debug.Log(context);
        //Debug.Log(context.phase);
        playerAnimator.SetBool("isMoving", true);
        CachedMoveInput = context.ReadValue<Vector2>();
    }
    public void ResetMovement(InputAction.CallbackContext context)
    {
        playerAnimator.SetBool("isMoving", false);
        CachedMoveInput = new Vector2(0.0f, 0.0f);
    }
    public void MousePosition(InputAction.CallbackContext context)
    {
        CachedAimInput = context.ReadValue<Vector2>();
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
}
