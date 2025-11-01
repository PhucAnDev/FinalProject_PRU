using UnityEngine;

public class CheatManager : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public BoolVariable IsAlive; // Dùng BoolVariable của hệ thống hiện tại

    private bool lastCheatState = false; // để phát hiện thay đổi trạng thái cheat

    private void Update()
    {
        // Nếu trạng thái cheat thay đổi (từ bật sang tắt hoặc ngược lại)
        if (PlayerMovement.IsCheatModeActive != lastCheatState)
        {
            lastCheatState = PlayerMovement.IsCheatModeActive;

            if (lastCheatState)
                ActivateCheat();
            else
                DeactivateCheat();
        }
    }

    private void ActivateCheat()
    {
        Debug.Log("🔥 CHEAT MODE ACTIVATED: Invincible + Max Speed!");

        // Luôn sống
        if (IsAlive != null)
            IsAlive.SetValue(true);

       
    }

    private void DeactivateCheat()
    {
        Debug.Log("❄️ CHEAT MODE DEACTIVATED!");

        // Trả về tốc độ mặc định
        if (playerMovement != null)
        {
            playerMovement.GetGroundSpeed().SetGroundSpeed(playerMovement.DefaultSpeed.Value);
        }
    }
}
