using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestSO", menuName = "QuestSO")]
public class QuestSO : ScriptableObject
{
    public string questName;
    [TextArea] public string questDescription;
    public int questLever1;

    public List<QuestObjective> objectives;
}

[System.Serializable]
public class QuestObjective
{
    public string description;

    //public ItemSO targetItem;               //Biến dùng cho nhiệm vụ thu thập Ỉtem
    //public ActorSO targetNPC;               //Biến dùng cho nhiệm vụ liên quan đến NPC
    //public LocationSO targetLocation;       //Biến dùng cho nhiệm vụ đến vị trí

    public int requiredAmount;                  //Biến thu thập số tiền cần thiết
    public int currentAmount;                   //Biến theo dõi số tiền hiện tại

}