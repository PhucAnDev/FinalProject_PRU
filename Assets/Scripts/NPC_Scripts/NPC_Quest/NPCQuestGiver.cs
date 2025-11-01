using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NPCQuestGiver : MonoBehaviour
{
    [Header("Quest hiển thị khi lại gần")]
    [SerializeField] private QuestSO quest;

    [Header("UI")]
    [SerializeField] private QuestLogUI questUI;

    [Header("Tương tác")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool requireInteractKey = false; // bật nếu muốn nhấn phím mới mở
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Indicator (tùy chọn)")]
    [SerializeField] private GameObject questIndicator; // icon chấm hỏi trên đầu NPC

    private void Reset()
    {
        // collider dùng làm vùng 'đến gần' → phải là trigger
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (!requireInteractKey) ShowPreview();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (requireInteractKey && Input.GetKeyDown(interactKey)) ShowPreview();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        questUI?.CloseOnlyPanel();
        ToggleIndicator(true);
    }

    private void ShowPreview()
    {
        if (!quest || !questUI) return;

        // == GỌI UI mở panel ở chế độ PREVIEW ==
        // Nếu QuestLogUI của bạn có hàm ShowPreviewFromNPC(NPCQuestPresenter, QuestSO),
        // bạn có thể thêm 1 overload nhận NPCQuestGiver (xem ghi chú bên dưới),
        // còn ở đây ta gọi thẳng phiên bản dành cho Giver:
        questUI.ShowPreviewFromNPC(this, quest);

        ToggleIndicator(false);
    }

    // UI có thể gọi nếu bạn muốn hiệu ứng/ẩn icon khi Accept/Decline
    public void NotifyAccepted() => ToggleIndicator(false);
    public void NotifyDeclined() => ToggleIndicator(true);

    private void ToggleIndicator(bool show)
    {
        if (questIndicator) questIndicator.SetActive(show);
    }
}
