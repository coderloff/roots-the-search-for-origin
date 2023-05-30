using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    Vector2 moveInput;
    Rigidbody2D rb;

    [Header("Animation")]
    [SerializeField] Animator animator;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Health")]
    [SerializeField] private int health = 10;
    [HideInInspector] public bool isAlive = true;
    public bool isDeadButWillReborn;

    [Header("Run")]
    [SerializeField] float runSpeed = 10f;

    [Header("Jump")]
    [SerializeField] float jumpAmount = 10f;
    [SerializeField] BoxCollider2D feetCollider;
    bool isJumping;

    [Header("Climb")]
    [SerializeField] float climbAmount = 10f;
    [SerializeField] LayerMask whatIsClimbing;
    float startGravity;

    [Header("Attack")]
    [SerializeField] float attackRange = 1f;
    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask whatIsEnemy;
    bool isAttacking;
    bool canAttack = true;

    [Header("Father")]
    [SerializeField] DialogueTrigger fatherTrigger;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        GetComponent<DialogueTrigger>().StartDialogue();
        startGravity = rb.gravityScale;
    }

    void Update()
    {
        if (!isAlive) { return; }
        Run();
        if(!isAttacking) FlipSprite();
        if (isJumping) { CheckJump(); }
        Climb();
        if (isDeadButWillReborn)
        {
            WaitForDialogue();
        }
    }

    void WaitForDialogue()
    {
        if (DialogueManager.isEnd)
        {
            FindObjectOfType<Enemy>().MakePlayerImmortal();
            animator.SetTrigger("Reborn");
            isDeadButWillReborn = false;
        }
    }

    void OnMove(InputValue value)
    {
        if(!isAlive || DialogueManager.isActive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void Run()
    {
        if(DialogueManager.isActive)
        {
            moveInput = Vector2.zero;
            rb.velocity = Vector2.zero;
            animator.SetBool("isRunning", false);
            return;
        }
        Vector2 playerMovement = new Vector2(moveInput.x * runSpeed, rb.velocity.y);
        if(!isAttacking) rb.velocity = playerMovement;

        animator.SetBool("isRunning", isRunning());
    }
    void FlipSprite()
    {
        if (isRunning())
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
        }
    }

    void OnJump(InputValue value)
    {
        if (!isAlive || !isGrounded() || DialogueManager.isActive) { return; }
        if (value.isPressed)
        {
            rb.velocity += new Vector2(0f, jumpAmount);
            animator.SetBool("isJumping", true);
            StartCoroutine(WaitForJump());
        }
    }

    IEnumerator WaitForJump()
    {
        yield return new WaitForSeconds(0.1f);
        isJumping = true;
    }

    void CheckJump()
    {
        if (isGrounded())
        {
            isJumping = false;
            animator.SetBool("isJumping", isJumping);
        }
    }

    void Climb()
    {
        if (!feetCollider.IsTouchingLayers(whatIsClimbing))
        {
            rb.gravityScale = startGravity;
            animator.SetBool("isClimbing", false);
            return;
        }
        Vector2 climbVelocity = new Vector2(rb.velocity.x, moveInput.y * climbAmount);
        rb.velocity = climbVelocity;
        rb.gravityScale = 0;
        bool playerHasVerticalSpeed = Mathf.Abs(moveInput.y) > Mathf.Epsilon;
        animator.SetBool("isClimbing", playerHasVerticalSpeed);
        if (feetCollider.IsTouchingLayers(whatIsClimbing))
        {
            isJumping = false;
        }
    }

    void OnAttack(InputValue value)
    {
        if(!isAlive || DialogueManager.isActive || isDeadButWillReborn) { return; }
        if (value.isPressed && canAttack)
        {
            animator.SetTrigger("Attack");

            Collider2D[] hittedEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, whatIsEnemy);
            foreach(Collider2D enemy in hittedEnemies)
            {
                Debug.Log("Yeahh man: " + enemy.name);
                enemy.GetComponent<Enemy>().Hurt();
            }
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        canAttack = false;
        yield return new WaitForSeconds(0.3f);
        canAttack = true;
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    public void Hurt()
    {
        if (!isAlive) { return; }

        health -= 1;
        MakeDamage();
        if (health <= 0)
        {
            DieAndReborn();
        }
    }

    void MakeDamage()
    {
        if (isAlive)
        {
            animator.SetTrigger("Hurt");
        }
    }
    
    void DieAndReborn()
    {
        isDeadButWillReborn = true;
        animator.SetTrigger("Die");
        fatherTrigger.StartDialogue();
    }

    bool isGrounded()
    {
        return feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }

    bool isRunning()
    {
        if(DialogueManager.isActive) { return false; }
        return Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
    }
}
