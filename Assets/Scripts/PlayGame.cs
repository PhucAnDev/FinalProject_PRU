using UnityEngine;

public class PlayGame : MonoBehaviour
{
    
    public void OnStartClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    public void OnEditorClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif  
        Application.Quit();
    }
}
