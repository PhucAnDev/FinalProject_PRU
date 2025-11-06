using UnityEngine;
using TMPro;

public class GameOverWatcher : MonoBehaviour
{
    [Header("References")]
    public BoolVariable IsAlive;
    public GameOverUI gameOverUI;
    public ScoreManager scoreManager;

    [Header("UI References")]
    [Tooltip("Text hiển thị điểm cuối (Final Score)")]
    public TMP_Text finalScoreText;

    private bool wasAlive = true;

    void Update()
    {
        // Khi trạng thái đổi từ sống → chết
        if (wasAlive && !IsAlive.Value)
        {
            wasAlive = false;

            // Lấy điểm cuối cùng từ ScoreManager
            float finalScore = scoreManager != null ? scoreManager.GetFinalScore() : 0f;

            // Hiển thị lên TextMeshPro
            if (finalScoreText != null)
                finalScoreText.text = $"Final Score: {finalScore:0}";

            // Gọi UI Game Over
            if (gameOverUI != null)
                gameOverUI.ShowGameOver(finalScore);
        }
    }
}
