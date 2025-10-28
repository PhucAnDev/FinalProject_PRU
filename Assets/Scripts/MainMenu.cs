using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Thay ???  trong loadscene bằng tên scene của game nha . 
    public void PlayGame()
    {
        SceneManager.LoadScene("???");
    }

    public void Quit()
    {
        Debug.Log("Game is quitting...");
        Application.Quit();
    }

    [Header("Gán Popup Panel ở đây")]
    public GameObject popupPanel;

    public void ShowPopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }
}﻿

