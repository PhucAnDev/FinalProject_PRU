using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameWatcher : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private QuestManager questManager;
    [SerializeField] private QuestLogSlot[] questSlots;   // K�o ��ng 3 slot b?n d�ng
    [SerializeField] private GameObject endGameRoot;      // Panel endgame (?n s?n)

    [Header("Config")]
    [SerializeField] private int requiredDelivered = 3;   // �i?u ki?n th?ng
    [SerializeField] private bool pauseOnShow = true;     // D?ng game khi hi?n

    private bool shown;

    private void Awake()
    {
        if (endGameRoot) endGameRoot.SetActive(false);
    }

    private void OnEnable()
    {
        QuestManager.OnQuestDelivered += OnAnyQuestChanged;
        QuestManager.OnQuestStateChanged += OnAnyQuestStateChanged;
    }

    private void OnDisable()
    {
        QuestManager.OnQuestDelivered -= OnAnyQuestChanged;
        QuestManager.OnQuestStateChanged -= OnAnyQuestStateChanged;
    }

    private void Start() => Evaluate();

    private void OnAnyQuestChanged(QuestSO _)
    {
        Evaluate();
    }

    private void OnAnyQuestStateChanged(QuestSO _, QuestState __)
    {
        Evaluate();
    }

    private void Evaluate()
    {
        if (shown || questManager == null || questSlots == null || endGameRoot == null) return;

        int delivered = 0;
        foreach (var slot in questSlots)
        {
            if (!slot || !slot.gameObject.activeSelf || slot.currentQuest == null) continue;
            if (questManager.IsQuestTurnedIn(slot.currentQuest)) delivered++;
        }

        if (delivered >= requiredDelivered)
            ShowEndGame();
    }

    private void ShowEndGame()
    {
        shown = true;
        endGameRoot.SetActive(true);
        if (pauseOnShow) Time.timeScale = 0f; // ��ng b�ng gameplay
        Debug.Log("[EndGame] All required quests delivered. Show end screen.");
    }

    // Tu? ch?n: g?i t? n�t "Restart/Continue"
    public void HideEndGame()
    {
        shown = false;
        if (pauseOnShow) Time.timeScale = 1f;
        if (endGameRoot) endGameRoot.SetActive(false);
    }

    public void LoadEndGameScene()
    {
        //if (pauseOnShow) Time.timeScale = 1f; // ensure gameplay resumes at normal speed before switching
        SceneManager.LoadScene("MainMenu");
    }
}
