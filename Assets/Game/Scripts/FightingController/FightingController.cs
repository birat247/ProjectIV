using UnityEngine;

public class FightingController : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 1f;
    public float rotationSpeed = 10f;

    private CharacterController characterController;
    private Animator animator;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (characterController == null)
        {
            Debug.LogError("CharacterController component is missing on " + gameObject.name);
        }

        if (animator == null)
        {
            Debug.LogError("Animator component is missing on " + gameObject.name);
        }
    }

    void Update()
    {
        PerformMovement();
    }

    void PerformMovement()
    {
        if (characterController == null || animator == null)
            return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(-verticalInput, 0f, horizontalInput);

        if (movement.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }

        characterController.Move(
            movement.normalized *
            movementSpeed *
            Time.deltaTime
        );
    }
}