// 10/20/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("References")]
    public FloatVariable PlayerSpeed;
    public BoolVariable IsAlive;
    public TMP_Text scoreText;
    public TMP_Text finalScoreText; // Thêm tham chiếu đến FinalScoreText

    [Header("Score Config")]
    public float comboDuration = 3f;

    [Header("UI Reference")]
    public GameOverUI gameOverUI;

    private float score = 0f;
    private float comboMultiplier = 1f;
    private float comboTimer = 0f;
    private bool isScoringActive = true;
    private float lastTrickAddTime = -999f;
    private float trickCooldown = 0.1f;

    void Awake() => Instance = this;

    void Update()
    {
        if (!IsAlive.Value || !isScoringActive) return;

        if (comboMultiplier > 1f)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
                comboMultiplier = 1f;
        }

        if (scoreText != null)
            scoreText.text = "Score: " + Mathf.RoundToInt(score);
    }

    public void AddScore(float amount)
    {
        score += amount * comboMultiplier;
    }

    public void AddTrickScore(float amount)
    {
        if (!isScoringActive) return;

        if (Time.time - lastTrickAddTime < trickCooldown)
            return;

        score += amount * comboMultiplier;
        comboMultiplier = Mathf.Min(comboMultiplier + 1f, 5f);
        comboTimer = comboDuration;

        lastTrickAddTime = Time.time;
    }

    public void ResetCombo() => comboMultiplier = 1f;

    public void OnCrash()
    {
        if (!isScoringActive) return;

        isScoringActive = false;
        IsAlive.Value = false;
        StopScoring();

        if (finalScoreText != null) // Hiển thị điểm cuối cùng
        {
            finalScoreText.text = "Final Score: " + Mathf.RoundToInt(score);
        }

        if (gameOverUI != null)
            gameOverUI.ShowGameOver(score);
    }

    public void OnFinishLine()
    {
        if (!isScoringActive) return;
        AddScore(500f);
        StopScoring();

        if (finalScoreText != null) // Hiển thị điểm cuối cùng
        {
            finalScoreText.text = "Final Score: " + Mathf.RoundToInt(score);
        }

        if (gameOverUI != null)
            gameOverUI.ShowGameOver(score);
    }

    public void StopScoring()
    {
        isScoringActive = false;
        Debug.Log("🏁 Scoring stopped. Final score: " + score);
    }

    public float GetFinalScore() => score;
}