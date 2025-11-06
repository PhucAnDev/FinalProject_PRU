using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroduceScene : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}