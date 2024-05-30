using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkPlayerMovement : NetworkBehaviour
{
  // private GameObject arrow;
  public GameObject kickSlash;
  public GameObject InputUI;
    float arrowDamage;
    int arrowRange;
    private Player playerInput;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private Transform child;
    private bool groundedPlayer;
    private Animator animator;
    private bool isAiming;
    private bool isAimWalking;
    private bool isKicking;  // Track if kick animation is playing

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

    public bool rangedSpecialAttack;

    [SerializeField]
    private Transform arrowSpawnPoint;  // Position to instantiate the arrow
    [SerializeField]
    private Transform arrowReleasePoint;

    [SerializeField]
    private cardScript LongcardScript; // Reference to cardScript
    [SerializeField]
    private cardScript ShortcardScript; // Reference to cardScript

    [SerializeField]
    Transform slashSpawnPoint;
    private bool isOwner;

    public override void OnNetworkSpawn()
    {
        
        if(GetComponent<NetworkObject>().IsOwner)
        {
            InputUI.SetActive(true);
            Debug.Log(IsOwner);
            isOwner = true;
            
        playerInput.PlayerMain.Aim.started += OnAimStarted;
        playerInput.PlayerMain.Shoot.started += OnShootStarted;
        playerInput.PlayerMain.Kick.started += OnKickStarted;
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
        playerInput.PlayerMain.Aim.started -= OnAimStarted;
        playerInput.PlayerMain.Shoot.started -= OnShootStarted;
        playerInput.PlayerMain.Kick.started -= OnKickStarted;
        }
    }
    private void Awake()
    {
        playerInput = new Player();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        rangedSpecialAttack = false;

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
        if(!GetComponent<NetworkObject>().IsOwner) {return;}
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            animator.SetBool("isJumping", false);  // Ensure isJumping is false when grounded
        }

        Vector2 movementInput = playerInput.PlayerMain.Move.ReadValue<Vector2>();

        Vector3 move = cameraMain.forward * movementInput.y + cameraMain.right * movementInput.x;
        move.y = 0f;

        if (isAiming)
        {
            move *= 0.5f;  // Reduce speed when aiming
        }

        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;

            if (isAiming)
            {
                isAimWalking = true;
                animator.SetBool("isAimWalking", true);
            }
        }
        else
        {
            if (isAiming)
            {
                isAimWalking = false;
                animator.SetBool("isAimWalking", false);
            }
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
        isAiming = true;
        //arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation, arrowSpawnPoint);
        //StartBulletServerRpc(NetworkManager.Singleton.LocalClientId);
        StartCoroutine(HandleAimingSequence());
    }

    private void OnShootStarted(InputAction.CallbackContext context)
    {
        if (isAiming)
        {
            StartCoroutine(HandleShooting());
        }
    }

    private IEnumerator HandleAimingSequence()
    {
        // Set isDrawingArrow to true and wait for the animation duration
        animator.SetBool("isDrawingArrow", true);
        yield return new WaitForSeconds(0.35f); // Adjust to the duration of the drawing arrow animation

        // Set isDrawingArrow to false and isAiming to true
        animator.SetBool("isAiming", true);
        animator.SetBool("isDrawingArrow", false);
    }
    

    private IEnumerator HandleShooting()
    {
        ShootArrow();
        // Set isShooting to true
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("isShooting", true);

        // Shoot the arrow
        

        // Wait for the shooting animation duration
        yield return new WaitForSeconds(0.25f); // Adjust to the duration of the shooting animation
        animator.SetBool("isAiming", false);

        // Set isShooting to false and isIdle to true
        animator.SetBool("isShooting", false);
        animator.SetBool("isIdle", true);

        // Reset aiming state
        isAiming = false;
        isAimWalking = false;
        animator.SetBool("isAimWalking", false);
    }

    private void ShootArrow()
    {
        StartBulletServerRpc(NetworkManager.Singleton.LocalClientId);
        //ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
        //arrowScript.Shot(arrowReleasePoint.position - arrowSpawnPoint.position);

        
        //Rigidbody rb = arrow.GetComponent<Rigidbody>();
        //rb.velocity = transform.forward * 20f;  // Set arrow speed in the direction the player is facing

        // You can add additional logic here to adjust the arrow's direction if needed
    }
    [ServerRpc(RequireOwnership = false)]
    void StartBulletServerRpc(ulong clientID)
    {
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation, arrowSpawnPoint);
        NetworkObject arrowNetworkObject = arrow.GetComponent<NetworkObject>();
        
        arrowNetworkObject.Spawn();
        arrow.GetComponent<ArrowScript>().SetOwnershipServerRpc(clientID);
        ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
        arrowScript.Shot(arrowReleasePoint.position - arrowSpawnPoint.position);
        if(rangedSpecialAttack){
            arrowScript.rangedSpecialAttack = true;
            arrowScript.damage = arrowDamage;
            arrowScript.range = arrowRange;
        }
        
    }

    private void OnKickStarted(InputAction.CallbackContext context)
    {
        if (!isKicking)
        {
            isKicking = true;
            StartCoroutine(HandleKicking());
        }
    }

    private IEnumerator HandleKicking()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isKicking", true);

        // Wait for the kick animation duration
        yield return new WaitForSeconds(0.8f);

        animator.SetBool("isKicking", false);
        animator.SetBool("isIdle", true);

        isKicking = false;  // Reset isKicking state
        ShortcardScript.useCard(); // Call useCard() method
    }
    public void slashAttack(GameObject slash, float damageRadius, int damage)
    {
        
        
            StartSlashServerRpc();
        
        
    }
    [ServerRpc]
    void StartSlashServerRpc()
    {
        Debug.Log("slash Used");
        GameObject myKickSlash = Instantiate(kickSlash, slashSpawnPoint.position, transform.rotation);
        myKickSlash.GetComponent<NetworkObject>().Spawn();
    }

    public void rangedAttack(float speed, int damage, int range)
    {
        Debug.Log("rangeed special attack Used");
        arrowDamage = damage;
        arrowRange = range;
        if(isOwner){
            StartSlashServerRpc();
        }
        
        // Instantiate(slash, slashSpawnPoint.position, transform.rotation);
    }
}

/*
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
*/

