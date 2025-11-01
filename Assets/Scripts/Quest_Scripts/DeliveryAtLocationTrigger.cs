using UnityEngine;

public class DeliveryAtLocationTrigger : MonoBehaviour
{
    [Header("Cấu hình")]
    [SerializeField] private LocationSO thisLocation; // Điểm nhận (ScriptableObject)
    [SerializeField] private QuestManager questManager;
    [SerializeField] private QuestSO quest;           // Nhiệm vụ chứa objective deliver
    [SerializeField] private int objectiveIndex = 0;  // Chọn objective trong list
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private void Reset()
    {
        // Nếu có BoxCollider2D thì đặt isTrigger = true cho tiện
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    //private void OnTriggerStay2D(Collider2D other)
    //{
    //    if (!other.CompareTag("Player")) return;
    //    if (quest == null || questManager == null) return;

    //    if (Input.GetKeyDown(interactKey))
    //    {
    //        if (objectiveIndex < 0 || objectiveIndex >= quest.objectives.Count)
    //        {
    //            Debug.LogWarning("ObjectiveIndex không hợp lệ.");
    //            return;
    //        }                       

    //        var obj = quest.objectives[objectiveIndex];                                 
    //        bool delivered = questManager.TryDeliverItemAtLocation(quest, obj, thisLocation);

    //        if (delivered)
    //        {
    //            Debug.Log($"ĐÃ GIAO {obj.requiredAmount} x {obj.targetItem?.name} tại {thisLocation?.name}");
    //        }
    //        else
    //        {
    //            // Lý do có thể: sai địa điểm, thiếu hàng, type không đúng, ...
    //            Debug.Log("Chưa thể giao: kiểm tra đúng vị trí + đủ số lượng trong túi.");
    //        }
    //    }
    //}
}
