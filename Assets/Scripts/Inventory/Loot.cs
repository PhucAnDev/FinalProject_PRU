using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Loot : MonoBehaviour
{
    [Header("Data")]
    public ItemSO itemSO;              // Item định danh
    public int quantity = 1;           // Số lượng trong 'cục loot'

    [Header("Visuals")]
    public SpriteRenderer sr;          // Icon hiển thị
    public Animator anim;              // (tuỳ chọn) Animation "LootPickup"

    [Header("Pickup Settings")]
    [SerializeField] private float pickupDelay = 0.2f;          // Trễ nhặt sau khi spawn/drop
    [SerializeField] private float destroyDelayAfterPickup = 0.5f;

    public bool canBePickUp = true;    // true => có thể nhặt
    public static event Action<ItemSO, int> OnItemLooted;

    private void Reset()
    {
        // Đảm bảo Collider2D là trigger
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void Awake()
    {
        // Nếu đặt sẵn trong scene (không gọi Initialize), cho phép nhặt ngay
        canBePickUp = true;
    }

    private void OnValidate()
    {
        if (itemSO != null)
            UpdateAppearance();
    }

    /// <summary>
    /// Gọi khi spawn từ code (DropLoot). Tự khóa nhặt 1 lúc để tránh "nhặt dính tay".
    /// </summary>
    public void Initialize(ItemSO itemSO, int quantity)
    {
        this.itemSO = itemSO;
        this.quantity = Mathf.Max(1, quantity);
        UpdateAppearance();

        canBePickUp = false;
        StopAllCoroutines();
        StartCoroutine(EnablePickupAfterDelay());
    }

    private System.Collections.IEnumerator EnablePickupAfterDelay()
    {
        if (pickupDelay > 0f)
            yield return new WaitForSeconds(pickupDelay);
        canBePickUp = true;
    }

    private void UpdateAppearance()
    {
        if (sr != null && itemSO != null)
            sr.sprite = itemSO.icon;

        if (itemSO != null)
            name = $"{itemSO.itemName} x{quantity}";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (!canBePickUp) return;

        if (anim != null)
            anim.Play("LootPickup");

        // Nhặt toàn bộ stack
        OnItemLooted?.Invoke(itemSO, quantity);

        // Hủy object sau khi hiệu ứng chạy xong
        Destroy(gameObject, destroyDelayAfterPickup);
    }

    // Không cần OnTriggerExit2D để bật nhặt nữa (trước đây gây kẹt "không nhặt được")
}
