using UnityEngine;

public class GameOverWatcher : MonoBehaviour
{
    public BoolVariable IsAlive;        
    public GameOverUI gameOverUI;       
    public IntVariable FinalScore;    

    private bool wasAlive = true;

    void Update()
    {
        // Khi trạng thái đổi từ true → false
        if (wasAlive && !IsAlive.Value)
        {
            wasAlive = false;

            float score = FinalScore != null ? FinalScore.Value : 0f;
            gameOverUI.ShowGameOver(score);
        }
    }
}
