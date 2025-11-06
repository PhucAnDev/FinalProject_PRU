using UnityEngine;
using TMPro;

public class LivesUI : MonoBehaviour
{
    public IntVariable Lives;
    public TMP_Text livesText;

    private void Start()
    {
        UpdateLivesUI();
    }

    private void Update()
    {
        UpdateLivesUI();
    }

    private void UpdateLivesUI()
    {
        if (livesText != null && Lives != null)
        {
            livesText.text = "Lives: " + Lives.Value;
        }
    }
}
