using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverCanvas;
    public TMP_Text finalScoreText;

    private bool isShown = false;

    void Start()
    {
        if (gameOverCanvas != null && gameOverCanvas != gameObject)
            gameOverCanvas.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    public void ShowGameOver(float finalScore)
    {
        if (isShown) return;
        isShown = true;

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);

            if (finalScoreText != null)
                finalScoreText.text = $"Final Score: {Mathf.RoundToInt(finalScore)}";
        }

        if (MusicManager.Instance != null)
            MusicManager.Instance.StopMusic();

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMusic();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // nếu sau này bạn có MainMenu
    }
}
