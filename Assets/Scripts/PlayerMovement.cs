using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerStats playerStats;
    public PlayerUIManager uiManager;
    public Animator weaponAnimator; 

    public float sprintSpeedMultiplier = 1.5f;
    public float rotationSpeed = 250f;
    public float gravity = -9.81f;
    public float jumpHeight = 2.5f;

    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isWalking;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float moveForward = Input.GetAxis("Vertical");
        float moveRight = Input.GetAxis("Horizontal");

        bool isMoving = (moveForward != 0 || moveRight != 0);

        if (isMoving && !isWalking)
        {
            weaponAnimator.SetBool("isWalking", true);
            isWalking = true;
        }
        else if (!isMoving && isWalking)
        {
            weaponAnimator.SetBool("isWalking", false);
            isWalking = false;
        }

        float currentSpeed = playerStats.movementSpeed;
        Vector3 move = (transform.forward * moveForward + transform.right * moveRight).normalized;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        float rotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
