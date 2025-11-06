using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SkiSound : MonoBehaviour
{
    private AudioSource audioSource;
    private Coroutine fadeOutRoutine;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float baseVolume = 0.1f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    public void OnGrounded()
    {
        if (fadeOutRoutine != null)
        {
            StopCoroutine(fadeOutRoutine);
            fadeOutRoutine = null;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.volume = 0f;
            audioSource.Play();
            StartCoroutine(FadeIn(0.2f)); 
        }
    }

    public void OnAirborne()
    {
        // Fade out thay vì stop ngay
        if (audioSource.isPlaying)
        {
            fadeOutRoutine = StartCoroutine(FadeOut(0.3f));
        }
    }

    private IEnumerator FadeIn(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, baseVolume, time / duration); 
            yield return null;
        }
        audioSource.volume = baseVolume; 
    }

    private IEnumerator FadeOut(float duration)
    {
        float startVol = audioSource.volume;
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVol, 0, time / duration);
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = baseVolume; 
    }

}
