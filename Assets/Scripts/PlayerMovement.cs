using UnityEngine;
using System.Collections;

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
    private bool isSprinting;
    private bool isRegeneratingStamina = false;
    private bool canSprintAgaian = true;

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

        isSprinting = Input.GetKey(KeyCode.LeftShift) && playerStats.currentStamina > 0 && canSprintAgaian;

        float currentSpeed = isSprinting ? playerStats.movementSpeed * sprintSpeedMultiplier : playerStats.movementSpeed;

        if (isSprinting)
        {
            weaponAnimator.SetBool("isWalking", true);
            playerStats.currentStamina -= Time.deltaTime * 20; // Stamina drains
            if (playerStats.currentStamina < 0)
            {
                playerStats.currentStamina = 0;
                canSprintAgaian = false;
            }
            uiManager.UpdateStaminaBar();

            StopCoroutine(StaminaRegen()); // Stop regen if sprinting
            isRegeneratingStamina = false;
        }
        else if (!isRegeneratingStamina && playerStats.currentStamina < playerStats.maxStamina)
        {
            StartCoroutine(StaminaRegen());
        }

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

    private IEnumerator StaminaRegen()
    {
        isRegeneratingStamina = true;
        yield return new WaitForSeconds(1f);

        while (playerStats.currentStamina < playerStats.maxStamina)
        {
            playerStats.currentStamina += Time.deltaTime * 10;
            if (playerStats.currentStamina > playerStats.maxStamina) playerStats.currentStamina = playerStats.maxStamina;
            if (playerStats.currentStamina > playerStats.maxStamina * 0.25f) canSprintAgaian = true;
            uiManager.UpdateStaminaBar();
            yield return null;
        }

        isRegeneratingStamina = false;
    }
}
