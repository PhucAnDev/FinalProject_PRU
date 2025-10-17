using TMPro;
using UnityEngine;

public class QuestLogSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text questNameText;    //Nơi hiển thị tên nhiệm vụ
    [SerializeField] private TMP_Text questDescriptionText; //Nơi hiển thị mô tả nhiệm vụ
    [SerializeField] private TMP_Text questLeverText; //Nơi hiển thị tiến trình nhiệm vụ

    public QuestSO currentQuest; //Biến tham chiếu đến QuestSO

    private void OnValidate()
    {
        if (currentQuest != null)
            SetQuest(currentQuest);
    }
    public void SetQuest(QuestSO questSO)
    {
        currentQuest = questSO;
        // Cập nhật UI với thông tin nhiệm vụ
        questNameText.text = questSO.questName;
        questDescriptionText.text = questSO.questDescription;
        questLeverText.text = $"Level: {questSO.questLever1.ToString()}";
        // Giả sử nhiệm vụ chỉ có một mục tiêu để đơn giản hóa
        
    }
}
