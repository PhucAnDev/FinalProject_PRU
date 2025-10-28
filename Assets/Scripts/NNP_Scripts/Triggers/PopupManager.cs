using UnityEngine;

public class PopupManager : MonoBehaviour
{
    [Header("Gán Popup Panel ở đây")]
    public GameObject popupPanel;

    // Bật popup
    public void ShowPopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(true);
    }

    // Tắt popup
    public void ClosePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }
}