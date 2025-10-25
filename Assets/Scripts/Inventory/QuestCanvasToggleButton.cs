using System.Collections;
using UnityEngine;

public class QuestCanvasToggleButton : MonoBehaviour
{
    [SerializeField] private GameObject targetRoot;     // QuestCanvas (root)
    [Header("Nhiều CanvasGroup (nếu bạn không dùng 1 group ở root)")]
    [SerializeField] private CanvasGroup[] groups;
    [SerializeField] private bool useFade = true;
    [SerializeField] private float fadeDuration = 0.15f;
    [SerializeField] private bool startHidden = true;
    [SerializeField] private bool deactivateOnHide = false;

    void Awake()
    {
        if (startHidden)
        {
            if (useFade && groups != null && groups.Length > 0)
                foreach (var g in groups) SetCG(g, 0f, false, false);
            else if (targetRoot) targetRoot.SetActive(false);
        }
    }

    // GÁN NÚT GỌI HÀM NÀY
    public void Toggle()
    {
        if (useFade && groups != null && groups.Length > 0)
        {
            bool show = groups[0].alpha < 0.5f; // lấy theo group đầu
            StopAllCoroutines();
            StartCoroutine(FadeAll(show));
        }
        else if (targetRoot) targetRoot.SetActive(!targetRoot.activeSelf);
    }

    public void Show() { if (useFade && groups?.Length > 0) { StopAllCoroutines(); StartCoroutine(FadeAll(true)); } else if (targetRoot) targetRoot.SetActive(true); }
    public void Hide() { if (useFade && groups?.Length > 0) { StopAllCoroutines(); StartCoroutine(FadeAll(false)); } else if (targetRoot) targetRoot.SetActive(false); }

    IEnumerator FadeAll(bool show)
    {
        if (show && targetRoot && !targetRoot.activeSelf) targetRoot.SetActive(true);

        float t = 0f;
        float dur = Mathf.Max(0.01f, fadeDuration);
        float from = groups[0].alpha, to = show ? 1f : 0f;

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(from, to, t / dur);
            foreach (var g in groups) if (g) g.alpha = a;
            yield return null;
        }
        foreach (var g in groups) if (g) SetCG(g, to, show, show);

        if (!show && deactivateOnHide && targetRoot) targetRoot.SetActive(false);
    }

    void SetCG(CanvasGroup g, float a, bool interact, bool blocks)
    { g.alpha = a; g.interactable = interact; g.blocksRaycasts = blocks; }
}
