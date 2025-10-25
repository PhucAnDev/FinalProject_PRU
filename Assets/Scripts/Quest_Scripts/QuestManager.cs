using System;
using System.Collections.Generic;
using UnityEngine;

/// Trạng thái tổng thể của 1 Quest (tính từ dữ liệu hiện có)
public enum QuestState
{
    Inactive,   // chưa nhận
    Active,     // đã nhận nhưng chưa đủ
    Completed,  // đã đủ yêu cầu (có thể "giao")
    Delivered   // đã giao xong
}

public class QuestManager : MonoBehaviour
{
    // --- Core data ---
    private readonly Dictionary<QuestSO, Dictionary<QuestObjective, int>> questProgress = new();
    private readonly HashSet<QuestSO> activeQuests = new();
    private readonly HashSet<QuestSO> turnedInQuests = new();   // = Delivered
    private readonly HashSet<QuestSO> completedQuests = new();  // để bắn event Completed 1 lần

    // --- Events (UI/NPC có thể lắng nghe) ---
    public static event Action<QuestSO> OnQuestAccepted;
    public static event Action<QuestSO> OnQuestDeclined;
    public static event Action<QuestSO, QuestObjective, int, int> OnObjectiveProgress;
    public static event Action<QuestSO> OnQuestCompleted;   // fired 1 lần khi vừa Completed
    public static event Action<QuestSO> OnQuestDelivered;   // fired khi chuyển sang Delivered
    public static event Action<QuestSO, QuestState> OnQuestStateChanged;

    // ================== API chính ==================

    /// Bấm "Accept" -> nếu Inactive thì nhận; nếu Completed thì giao (Delivered).
    /// Trả về true nếu có thay đổi trạng thái.
    public bool AcceptOrDeliver(QuestSO q)
    {
        if (q == null) return false;

        var state = GetQuestState(q);
        switch (state)
        {
            case QuestState.Inactive:
                return AcceptQuest(q);

            case QuestState.Completed:
                return DeliverQuest(q);

            default:
                return false; // Active chưa đủ / Delivered rồi -> không đổi
        }
    }

    public bool AcceptQuest(QuestSO q)
    {
        if (q == null) return false;
        bool added = activeQuests.Add(q);
        if (!added) return false;

        if (!questProgress.ContainsKey(q))
            questProgress[q] = new Dictionary<QuestObjective, int>();
        foreach (var o in q.objectives)
            if (!questProgress[q].ContainsKey(o)) questProgress[q][o] = 0;

        completedQuests.Remove(q);
        turnedInQuests.Remove(q);

        OnQuestAccepted?.Invoke(q);
        FireStateChanged(q);
        Debug.Log($"[QuestManager] Accepted: {q.questName}");
        return true;
    }

    public void DeclineQuest(QuestSO q)
    {
        if (q == null) return;
        OnQuestDeclined?.Invoke(q);
        Debug.Log($"[QuestManager] Declined: {q.questName}");
    }

    /// Giao quest (đổi sang Delivered). Chỉ hợp lệ khi đang Completed.
    public bool DeliverQuest(QuestSO q)
    {
        if (q == null || !IsQuestCompleted(q)) return false;
        if (turnedInQuests.Contains(q)) return false;

        turnedInQuests.Add(q);
        OnQuestDelivered?.Invoke(q);
        FireStateChanged(q);
        Debug.Log($"[QuestManager] Delivered: {q.questName}");
        return true;
    }

    public QuestState GetQuestState(QuestSO q)
    {
        if (q == null) return QuestState.Inactive;
        if (IsQuestTurnedIn(q)) return QuestState.Delivered;
        if (IsQuestCompleted(q)) return QuestState.Completed;
        if (IsQuestActive(q)) return QuestState.Active;
        return QuestState.Inactive;
    }

    public bool IsQuestActive(QuestSO q) => q && activeQuests.Contains(q);
    public bool IsQuestTurnedIn(QuestSO q) => q && turnedInQuests.Contains(q);

    public bool IsQuestCompleted(QuestSO q)
    {
        if (q == null || q.objectives == null || q.objectives.Count == 0) return false;
        foreach (var o in q.objectives)
        {
            int req = Mathf.Max(1, o.requiredAmount);
            if (GetCurrentObjectiveAmount(q, o) < req) return false;
        }
        return true;
    }

    // ================== Cập nhật tiến độ ==================

