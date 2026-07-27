using System.Collections;
using UnityEngine;

public class FightingController : MonoBehaviour
{
    [Header("Player Movement")]
    public float movementSpeed = 1f;
    public float rotationSpeed = 10f;

    private CharacterController characterController;
    private Animator animator;

    [Header("Player Fight")]
    public float attackCooldown = 0.5f;
    public int attackDamages = 5;

    public string[] attackAnimations =
    {
        "Attack1Animation",
        "Attack2Animation",
        "Attack3Animation",
        "Attack4Animation"
    };

    [Header("Player Dodge")]
    public float dodgeDistance = 2f;

    public float attackRadius = 2.2f;
    public Transform[] opponents;

    private float lastAttackTime;

    [Header("Effects and Sound")]
    public ParticleSystem attack1Effect;
    public ParticleSystem attack2Effect;
    public ParticleSystem attack3Effect;
    public ParticleSystem attack4Effect;

    public AudioClip[] hitSounds;

    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    void Awake()
    {
        currentHealth = maxHealth;
        healthBar.GiveFullHealth(currentHealth);

        characterController =
            GetComponent<CharacterController>();

        animator =
            GetComponent<Animator>();

        if (characterController == null)
        {
            Debug.LogError(
                "CharacterController missing on "
                + gameObject.name);
        }

        if (animator == null)
        {
            Debug.LogError(
                "Animator missing on "
                + gameObject.name);
        }
    }

    void Update()
    {
        if (characterController == null ||
            animator == null)
            return;

        PerformMovement();
        PerformDodgeFront();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PerformAttack(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PerformAttack(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PerformAttack(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PerformAttack(3);
        }
    }

    void PerformMovement()
    {
        float horizontalInput =
            Input.GetAxis("Horizontal");

        float verticalInput =
            Input.GetAxis("Vertical");

        Vector3 movement =
            new Vector3(
                -verticalInput,
                0f,
                horizontalInput);

        if (movement != Vector3.zero)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(
                    movement
                );

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed *
                    Time.deltaTime
                );

            animator.SetBool(
                "Walking",
                true
            );
        }
        else
        {
            animator.SetBool(
                "Walking",
                false
            );
        }

        characterController.Move(
            movement *
            movementSpeed *
            Time.deltaTime
        );
    }

    void PerformAttack(
        int attackIndex
    )
    {
        if (attackIndex < 0 ||
            attackIndex >=
            attackAnimations.Length)
            return;

        if (Time.time -
            lastAttackTime >
            attackCooldown)
        {
            animator.Play(
                attackAnimations[
                    attackIndex
                ]
            );

            Debug.Log(
                "Performed attack "
                + (attackIndex + 1)
                + " dealing "
                + attackDamages
                + " damage"
            );

            lastAttackTime =
                Time.time;

            // Loop through all opponents
            foreach (
                Transform opponent
                in opponents
            )
            {
                if (opponent == null)
                    continue;

                float distance =
                    Vector3.Distance(
                        transform.position,
                        opponent.position
                    );

                if (distance <=
                    attackRadius)
                {
                    OpponentAI
                        opponentAI =
                        opponent
                        .GetComponent
                        <OpponentAI>();

                    if (
                        opponentAI !=
                        null
                    )
                    {
                        opponentAI
                        .StartCoroutine(
                            opponentAI
                            .PlayHitDamageAnimation(
                                attackDamages
                            )
                        );
                    }
                }
            }
        }
        else
        {
            Debug.Log(
                "Cannot perform attack yet. Cooldown active."
            );
        }
    }

    void PerformDodgeFront()
    {
        if (Input.GetKeyDown(
                KeyCode.E))
        {
            animator.Play(
                "DodgeFrontAnimation"
            );

            Vector3
                dodgeDirection =
                transform.forward *
                dodgeDistance;

            characterController.Move(
                dodgeDirection
            );
        }
    }

    public IEnumerator PlayHitDamageAnimation(int takeDamage)
    {
      yield return new WaitForSeconds(0.5f);

        // Play hit sound here
        if (hitSounds != null && hitSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, hitSounds.Length);
            AudioSource.PlayClipAtPoint(hitSounds[randomIndex], transform.position);
        }

        // Reduce health here
        currentHealth -= takeDamage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }


        animator.Play("HitDamageAnimation");
    }

    // Handle player death
    void Die()
    {
        Debug.Log("Player has died.");
        // Implement death logic here (e.g., respawn, game over screen)
    }

    // Animation Events
    public void
        Attack1Effect()
    {
        if (
            attack1Effect !=
            null
        )
        {
            attack1Effect.Play();
        }
    }

    public void
        Attack2Effect()
    {
        if (
            attack2Effect !=
            null
        )
        {
            attack2Effect.Play();
        }
    }

    public void
        Attack3Effect()
    {
        if (
            attack3Effect !=
            null
        )
        {
            attack3Effect.Play();
        }
    }

    public void
        Attack4Effect()
    {
        if (
            attack4Effect !=
            null
        )
        {
            attack4Effect.Play();
        }
    }
}