using UnityEngine;

public class LivesResetter : MonoBehaviour
{
    public IntVariable Lives;
    public int defaultLives = 3;

    private void Awake()
    {
        if (Lives != null)
        {
            Lives.Value = defaultLives;
        }
    }
}
