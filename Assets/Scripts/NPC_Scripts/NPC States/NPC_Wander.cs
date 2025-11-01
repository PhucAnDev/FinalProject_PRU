using UnityEngine;
using System.Collections;

public class NPC_Wander : MonoBehaviour
{
    [Header("Wander Area")]
    public float wanderWidth = 5f;
    public float wanderHeight = 5f;
    public Vector2 startingPosition;

    public float speed;
    private Vector2 target;

    public float pauseDuration = 1f;
    private bool isPaused = false;

    private Rigidbody2D rb;
    private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        StartCoroutine(PauseAndPickNewDestination());
    }

    private void Update()
    {
        if (isPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            StartCoroutine(PauseAndPickNewDestination());
        }

        Move();
    }

    private void Move()
    {
        Vector2 direction = target - (Vector2)transform.position;
        direction.Normalize();

        rb.linearVelocity = direction * speed;

        if ((direction.x > 0 && transform.localScale.x < 0) || (direction.x < 0 && transform.localScale.x > 0))
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    private IEnumerator PauseAndPickNewDestination()
    {
        isPaused = true;
        anim.Play("Idle");

        yield return new WaitForSeconds(pauseDuration);

        target = GetRandomTarget();

        isPaused = false;
        anim.Play("Walk");
    }

    private Vector2 GetRandomTarget()
    {
        float halfWidth = wanderWidth / 2;
        float halfHeight = wanderHeight / 2;

        int edge = Random.Range(0, 4);

        switch (edge)
        {
            case 0:
                return new Vector2(
                    startingPosition.x - halfWidth,
                    Random.Range(startingPosition.y - halfHeight, startingPosition.y + halfHeight)
                );
            case 1:
                return new Vector2(
                    startingPosition.x + halfWidth,
                    Random.Range(startingPosition.y - halfHeight, startingPosition.y + halfHeight)
                );
            case 2:
                return new Vector2(
                    Random.Range(startingPosition.x - halfWidth, startingPosition.x + halfWidth),
                    startingPosition.y - halfHeight
                );
            default:
                return new Vector2(
                    Random.Range(startingPosition.x - halfWidth, startingPosition.x + halfWidth),
                    startingPosition.y + halfHeight
                );
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(PauseAndPickNewDestination());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(startingPosition, new Vector3(wanderWidth, wanderHeight, 0));
    }
}