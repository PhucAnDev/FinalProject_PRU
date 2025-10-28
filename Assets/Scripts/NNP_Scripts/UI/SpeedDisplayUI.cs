// 10/16/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using TMPro;

public class SpeedDisplayUI : MonoBehaviour
{
    public TMP_Text speedText;
    public GroundSpeed groundSpeed;

    private void Update()
    {
        if (speedText != null && groundSpeed != null)
        {
            speedText.text = $"Speed: {groundSpeed.GetGroundSpeed():0.00} m/s";
        }
    }
}