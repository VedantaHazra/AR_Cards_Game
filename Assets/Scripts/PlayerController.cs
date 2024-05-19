using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Player playerInput;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private Transform child;
    private bool groundedPlayer;
    private Animator animator;

    [SerializeField]
    private Transform cameraMain;
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = 4f;

    private void Awake()
    {
        playerInput = new Player();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        // Register the callback methods for Aim action
        playerInput.PlayerMain.Aim.started += OnAimStarted;
        playerInput.PlayerMain.Aim.canceled += OnAimCanceled;
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Start()
    {
        cameraMain = Camera.main.transform;
        child = transform.GetChild(0).transform;
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movementInput = playerInput.PlayerMain.Move.ReadValue<Vector2>();

        Vector3 move = cameraMain.forward * movementInput.y + cameraMain.right * movementInput.x;
        move.y = 0f;

        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        if (playerInput.PlayerMain.Jump.WasPressedThisFrame() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            animator.SetBool("isJumping", true);
            animator.SetBool("isIdle", false);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (movementInput != Vector2.zero)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
            Quaternion rotation = Quaternion.Euler(new Vector3(child.localEulerAngles.x, cameraMain.localEulerAngles.y, child.localEulerAngles.z));
            child.rotation = Quaternion.Lerp(child.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            animator.SetBool("isWalking", false);
            if (groundedPlayer)
            {
                animator.SetBool("isIdle", true);
            }
        }

        if (groundedPlayer)
        {
            animator.SetBool("isJumping", false);
            if (movementInput == Vector2.zero)
            {
                animator.SetBool("isIdle", true);
            }
        }
        else
        {
            animator.SetBool("isIdle", false);
        }
    }

    private void OnAimStarted(InputAction.CallbackContext context)
    {
        animator.SetBool("isDrawingArrow", true);
        animator.SetBool("isAiming", false);
    }

    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        animator.SetBool("isDrawingArrow", false);
        animator.SetBool("isAiming", true);
    }
}
