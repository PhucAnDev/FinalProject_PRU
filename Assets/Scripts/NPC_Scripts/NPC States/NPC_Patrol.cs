using System.Collections;
using UnityEngine;

public class NPC_Patrol : MonoBehaviour
{
    private int currentPatrolIndex;
    public Vector2[] patrolPoints;
    private Vector2 target;
    public float speed = 2f;
    public float pauseDuration = 1.5f;
    public bool isPaused;

    private Rigidbody2D rb;
    private Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        StartCoroutine(SetPatrolPoint());
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        Vector2 direction = ((Vector3)target - transform.position).normalized;
        if(direction.x < 0 && transform.localScale.x > 0 || direction.x > 0 && transform.localScale.x < 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        rb.linearVelocity = direction * speed;
        if (Vector2.Distance(transform.position,target) < .1f)
            StartCoroutine(SetPatrolPoint());

    }
    IEnumerator SetPatrolPoint()
    {
        isPaused = true;
        anim.Play("Idle");
        yield return new WaitForSeconds(pauseDuration);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        target = patrolPoints[currentPatrolIndex];
        isPaused = false;
        anim.Play("Walk");
    }
}