    public void UpdateObjectiveProgress(QuestSO questSO, QuestObjective objective)
    {
        if (questSO == null || objective == null) return;

        if (!questProgress.ContainsKey(questSO))
            questProgress[questSO] = new Dictionary<QuestObjective, int>();

        var dict = questProgress[questSO];
        int newAmount = 0;

        switch (objective.type)
        {
            case ObjectiveType.CollectItem:
                if (objective.targetItem != null && InventoryManagemant.Instance != null)
                    newAmount = InventoryManagemant.Instance.GetItemQuantity(objective.targetItem);
                break;

            case ObjectiveType.GoToLocation:
                if (objective.targetLocation != null
                    && LocationHistoryTracker.Instance != null
                    && LocationHistoryTracker.Instance.HasVisited(objective.targetLocation))
                    newAmount = objective.requiredAmount;
                break;

            case ObjectiveType.TalkToNPC:
                // nếu có tracker -> newAmount = objective.requiredAmount khi đã nói chuyện
                break;

            case ObjectiveType.DeliverItemToLocation:
                if (dict.TryGetValue(objective, out int delivered)) newAmount = delivered;
                break;
        }

        newAmount = Mathf.Clamp(newAmount, 0, Mathf.Max(1, objective.requiredAmount));
        dict[objective] = newAmount;

        OnObjectiveProgress?.Invoke(questSO, objective, newAmount, objective.requiredAmount);
        TryFireCompleted(questSO);
    }

    public int AddDeliveredProgress(QuestSO questSO, QuestObjective objective, int amount)
    {
        if (questSO == null || objective == null) return 0;

        if (!questProgress.ContainsKey(questSO))
            questProgress[questSO] = new Dictionary<QuestObjective, int>();

        var dict = questProgress[questSO];
        dict.TryGetValue(objective, out int current);

        int target = Mathf.Max(1, objective.requiredAmount);
        int canAdd = Mathf.Clamp(amount, 0, target - current);
        if (canAdd <= 0) return 0;

        int newVal = current + canAdd;
        dict[objective] = newVal;

        OnObjectiveProgress?.Invoke(questSO, objective, newVal, objective.requiredAmount);
        TryFireCompleted(questSO);
        return canAdd;
    }

    private void TryFireCompleted(QuestSO questSO)
    {
        if (questSO == null) return;
        if (!completedQuests.Contains(questSO) && IsQuestCompleted(questSO))
        {
            completedQuests.Add(questSO);
            OnQuestCompleted?.Invoke(questSO);
            FireStateChanged(questSO); // đổi từ Active -> Completed
            Debug.Log($"[QuestManager] Completed: {questSO.questName}");
        }
    }

    private void FireStateChanged(QuestSO q)
        => OnQuestStateChanged?.Invoke(q, GetQuestState(q));

    // ================== Helpers cho UI ==================

    public string GetProgressText(QuestSO questSO, QuestObjective objective)
    {
        int current = GetCurrentObjectiveAmount(questSO, objective);
        if (current >= Mathf.Max(1, objective.requiredAmount)) return "Completed";

        return objective.type switch
        {
            ObjectiveType.CollectItem => $"{InventoryManagemant.Instance?.GetItemQuantity(objective.targetItem) ?? 0}/{objective.requiredAmount}",
            ObjectiveType.DeliverItemToLocation => $"Delivered {current}/{objective.requiredAmount}",
            _ => "In Progress"
        };
    }

    public int GetCurrentObjectiveAmount(QuestSO questSO, QuestObjective objective)
    {
        if (questSO != null &&
            questProgress.TryGetValue(questSO, out var dict) &&
            dict.TryGetValue(objective, out int amount))
            return amount;
        return 0;
    }

    /// Tiến độ trung bình toàn quest (0..1)
    public float GetProgressPercent(QuestSO q)
    {
        if (q == null || q.objectives == null || q.objectives.Count == 0) return 0f;
        float sum = 0f;
        foreach (var o in q.objectives)
        {
            int cur = GetCurrentObjectiveAmount(q, o);
            int req = Mathf.Max(1, o.requiredAmount);
            sum += Mathf.Clamp01(req == 0 ? 1f : (float)cur / req);
        }
        return sum / q.objectives.Count;
    }

    /// Cập nhật nhanh mọi quest đang Active (gọi khi mở UI)
    public void UpdateAllActiveProgress()
    {
        foreach (var q in activeQuests)
            foreach (var o in q.objectives)
                UpdateObjectiveProgress(q, o);
    }

    /// Reset tiến độ một quest (nếu chơi lại)
    public void ResetQuestProgress(QuestSO q)
    {
        if (q == null || !questProgress.ContainsKey(q)) return;
        foreach (var o in q.objectives) questProgress[q][o] = 0;
        completedQuests.Remove(q);
        turnedInQuests.Remove(q);
        FireStateChanged(q);
    }

    // ================== Endgame check ==================

    /// True nếu TẤT CẢ quest trong danh sách đều Delivered
    public bool AreAllDelivered(ICollection<QuestSO> allQuests)
    {
        if (allQuests == null || allQuests.Count == 0) return false;
        foreach (var q in allQuests)
            if (!IsQuestTurnedIn(q)) return false;
        return true;
    }

    /// True nếu TẤT CẢ quest đang Active đều Delivered (tiện khi bạn chỉ theo dõi active list)
    public bool AreAllActiveDelivered()
    {
        foreach (var q in activeQuests)
            if (!IsQuestTurnedIn(q)) return false;
        return activeQuests.Count > 0; // có ít nhất 1 quest và tất cả đã Delivered
    }
}
