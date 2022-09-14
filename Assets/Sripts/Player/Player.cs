using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    private float speed = 6f;

    Vector3 movement;
    private Rigidbody rb;
    int floorMask;
    float camRayLength = 100f;

    private Vector2 CachedMoveInput { get; set; }
    private Vector2 CachedAimInput { get; set; }

    private void Awake()
    {
        // Init components
        rb = GetComponent<Rigidbody>();

        // Create Input System
        PlayerInputActions playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Shoot.performed += Shoot;
        playerInputActions.Player.Movement.performed += Movement;
        playerInputActions.Player.Movement.canceled += ResetMovement;
        playerInputActions.Player.Aim.performed += MousePosition;

        // Others
        floorMask = LayerMask.GetMask("Floor");
    }

    private void FixedUpdate()
    {
        Move();
        Aim();
    }


    public void Shoot(InputAction.CallbackContext context)
    {
        Debug.Log(context.phase);
        if (context.performed) { Debug.Log("Shoot!"); }
    }

    public void Movement(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        //Debug.Log(context.phase);
        CachedMoveInput = context.ReadValue<Vector2>();
    }
    public void ResetMovement(InputAction.CallbackContext context)
    {
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
