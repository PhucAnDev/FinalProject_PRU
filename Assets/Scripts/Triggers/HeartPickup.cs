using UnityEngine;
using System.Collections;

public class HeartPickup : MonoBehaviour
{
    [Header("References")]
    public BoolVariable IsAlive;
    public IntVariable Lives;

    [Tooltip("Tag của vật hồi máu (ví dụ Heart)")]
    public string healTag = "Heart";

    [Tooltip("Số mạng tối đa có thể có")]
    public int maxLives = 3;

    [Tooltip("Thời gian chờ giữa các lần hồi (giây)")]
    public float healCooldown = 1f;

    [Header("Sound Settings")]
    [Tooltip("Âm thanh phát ra khi nhặt Heart.")]
    public AudioClip healSound;
    private AudioSource audioSource;

    private bool canHeal = true;

    private void Start()
    {
        // 🔊 Chuẩn bị audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;  // 0 = 2D sound
        audioSource.volume = 0.5f;      // chỉnh âm lượng tùy ý
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ❌ Nếu nhân vật chết thì không hồi
        if (!IsAlive.Value) return;

        // ❤️ Nếu va chạm đúng tag Heart và có thể hồi
        if (other.CompareTag(healTag) && canHeal)
        {
            Heal(other.gameObject);
        }
    }

    private void Heal(GameObject heartObject)
    {
        // ❤️ Nếu mạng chưa đầy thì cộng thêm
        if (Lives.Value < maxLives)
        {
            Lives.Value++;
            Debug.Log($"{gameObject.name} picked Heart! Lives: {Lives.Value}");

            // 🔊 Phát âm thanh hồi máu
            if (healSound != null)
                audioSource.PlayOneShot(healSound);
        }
        else
        {
            Debug.Log("Lives are already full!");
        }

        // 🧹 Xóa Heart sau khi nhặt
        Destroy(heartObject);

        // ⏳ Cooldown hồi máu
        StartCoroutine(HealCooldown());
    }

    private IEnumerator HealCooldown()
    {
        canHeal = false;
        yield return new WaitForSeconds(healCooldown);
        canHeal = true;
    }
}
