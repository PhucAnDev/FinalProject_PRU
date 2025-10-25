using TMPro;
using UnityEngine;

public class QuestLogSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text questNameText;    //Nơi hiển thị tên nhiệm vụ
    //[SerializeField] private TMP_Text questDescriptionText; //Nơi hiển thị mô tả nhiệm vụ
    //[SerializeField] private TMP_Text questLeverText;

    public QuestSO currentQuest; //Biến tham chiếu đến QuestSO

    public QuestLogUI questLogUI; //Tham chiếu đến QuestLogUI để thông báo khi nhiệm vụ được chọn

    private void OnValidate()
    {
        if (currentQuest != null)
            SetQuest(currentQuest);
        else
            gameObject.SetActive(false);
    }
    public void SetQuest(QuestSO questSO)
    {
        currentQuest = questSO;
        // Cập nhật UI với thông tin nhiệm vụ
        questNameText.text = questSO.questName;
        //questDescriptionText.text = questSO.questDescription;
        //questLeverText.text = $"Level: {questSO.questLever1.ToString()}";
        // Giả sử nhiệm vụ chỉ có một mục tiêu để đơn giản hóa

        gameObject.SetActive(true);
    }

    public void OnSlotClicked()
    {
            questLogUI.HandleQuestClicked(currentQuest);
    }
}
