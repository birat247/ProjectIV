using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentAI : MonoBehaviour
{
    [Header("Opponent Movement")]
    public float movementSpeed = 1f;
    public float rotationSpeed = 10f;

    private CharacterController characterController;
    private Animator animator;

    [Header("Opponent Fight")]
    public float attackCooldown = 0.5f;
    public int attackDamage = 5;

    public string[] attackAnimations =
    {
        "Attack1Animation",
        "Attack2Animation",
        "Attack3Animation",
        "Attack4Animation"
    };

    public float dodgeDistance = 2f;
    public int attackCount = 0;
    public int randomNumber;
    public float attackRadius = 2f;

    [Header("Target")]
    public Transform player;

    // Reference to player script
    private FightingController fightingController;

    public bool isTakingDamage;

    private float lastAttackTime;

    [Header("Effects and Sound")]
    public ParticleSystem attack1Effect;
    public ParticleSystem attack2Effect;
    public ParticleSystem attack3Effect;
    public ParticleSystem attack4Effect;

    void Awake()
    {
        characterController =
            GetComponent<CharacterController>();

        animator =
            GetComponent<Animator>();

        createRandomNumber();

        // Automatically find player
        if (player == null)
        {
            GameObject p =
                GameObject.FindGameObjectWithTag("Player");

            if (p != null)
            {
                player = p.transform;

                fightingController =
                    p.GetComponent<FightingController>();
            }
        }
        else
        {
            fightingController =
                player.GetComponent<FightingController>();
        }
    }

    void Update()
    {
        if (player == null)
            return;

        TrackPlayer();
    }

    void TrackPlayer()
    {
        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        float distance = direction.magnitude;

        // Face player
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
        }

        // Move towards player
        if (distance > attackRadius)
        {
            animator.SetBool("Walking", true);

            characterController.Move(
                direction.normalized *
                movementSpeed *
                Time.deltaTime
            );
        }
        else
        {
            animator.SetBool("Walking", false);

            if (Time.time - lastAttackTime >
                attackCooldown)
            {
                int attackIndex =
                    Random.Range(
                        0,
                        attackAnimations.Length
                    );

                PerformAttack(attackIndex);
            }
        }
    }

    void PerformAttack(int attackIndex)
    {
        animator.Play(
            attackAnimations[attackIndex]
        );

        Debug.Log(
            "Performed attack " +
            (attackIndex + 1) +
            " dealing " +
            attackDamage +
            " damage."
        );

        // Play damage animation on player
        if (fightingController != null)
        {
            fightingController.StartCoroutine(
                fightingController
                .PlayHitDamageAnimation(
                    attackDamage
                )
            );
        }

        lastAttackTime = Time.time;
    }

    void PerformDodgeFront()
    {
        animator.Play(
            "DodgeFrontAnimation"
        );

        Vector3 dodgeDirection =
            -transform.forward *
            dodgeDistance;

        characterController.Move(
            dodgeDirection *
            Time.deltaTime
        );
    }

    void createRandomNumber()
    {
        randomNumber =
            Random.Range(1, 5);
    }

    public IEnumerator PlayHitDamageAnimation(
        int takeDamage)
    {
        yield return new WaitForSeconds(
            0.5f
        );

        animator.Play(
            "HitDamageAnimation"
        );

        // Reduce AI health here if needed
    }

    public void Attack1Effect()
    {
        if (attack1Effect != null)
            attack1Effect.Play();
    }

    public void Attack2Effect()
    {
        if (attack2Effect != null)
            attack2Effect.Play();
    }

    public void Attack3Effect()
    {
        if (attack3Effect != null)
            attack3Effect.Play();
    }

    public void Attack4Effect()
    {
        if (attack4Effect != null)
            attack4Effect.Play();
    }
}