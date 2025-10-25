using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestSO", menuName = "QuestSO")]
public class QuestSO : ScriptableObject
{
    public string questName;

    [TextArea] public string questDescription; // vẫn giữ để không vỡ asset cũ
    [TextArea] public string header;           // NEW: mô tả ngắn hiển thị ở trên (nếu để trống sẽ fallback)

    public int questLever1;

    public List<QuestObjective> objectives = new();

    // Tiện ích: lấy text header không bị trống
    public string GetHeaderText()
    {
        if (!string.IsNullOrWhiteSpace(header))
            return header;

        if (!string.IsNullOrWhiteSpace(questDescription))
            return questDescription;

        var prim = objectives?.Find(o => o.useAsHeader);
        if (prim != null && !string.IsNullOrWhiteSpace(prim.description))
            return prim.description;

        return questName; // fallback cuối
    }

    // Danh sách objective để hiển thị (loại bỏ cái được dùng làm header)
    public IEnumerable<QuestObjective> GetDisplayObjectives()
    {
        if (objectives == null) yield break;
        foreach (var o in objectives)
            if (!o.useAsHeader) yield return o;
    }
}

[System.Serializable]
public class QuestObjective
{
    public ObjectiveType type;

    [TextArea] public string description;

    [Tooltip("Đánh dấu objective này làm tiêu đề mô tả nhiệm vụ và KHÔNG lặp lại bên dưới.")]
    public bool useAsHeader = false; // NEW

    [SerializeField] private Object target; // ItemSO / NPC / LocationSO
    public ItemSO targetItem => target as ItemSO;
    public NPC targetNPC => target as NPC;
    public LocationSO targetLocation => target as LocationSO;

    [Header("Chỉ dùng cho DeliverItemToLocation")]
    public LocationSO deliverToLocation;

    public int requiredAmount = 1;
}

public enum ObjectiveType
{
    CollectItem,
    GoToLocation,
    TalkToNPC,
    DeliverItemToLocation
}