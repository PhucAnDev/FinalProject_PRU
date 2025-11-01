using System.Text;
using TMPro;
using UnityEngine;

public class QuestLogUI : MonoBehaviour
{
    private enum PanelMode { None, Preview, Inspect }

    [Header("Refs")]
    [SerializeField] private QuestManager questManager;
    [SerializeField] private TMP_Text questDescriptionText;
    [SerializeField] private TMP_Text acceptButtonText;   // text nằm trong nút Accept

    [Header("Panels to toggle")]
    [SerializeField] private GameObject detailsPanel;     // QuestDetails
    [SerializeField] private GameObject ribbonPanel;      // QuestRibbon (optional)
    [SerializeField] private GameObject questCanvasRoot;  // QuestCanvas (optional)

    [Header("Hiển thị")]
    [SerializeField] private bool showObjectivesBelowDescription = true;
    [SerializeField] private bool showProgressNumbers = true;
    [SerializeField] private bool closePanelOnAccept = true;

    [Header("Quest slots trong log")]
    [SerializeField] private QuestLogSlot[] questSlots;
    [SerializeField] private bool hideAllSlotsOnStart = true;

    [Header("Tên child trong mỗi QuestSlot (để bật/tắt)")]
    [SerializeField] private string completedIconName = "IconCompleted";
    [SerializeField] private string completedTextName = "StatusText"; // để trống nếu không dùng

    private bool isCollapsed = true;
    private PanelMode mode = PanelMode.None;

    private QuestSO pendingQuest;
    private NPCQuestGiver pendingGiver;

    // ---------- lifecycle ----------
    private void Awake()
    {
        if (hideAllSlotsOnStart && questSlots != null)
        {
            foreach (var s in questSlots)
            {
                if (!s) continue;
                if (s.currentQuest == null) s.gameObject.SetActive(false);
                else s.SetQuest(s.currentQuest);
            }
        }
        SetAcceptText(""); // ẩn nút lúc đầu
    }

    private void OnEnable()
    {
        QuestManager.OnObjectiveProgress += HandleProgressEvent;
        QuestManager.OnQuestStateChanged += HandleStateEvent;
        QuestManager.OnQuestDelivered += HandleDeliveredEvent;   // <— nghe Delivered
    }
    private void OnDisable()
    {
        QuestManager.OnObjectiveProgress -= HandleProgressEvent;
        QuestManager.OnQuestStateChanged -= HandleStateEvent;
        QuestManager.OnQuestDelivered -= HandleDeliveredEvent;
    }

    private void HandleProgressEvent(QuestSO q, QuestObjective _, int __, int ___)
    {
        if (mode == PanelMode.Preview && pendingQuest == q)
            UpdateAcceptButtonFor(q);
    }
    private void HandleStateEvent(QuestSO q, QuestState st)
    {
        if (mode == PanelMode.Preview && pendingQuest == q)
            UpdateAcceptButtonFor(q);
        RefreshSlotStates();
    }
    private void HandleDeliveredEvent(QuestSO q)
    {
        if (mode == PanelMode.Preview && pendingQuest == q)
            UpdateAcceptButtonFor(q);
        RefreshSlotStates();
    }

    // ---------- mở từ NPC (preview) ----------
    public void ShowPreviewFromNPC(NPCQuestGiver giver, QuestSO quest)
    {
        mode = PanelMode.Preview;
        pendingGiver = giver;
        pendingQuest = quest;

        OpenPanel();

        // đồng bộ số liệu collect/go-to trước khi render
        questManager?.UpdateAllActiveProgress();

        RenderQuestDetails(quest);
        UpdateAcceptButtonFor(quest);
        RefreshSlotStates();
    }

    // ---------- xem từ slot (inspect) ----------
    public void HandleQuestClicked(QuestSO quest)
    {
        mode = PanelMode.Inspect;
        OpenPanel();
        RenderQuestDetails(quest);
        SetAcceptText(""); // chỉ xem → không hiện nút
    }

    // ---------- nút Accept/Deliver ----------
    public void OnAcceptClicked()
    {
        if (mode != PanelMode.Preview || !pendingQuest || questManager == null) return;

        var before = questManager.GetQuestState(pendingQuest);
        bool changed = questManager.AcceptOrDeliver(pendingQuest);

        if (changed)
        {
            if (before == QuestState.Inactive && !IsQuestAlreadyInLog(pendingQuest))
                AddQuestToLog(pendingQuest);

            RefreshSlotStates();
            if (closePanelOnAccept) CloseOnlyPanel();
        }

        pendingGiver?.NotifyAccepted();
        ClearPending();
    }

    public void OnDeclineClicked()
    {
        pendingGiver?.NotifyDeclined();
        ClearPending();
        CloseAll();
    }

    // ---------- UI helpers ----------
    public void OnMinimizeClicked() => SetCollapsed(true);
    public void OnExpandClicked() => SetCollapsed(false);
    public void OnToggleClicked() => SetCollapsed(!isCollapsed);

