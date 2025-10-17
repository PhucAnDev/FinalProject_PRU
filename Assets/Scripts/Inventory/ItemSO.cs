using UnityEngine;

[CreateAssetMenu(fileName = "NewItem")]
public class ItemSO : ScriptableObject
{
    public string  itemName;
    [TextArea] public string itemDescription;
    public Sprite icon;

    public int stackSize = 3;
}
