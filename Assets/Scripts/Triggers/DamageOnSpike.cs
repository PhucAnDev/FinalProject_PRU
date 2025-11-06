using UnityEngine;
using System.Collections;

public class DamageOnSpike : MonoBehaviour
{
    [Header("References")]
    public BoolVariable IsAlive;
    public IntVariable Lives;

    [Tooltip("Tag của vật gây sát thương (ví dụ Spike)")]
    public string damageTag = "Spike";

    [Tooltip("Thời gian miễn sát thương sau khi bị trúng (giây)")]
    public float invincibilityDuration = 0.75f;

    [Header("Sound Settings")]
    [Tooltip("Âm thanh phát ra khi máu còn 2 (đau nhẹ).")]
    public AudioClip damageSound;

    [Tooltip("Âm thanh phát ra khi máu còn 1 (nguy hiểm).")]
    public AudioClip lowHealthSound;

    private AudioSource audioSource;
    private bool canTakeDamage = true;

    private void Start()
    {
        // 🔊 Chuẩn bị audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;   // 0 = 2D sound
        audioSource.volume = 0.4f;       // chỉnh tùy tai nghe hoặc game
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 🛡️ Nếu cheat đang bật → không nhận sát thương
        if (PlayerMovement.IsCheatModeActive)
        {
            Debug.Log("🛡️ Cheat active - no spike damage!");
            return;
        }

        if (!IsAlive.Value) return;

        if (other.CompareTag(damageTag) && canTakeDamage)
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        if (PlayerMovement.IsCheatModeActive)
        {
            Debug.Log("🛡️ Cheat active - ignoring TakeDamage()");
            return;
        }

        Lives.Value--;
        Debug.Log($"{gameObject.name} hit Spike! Remaining lives: {Lives.Value}");

        // 🔊 Phát âm thanh theo lượng máu còn lại
        if (Lives.Value == 2 && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
            Debug.Log("💢 Damage sound (Lives=2) played!");
        }
        else if (Lives.Value == 1 && lowHealthSound != null)
        {
            audioSource.PlayOneShot(lowHealthSound);
            Debug.Log("⚠️ Low health sound (Lives=1) played!");
        }

        // 💀 Nếu hết máu → chết
        if (Lives.Value <= 0)
        {
            Lives.Value = 0;
            IsAlive.Value = false;
        }

        StartCoroutine(InvincibilityCooldown());
    }

    private IEnumerator InvincibilityCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(invincibilityDuration);
        canTakeDamage = true;
    }
}
