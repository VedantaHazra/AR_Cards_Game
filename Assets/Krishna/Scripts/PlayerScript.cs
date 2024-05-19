using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    //Essentials
    public Transform cam;
    CharacterController controller;
    float turnSmoothTime = .1f;
    float turnSmoothvelocity;
    Animator anim;

    //Movement
    Vector2 movement;
    public float walkSpeed;
    public float sprintSpeed;
    bool sprinting;
    float trueSpeed;

    //Jumping
    public float jumpHeight;
    public float gravity;
    bool isGrounded;
    Vector3 velocity;

    void Start()
    {
        trueSpeed = walkSpeed;
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        anim = GetComponentInChildren<Animator>();
    }
    void Update()
    {

        isGrounded = Physics.CheckSphere(transform.position, .1f, 1);
        anim.SetBool("IsGrounded", isGrounded);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -1f;
        }



        if (Input.GetKey(KeyCode.LeftShift))
        {
            trueSpeed = sprintSpeed;
            sprinting = true;

        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            trueSpeed = walkSpeed;
            sprinting = false;
        }
        anim.transform.localPosition = Vector3.zero;
        anim.transform.localEulerAngles = Vector3.zero;

        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 direction = new Vector3(movement.x, 0, movement.y).normalized;


        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothvelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * Time.deltaTime * trueSpeed);

            if (sprinting)
            {
                anim.SetFloat("Speed", 2);
            }
            else
            {
                anim.SetFloat("Speed", 1);
            }
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }


        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt((jumpHeight * 10) * -2f * gravity);
        }

        if (velocity.y > -20)
        {
            velocity.y += gravity * 10 * Time.deltaTime;
        }

        // velocity.y += (gravity * 10) * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
}
