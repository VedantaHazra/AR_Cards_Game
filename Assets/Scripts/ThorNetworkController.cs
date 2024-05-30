using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Netcode;


public class ThorNetworkController : NetworkBehaviour
{
    public GameObject InputUI;
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
    [SerializeField]
    private float attackMoveSpeed = 5f; // Speed at which the character moves during the attack
    private bool isOwner;

    public override void OnNetworkSpawn()
    {
        
        if(GetComponent<NetworkObject>().IsOwner)
        {
            InputUI.SetActive(true);
            Debug.Log(IsOwner);
            isOwner = true;
        playerInput = new Player();
        playerInput.ThorMain.Attack.started += OnAttackStarted;
        playerInput.ThorMain.Shoot.started += OnShootStarted;
        SetOwnerIDToDamageGiverServerRpc(NetworkManager.Singleton.LocalClientId);
        }
        else{
            isOwner = false;
        }
    }
    [ServerRpc]
    void SetOwnerIDToDamageGiverServerRpc(ulong clientID)
    {
        gameObject.GetComponentInChildren<GiveDamageScript>().SetOwnershipServerRpc(clientID);
    }
    public override void OnNetworkDespawn()
    {
        if(GetComponent<NetworkObject>().IsOwner)
        {
        playerInput.ThorMain.Attack.started += OnAttackStarted;
        playerInput.ThorMain.Shoot.started += OnShootStarted;
        }
    }
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

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
        if(!isOwner) { return; }
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

        float attackDuration = 1.46f; // Adjust to the duration of the attack animation
        float elapsedTime = 0f;

        // Move the character forward during the attack animation
        while (elapsedTime < attackDuration)
        {
            controller.Move(transform.forward * attackMoveSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

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
        yield return new WaitForSeconds(2.15f); // Adjust to the duration of the short attack animation

        animator.SetBool("isAttackingShort", false);
        animator.SetBool("isIdle", true);
        isAttackingShort = false;
    }

}
