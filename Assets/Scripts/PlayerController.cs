using System.Collections;
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
    private bool isAiming;

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
    private GameObject arrowPrefab;  // Prefab for the arrow
    [SerializeField]
    private Transform arrowSpawnPoint;  // Position to instantiate the arrow

    private void Awake()
    {
        playerInput = new Player();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        playerInput.PlayerMain.Aim.started += OnAimStarted;
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
        if (!isAiming)
        {
            isAiming = true;
            StartCoroutine(HandleAimingSequence());
        }
    }

    private IEnumerator HandleAimingSequence()
    {
        // Set isDrawingArrow to true and wait for the animation duration
        animator.SetBool("isDrawingArrow", true);
        yield return new WaitForSeconds(0.5f); // Adjust to the duration of the drawing arrow animation

        // Set isDrawingArrow to false and isAiming to true, then wait for the animation duration
        animator.SetBool("isAiming", true);
        animator.SetBool("isDrawingArrow", false);

        yield return new WaitForSeconds(1.5f); // Adjust to the duration of the aiming animation

        // Set isAiming to false and isShooting to true
        animator.SetBool("isShooting", true);
        animator.SetBool("isAiming", false);


        // Shoot the arrow


        // Wait for the shooting animation duration
        yield return new WaitForSeconds(0.5f); // Adjust to the duration of the shooting animation

        // Set isShooting to false and isIdle to true
        animator.SetBool("isIdle", true);
        animator.SetBool("isShooting", false);


        // Reset aiming state
        isAiming = false;
    }

    private void ShootArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        // Find the closest enemy with the "Enemy" tag
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Vector3 direction = (closestEnemy.transform.position - arrowSpawnPoint.position).normalized;
            rb.velocity = direction * 20f;  // Set arrow speed
        }
        else
        {
            rb.velocity = cameraMain.forward * 20f;  // Default arrow direction if no enemy found
        }
    }

    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, currentPosition);
            if (distance < minDistance)
            {
                closest = enemy;
                minDistance = distance;
            }
        }

        return closest;
    }
}
