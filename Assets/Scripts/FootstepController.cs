using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public class FootstepController : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource source;
    public AudioClip[] footstepClips;
    [Range(0f, 1f)] public float volume = 0.7f;
    public Vector2 pitchJitter = new Vector2(0.95f, 1.05f);

    [Header("Movement")]
    public Rigidbody2D rb;
    public float moveThreshold = 0.05f;     // tốc độ tối thiểu để kêu
    public float stepDistance = 0.9f;       // quãng đường giữa 2 tiếng bước

    [Header("Ground Check")]
    public LayerMask groundMask;            // đặt là layer của tilemap/nền
    public float groundRadius = 0.08f;

    private Collider2D col;
    private Vector2 lastPos;
    private float distAccum;

    void Awake()
    {
        if (!source) source = GetComponent<AudioSource>();
        if (!rb) rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        lastPos = rb.position;

        // AudioSource kiểu 2D
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;
    }

    void Update()
    {
        bool grounded = IsGrounded();
        float speed = rb.linearVelocity.magnitude; // Unity 6

        if (grounded && speed > moveThreshold)
        {
            // tích lũy quãng đường đã đi
            distAccum += Vector2.Distance(rb.position, lastPos);
            if (distAccum >= stepDistance)
            {
                PlayFootstep();
                distAccum = 0f;
            }
        }
        else
        {
            // khi dừng lại, giữ một ít để lần sau vào nhịp mượt hơn
            distAccum = Mathf.Min(distAccum, stepDistance * 0.9f);
        }

        lastPos = rb.position;
    }

    bool IsGrounded()
    {
        // kiểm tra chạm nền bằng 1 vòng tròn nhỏ dưới đáy collider
        var b = col.bounds;
        Vector2 feet = new Vector2(b.center.x, b.min.y - 0.02f);
        return Physics2D.OverlapCircle(feet, groundRadius, groundMask);
    }

    void PlayFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;
        var clip = footstepClips[Random.Range(0, footstepClips.Length)];
        source.pitch = Random.Range(pitchJitter.x, pitchJitter.y);
        source.PlayOneShot(clip, volume);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!GetComponent<Collider2D>()) return;
        var b = GetComponent<Collider2D>().bounds;
        Vector2 feet = new Vector2(b.center.x, b.min.y - 0.02f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(feet, groundRadius);
    }
#endif
}
