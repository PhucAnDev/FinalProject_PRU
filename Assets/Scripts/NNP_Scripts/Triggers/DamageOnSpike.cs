using UnityEngine;

public class DamageOnSpike : MonoBehaviour
{
    [Header("References")]
    public BoolVariable IsAlive;
    public IntVariable Lives;

    [Tooltip("Tag của vật gây sát thương (ví dụ Spike)")]
    public string damageTag = "Spike";

    [Tooltip("Thời gian miễn sát thương sau khi bị trúng (giây)")]
    public float invincibilityDuration = 0.75f;

    private bool canTakeDamage = true;

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
        // 🛡️ Double check (đề phòng gọi trực tiếp)
        if (PlayerMovement.IsCheatModeActive)
        {
            Debug.Log("🛡️ Cheat active - ignoring TakeDamage()");
            return;
        }

        Lives.Value--;
        Debug.Log($"{gameObject.name} hit Spike! Remaining lives: {Lives.Value}");

        if (Lives.Value <= 0)
        {
            Lives.Value = 0;
            IsAlive.Value = false;
        }
        StartCoroutine(InvincibilityCooldown());
    }

    private System.Collections.IEnumerator InvincibilityCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(invincibilityDuration);
        canTakeDamage = true;
    }
}
