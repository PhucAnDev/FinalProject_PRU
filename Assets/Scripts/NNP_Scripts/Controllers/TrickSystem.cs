using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TrickSystem : MonoBehaviour
{
    [Header("References")]
    public BoolVariable IsAlive;
    public ScoreManager scoreManager;

    private Rigidbody2D rb;
    private bool isAirborne = false;
    private bool wasGrounded = true;

    private float startAngle;
    private float totalRotation = 0f;
    private float airTime = 0f;
    private float lastTrickTime = -999f; // chống double hit
    private float groundExitTime = 0f;

    [Header("Config")]
    [Tooltip("Số độ xoay trong một vòng (360°).")]
    public float rotationPerSpin = 360f;
    [Tooltip("Điểm thưởng cho mỗi vòng xoay hoàn chỉnh.")]
    public float pointsPerSpin = 1000f;
    [Tooltip("Độ sai lệch cho phép (ví dụ: 30° vẫn tính 1 vòng).")]
    public float forgivingAngle = 30f;
    [Tooltip("Thời gian tối thiểu trên không để trick được tính.")]
    public float minAirTime = 0.25f;
    [Tooltip("Góc xoay tối thiểu để trick được tính.")]
    public float minRotation = 180f;
    [Tooltip("Thời gian cooldown sau khi tiếp đất để ngăn cộng điểm lặp.")]
    public float trickCooldown = 0.4f;
    [Tooltip("Layer của mặt đất.")]
    public LayerMask groundLayer;
    [Tooltip("Vị trí kiểm tra mặt đất.")]
    public Transform groundCheck;
    [Tooltip("Khoảng cách raycast để phát hiện mặt đất.")]
    public float groundCheckDistance = 0.5f;

    [Header("Sound Settings")]
    public AudioClip trickSound;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 0.2f;
    }

    void Update()
    {
        if (!IsAlive.Value) return;

        bool grounded = IsGrounded();

        // 🟢 Khi vừa rời mặt đất
        if (wasGrounded && !grounded)
        {
            isAirborne = true;
            startAngle = transform.eulerAngles.z;
            totalRotation = 0f;
            airTime = 0f;
            groundExitTime = Time.time;
        }

        // 🟠 Khi đang trên không
        if (isAirborne)
        {
            airTime += Time.deltaTime;

            float currentAngle = transform.eulerAngles.z;
            float delta = Mathf.DeltaAngle(startAngle, currentAngle);
            totalRotation += Mathf.Abs(delta);
            startAngle = currentAngle;
        }

        // 🔵 Khi vừa chạm đất trở lại
        if (!wasGrounded && grounded && isAirborne)
        {
            // chỉ tính trick nếu đã rời đất > minAirTime và đủ xoay
            if (airTime >= minAirTime && totalRotation >= minRotation)
            {
                // chống rung hoặc double-hit khi tiếp đất cùng collider
                if (Time.time - lastTrickTime > trickCooldown && Time.time - groundExitTime > 0.1f)
                {
                    EvaluateTrick();
                    lastTrickTime = Time.time;
                }
            }
            else
            {
                Debug.Log($"⚠️ Không tính trick [AirTime={airTime:F2}s] [Rotation={totalRotation:F1}°]");
            }

            isAirborne = false;
            totalRotation = 0f;
        }

        wasGrounded = grounded;
    }

    private void EvaluateTrick()
    {
        int fullSpins = Mathf.FloorToInt((totalRotation + forgivingAngle) / rotationPerSpin);

        if (fullSpins > 0 && IsAlive.Value)
        {
            float trickScore = fullSpins * pointsPerSpin;
            scoreManager.AddTrickScore(trickScore);

            if (trickSound != null)
                audioSource.PlayOneShot(trickSound);

            Debug.Log($"✅ Trick success! Spins={fullSpins}, Rotation={totalRotation:F1}°, +{trickScore} pts");
        }
        else
        {
            Debug.Log($"ℹ Trick ended: Rotation={totalRotation:F1}°, no full spin.");
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck == null) return false;

        RaycastHit2D hit = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        Debug.DrawRay(
            groundCheck.position,
            Vector2.down * groundCheckDistance,
            hit.collider ? Color.green : Color.red
        );

        return hit.collider != null;
    }
}
