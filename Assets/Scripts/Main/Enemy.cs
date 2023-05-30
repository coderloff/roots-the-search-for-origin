using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Player")]
    private GameObject player;
    private Player playerScript;

    [Header("Movement")]
    private Rigidbody2D rb;
    [SerializeField] BoxCollider2D detectorCollider;

    [Header("Animation")]
    [SerializeField] Animator animator;
    [SerializeField] Animator arenaAnimator;

    [Header("Run")]
    [SerializeField] private float speed = 2f;
    [SerializeField] bool isTouching;

    bool canMove;

    bool isDialogueStarted;
    bool isDieDialogueStarted;

    [Header("Attack")]
    [SerializeField] private float touchAreaWidth = 10f;
    [SerializeField] private float touchAreaHeight = 0.8f;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange = 1.78f;
    [SerializeField] float attackDistance = 1.78f;
    [SerializeField] private float attackTime = 1f;
    [SerializeField] LayerMask whatIsPlayer;
    [SerializeField] bool canAttack = true;
    [SerializeField] bool isAttackingPossible;

    [Header("Hurt")]
    [SerializeField] private float hurtTime = 1f;
    private bool isHurt;

    [Header("Health")]
    [SerializeField] private int health = 4;
    private bool isAlive = true;

    bool isPlayerImmortal;

    [SerializeField] float deathTime = 2f;

    [SerializeField] DialogueTrigger trigger;
    [SerializeField] DialogueTrigger dieTrigger;

    [SerializeField] AudioSource mainAudio;
    [SerializeField] AudioSource eliteFight;
    [SerializeField] AudioSource winAudio;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();
    }

    void Update()
    {
        // isTouching = Physics2D.OverlapCircle(attackPoint.position, attackRange, 0, whatIsPlayer);
        if (isDialogueStarted)
        {
            canMove = DialogueManager.isEnd;
        }
        if (isDieDialogueStarted)
        {
            if (DialogueManager.isEnd)
            {
                DestroyEnemy();
            }
        }

        if (!isAlive || isHurt || !canMove || playerScript.isDeadButWillReborn) { return; }

        isTouching = Physics2D.OverlapBox(transform.position, new Vector2(touchAreaWidth, touchAreaHeight), 0, whatIsPlayer);
        isAttackingPossible = Physics2D.OverlapCircle(attackPoint.position, attackRange, whatIsPlayer);

        if (isAttackingPossible && canAttack && !playerScript.isDeadButWillReborn)
        {
            if (canMove)
            {
                StartCoroutine(Attack());
            }
        }
        if (isTouching && canAttack)
        {
            Run();
        }
        else
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        if (canMove)
        {
            StartCoroutine(StartFight());
        }

        animator.SetBool("isRunning", isRunning());
    }

    void DestroyEnemy()
    {
        arenaAnimator.ResetTrigger("Fight");
        arenaAnimator.SetTrigger("Release");
        Destroy(gameObject);
    }

    void Run()
    {
        if(!canMove || playerScript.isDeadButWillReborn) { return; }
        if (player.transform.position.x - transform.position.x > attackDistance)
        {
            transform.localScale = Vector3.one;
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
        if (player.transform.position.x - transform.position.x < -attackDistance)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        }
    }

    IEnumerator StartFight()
    {
        Destroy(detectorCollider);
        arenaAnimator.SetTrigger("Fight");
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    IEnumerator Attack()
    {
        if(isHurt || !playerScript.isAlive || !canMove || playerScript.isDeadButWillReborn) { yield return null; }
        canAttack = false;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackTime * 0.7f);
        if (!isHurt && Physics2D.OverlapCircle(attackPoint.position, attackRange, whatIsPlayer))
        {
            if(!isPlayerImmortal)
            playerScript.Hurt();
        }
        yield return new WaitForSeconds(attackTime * 0.3f);
        canAttack = true;
    }

    public void Hurt()
    {
        if (isAlive && isPlayerImmortal)
        {
            isHurt = true;
            animator.SetTrigger("Hurt");
            health -= 1;
            if (health <= 0)
            {
                isAlive = false;
                animator.SetBool("isAlive", false);
            }
            StartCoroutine(MakeDamage());
        }
    }

    IEnumerator MakeDamage()
    {
        if (isAlive)
        {
            yield return new WaitForSeconds(hurtTime);
            isHurt = false;
        }
        else
        {
            animator.SetTrigger("Die");
            yield return new WaitForSeconds(deathTime);
            dieTrigger.StartDialogue();
            isDieDialogueStarted = true;
        }
    }

    public void MakePlayerImmortal()
    {
        isPlayerImmortal = true;
        eliteFight.Stop();
        winAudio.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            trigger.StartDialogue();
            isDialogueStarted = true;
            mainAudio.Stop();
            eliteFight.Play();
        }
    }

    bool isRunning()
    {
        return Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(touchAreaWidth, touchAreaHeight, 0f));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(attackDistance, touchAreaHeight, 0f));
    }
}
