using UnityEngine;

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

    private bool canHeal = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // nếu nhân vật chết thì không hồi
        if (!IsAlive.Value) return;

        // nếu va chạm đúng tag Heart và chưa hết cooldown
        if (other.CompareTag(healTag) && canHeal)
        {
            Heal(other.gameObject);
        }
    }

    private void Heal(GameObject heartObject)
    {
        // nếu mạng < maxLives thì mới cộng
        if (Lives.Value < maxLives)
        {
            Lives.Value++;
            Debug.Log($"{gameObject.name} picked Heart! Lives: {Lives.Value}");
        }
        else
        {
            Debug.Log("Lives are already full!");
        }

        // xóa Heart sau khi nhặt
        Destroy(heartObject);

        // cooldown hồi máu
        StartCoroutine(HealCooldown());
    }

    private System.Collections.IEnumerator HealCooldown()
    {
        canHeal = false;
        yield return new WaitForSeconds(healCooldown);
        canHeal = true;
    }
}
