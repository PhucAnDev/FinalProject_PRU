using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class DeliveryDropZone2D : MonoBehaviour
{
    [Header("Định danh điểm nhận (khớp với objective.deliverToLocation)")]
    [SerializeField] private LocationSO thisLocation;

    [Header("Quest liên kết")]
    [SerializeField] private QuestManager questManager;
    [SerializeField] private QuestSO quest;
    [SerializeField] private int objectiveIndex = 0;

    private QuestObjective Objective =>
        (quest != null && objectiveIndex >= 0 && objectiveIndex < quest.objectives.Count)
        ? quest.objectives[objectiveIndex] : null;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;

        var rb = GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.bodyType = RigidbodyType2D.Kinematic; // để trigger hoạt động ổn định
            rb.simulated = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) => TryAbsorbLoot(other);
    private void OnTriggerStay2D(Collider2D other) => TryAbsorbLoot(other); // bắt cả case loot spawn sẵn trong vùng

    private void TryAbsorbLoot(Collider2D other)
    {
        var obj = Objective;
        if (questManager == null || quest == null || obj == null) return;
        if (obj.type != ObjectiveType.DeliverItemToLocation) return;

        // Nếu objective yêu cầu đúng location, check khớp
        if (obj.deliverToLocation != null && thisLocation != obj.deliverToLocation) return;

        var loot = other.GetComponent<Loot>();
        if (loot == null) return;
        if (loot.itemSO != obj.targetItem) return;
        if (loot.quantity <= 0) return;

        int already = questManager.GetCurrentObjectiveAmount(quest, obj);
        int need = Mathf.Max(0, obj.requiredAmount - already);
        if (need <= 0) return; // đã xong

        int take = Mathf.Min(need, loot.quantity);
        int added = questManager.AddDeliveredProgress(quest, obj, take);
        if (added <= 0) return;

        // Ăn loot
        loot.quantity -= added;
        if (loot.quantity <= 0)
        {
            Destroy(loot.gameObject);
        }
        else
        {
            // Cập nhật tên hiển thị còn lại (icon giữ nguyên)
            loot.name = $"{loot.itemSO.itemName} x{loot.quantity}";
        }

        Debug.Log($"[DeliveryZone] Đã nhận {added} x {loot.itemSO.name}. {questManager.GetProgressText(quest, obj)}");
    }
}