    public void CloseOnlyPanel() => SetCollapsed(true);
    public void CloseAll()
    {
        SetCollapsed(true);
        if (questCanvasRoot) questCanvasRoot.SetActive(false);
    }
    private void OpenPanel()
    {
        if (questCanvasRoot) questCanvasRoot.SetActive(true);
        SetCollapsed(false);
    }
    private void SetCollapsed(bool collapse)
    {
        isCollapsed = collapse;
        if (detailsPanel) detailsPanel.SetActive(!collapse);
        if (ribbonPanel) ribbonPanel.SetActive(collapse);
    }

    private void UpdateAcceptButtonFor(QuestSO quest)
    {
        if (!questManager || quest == null) { SetAcceptText(""); return; }

        switch (questManager.GetQuestState(quest))
        {
            case QuestState.Inactive: SetAcceptText("Nhận"); break;
            case QuestState.Completed: SetAcceptText("Giao"); break;
            default: SetAcceptText(""); break; // Active/Delivered -> ẩn nút
        }
    }

    private void SetAcceptText(string s)
    {
        if (!acceptButtonText) return;
        acceptButtonText.text = s;
        acceptButtonText.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(s));
    }

    private void RenderQuestDetails(QuestSO quest)
    {
        if (!quest) return;

        var sb = new StringBuilder();
        string header = string.IsNullOrEmpty(quest.questDescription) ? quest.questName : quest.questDescription;
        sb.AppendLine(header);

        // hiện trạng thái tổng thể ngay dưới tiêu đề
        if (questManager)
        {
            var st = questManager.GetQuestState(quest);
            if (st == QuestState.Delivered) sb.AppendLine("<i>Trạng thái: Đã giao</i>");
            else if (st == QuestState.Completed) sb.AppendLine("<i>Trạng thái: Hoàn thành</i>");
        }

        if (showObjectivesBelowDescription && quest.objectives != null)
        {
            foreach (var o in quest.objectives)
            {
                string line = $"• {o.description}";
                if (showProgressNumbers && questManager != null)
                {
                    questManager.UpdateObjectiveProgress(quest, o);
                    string p = questManager.GetProgressText(quest, o);
                    if (!string.IsNullOrEmpty(p)) line += $" ({p})";
                }
                sb.AppendLine(line);
            }
        }

        if (questDescriptionText)
        {
            questDescriptionText.richText = true;
            questDescriptionText.text = sb.ToString();
        }
    }

    private bool IsQuestAlreadyInLog(QuestSO quest)
    {
        if (questSlots == null) return false;
        foreach (var s in questSlots)
            if (s && s.gameObject.activeSelf && s.currentQuest == quest)
                return true;
        return false;
    }

    private void AddQuestToLog(QuestSO quest)
    {
        if (questSlots == null) return;

        foreach (var slot in questSlots)
        {
            if (!slot) continue;

            if (!slot.gameObject.activeSelf || slot.currentQuest == null)
            {
                slot.gameObject.SetActive(true);
                slot.SetQuest(quest);
                return;
            }
        }

        Debug.LogWarning("QuestLogUI: Hết slot trống để thêm quest.");
    }

    // ==== Bật/tắt IconCompleted & StatusText trực tiếp trong Slot ====
    public void RefreshSlotStates()
    {
        if (questSlots == null || questManager == null) return;

        foreach (var slot in questSlots)
        {
            if (!slot || !slot.gameObject.activeSelf || slot.currentQuest == null) continue;

            bool delivered = questManager.IsQuestTurnedIn(slot.currentQuest);
            bool completed = questManager.IsQuestCompleted(slot.currentQuest); // không OR để phân biệt

            // IconCompleted: bật khi Completed hoặc Delivered
            var iconT = FindDeep(slot.transform, completedIconName);
            if (iconT) iconT.gameObject.SetActive(completed || delivered);

            // StatusText (nếu có): "Hoàn thành" / "Đã giao"
            if (!string.IsNullOrEmpty(completedTextName))
            {
                var txtT = FindDeep(slot.transform, completedTextName);
                if (txtT)
                {
                    var tmp = txtT.GetComponent<TMP_Text>();
                    txtT.gameObject.SetActive(completed || delivered);
                    if (tmp) tmp.text = delivered ? "Đã giao" : "Hoàn thành";
                }
            }
        }
    }

    // tìm child đệ quy theo tên
    private static Transform FindDeep(Transform root, string name)
    {
        if (!root || string.IsNullOrEmpty(name)) return null;
        for (int i = 0; i < root.childCount; i++)
        {
            var c = root.GetChild(i);
            if (c.name == name) return c;
            var r = FindDeep(c, name);
            if (r) return r;
        }
        return null;
    }

    private void ClearPending()
    {
        mode = PanelMode.None;
        pendingQuest = null;
        pendingGiver = null;
        SetAcceptText("");
    }
}
