using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestCompletionIndicator : MonoBehaviour
{
    public enum IndicatorMode { SlotIcon, GlobalPopup }

    [Header("Mode")]
    [SerializeField] private IndicatorMode mode = IndicatorMode.SlotIcon;

    [Header("Common")]
    [Tooltip("Để trống nếu muốn lấy Quest từ QuestSlot cha")]
    [SerializeField] private QuestSO questOverride;
    [Tooltip("Nếu không dùng event, đặt >0 để kiểm tra định kỳ (giây). 0 = tắt polling")]
    [SerializeField] private float pollInterval = 0f;

    [Header("Slot Icon (gắn lên IconCompleted)")]
    [SerializeField] private QuestLogSlot slot;          // auto lấy từ parent
    [SerializeField] private GameObject iconGO;          // auto = chính gameObject
    [SerializeField] private TMP_Text statusText;        // (tuỳ chọn) chữ “Hoàn thành”

    [Header("Global Popup (gắn lên popup/badge)")]
    [SerializeField] private bool listenAny = false;     // true: nghe tất cả quest
    [SerializeField] private GameObject popupRoot;       // auto = chính gameObject
    [SerializeField] private TMP_Text popupLabel;        // (tuỳ chọn) hiện tên quest
    [SerializeField] private string labelFormat = "Hoàn thành: {0}";
    [SerializeField] private float autoHideAfter = 0f;   // >0: tự ẩn sau X giây

    private QuestManager mgr;
    private Coroutine pollCo;

    // ---------- lifecycle ----------
    private void OnValidate()
    {
        if (!slot) slot = GetComponentInParent<QuestLogSlot>();
        if (!iconGO) iconGO = gameObject;
        if (!popupRoot) popupRoot = gameObject;
    }

    private void OnEnable()
    {
        mgr = FindObjectOfType<QuestManager>();

        // Ẩn mặc định
        SetSlotIconVisible(false);
        SetPopupVisible(false);

        // Lắng nghe event từ QuestManager (cần bản QuestManager có OnObjectiveProgress/OnQuestCompleted)
        QuestManager.OnObjectiveProgress += OnObjectiveProgress;
        QuestManager.OnQuestCompleted += OnQuestCompleted;

        // Fallback: nếu bạn không muốn dùng event, có thể bật polling
        if (pollInterval > 0f && mode == IndicatorMode.SlotIcon)
            pollCo = StartCoroutine(PollLoop());
    }

    private void OnDisable()
    {
        QuestManager.OnObjectiveProgress -= OnObjectiveProgress;
        QuestManager.OnQuestCompleted -= OnQuestCompleted;
        if (pollCo != null) StopCoroutine(pollCo);
    }

    // ---------- events ----------
    private void OnObjectiveProgress(QuestSO q, QuestObjective _, int __, int ___)
    {
        if (mode != IndicatorMode.SlotIcon) return;
        if (q == GetTargetQuest()) RefreshSlotOnce();
    }

    private void OnQuestCompleted(QuestSO q)
    {
        var target = GetTargetQuest();

        if (mode == IndicatorMode.SlotIcon)
        {
            if (q == target) SetSlotIconVisible(true);
        }
        else // GlobalPopup
        {
            if (listenAny || q == target) ShowPopup(q);
        }
    }

    // ---------- logic ----------
    private QuestSO GetTargetQuest()
    {
        if (questOverride) return questOverride;
        if (slot) return slot.currentQuest;
        return null;
    }

    private IEnumerator PollLoop()
    {
        var wait = new WaitForSeconds(pollInterval);
        while (true)
        {
            RefreshSlotOnce();
            yield return wait;
        }
    }

    public void RefreshSlotOnce()
    {
        if (mode != IndicatorMode.SlotIcon) return;

        var q = GetTargetQuest();
        bool done = (mgr && q && mgr.IsQuestCompleted(q));
        SetSlotIconVisible(done);
    }

    private void SetSlotIconVisible(bool on)
    {
        if (mode != IndicatorMode.SlotIcon) return;

        if (iconGO) iconGO.SetActive(on);
        if (statusText)
        {
            statusText.gameObject.SetActive(on);
            if (on) statusText.text = "Hoàn thành";
        }
    }

    private void ShowPopup(QuestSO q)
    {
        if (mode != IndicatorMode.GlobalPopup) return;

        if (popupRoot) popupRoot.SetActive(true);
        if (popupLabel)
            popupLabel.text = string.Format(labelFormat, q ? q.questName : "Quest");

        if (autoHideAfter > 0f)
            StartCoroutine(HideLater(autoHideAfter));
    }

    private IEnumerator HideLater(float t)
    {
        yield return new WaitForSeconds(t);
        SetPopupVisible(false);
    }

    private void SetPopupVisible(bool on)
    {
        if (mode != IndicatorMode.GlobalPopup) return;
        if (popupRoot) popupRoot.SetActive(on);
        if (popupLabel) popupLabel.gameObject.SetActive(on);
    }
}
