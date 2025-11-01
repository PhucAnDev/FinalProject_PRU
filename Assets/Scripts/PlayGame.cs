using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class PlayGame : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioMixer mixer;

    public void SetVolume()
    {
        mixer.SetFloat("volume", volumeSlider.value);
    }

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
