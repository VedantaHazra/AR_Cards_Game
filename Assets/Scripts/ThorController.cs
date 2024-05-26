using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ThorController : MonoBehaviour
{
    private Player playerInput;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private Transform child;
    private bool groundedPlayer;
    private Animator animator;
    private bool isAttacking;  // Track if attack animation is playing
    private bool isAttackingShort;  // Track if attack animation is playing


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

        playerInput.ThorMain.Attack.started += OnAttackStarted;
        playerInput.ThorMain.Shoot.started += OnShootStarted;
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
            animator.SetBool("isJumping", false);  // Ensure isJumping is false when grounded
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
            // playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
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

    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        if (!isAttacking)
        {
            isAttacking = true;
            StartCoroutine(HandleAttackSequence());
        }
    }

    private IEnumerator HandleAttackSequence()
    {
        // Play the battlecry animation
        animator.SetBool("isRecharging", true);
        yield return new WaitForSeconds(2.5f); // Adjust to the duration of the battlecry animation

        // Set isRecharging to false and isAttacking to true
        animator.SetBool("isThortacking", true);
        animator.SetBool("isRecharging", false);


        // Wait for the actual attack animation duration
        yield return new WaitForSeconds(0.8f); // Adjust to the duration of the attack animation

        // Reset the attacking state
        animator.SetBool("isThortacking", false);
        animator.SetBool("isIdle", true);
        isAttacking = false;
    }
    private void OnShootStarted(InputAction.CallbackContext context)
    {
        if (!isAttackingShort)
        {
            isAttackingShort = true;
            StartCoroutine(HandleShortAttackSequence());




        }
    }
    private IEnumerator HandleShortAttackSequence()
    {
        animator.SetBool("isIdle", false);

        animator.SetBool("isAttackingShort", true);
        yield return new WaitForSeconds(2.15f);

        animator.SetBool("isAttackingShort", false);
        animator.SetBool("isIdle", true);


        isAttackingShort = false;



    }

}
