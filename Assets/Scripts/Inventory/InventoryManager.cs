using System.Linq;
using UnityEngine;

public class InventoryManagemant : MonoBehaviour
{
    public InventorySlot[] itemSlots;
    public GameObject lootPrefab;
    public Transform player;


    private void Start()
    {
        foreach (var slot in itemSlots)
        {
            slot.UpdateUI();
        }
    }

    private void OnEnable() => Loot.OnItemLooted += AddItem;
    private void OnDisable() => Loot.OnItemLooted -= AddItem;

    private void AddItem(ItemSO itemSO, int quantity)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
            {
                int availableSpace = itemSO.stackSize - slot.quantity;
                int quantityToAdd = Mathf.Min(availableSpace, quantity);

                slot.quantity += quantityToAdd;
                quantity -= quantityToAdd;

                slot.UpdateUI();

                if (quantity <= 0)
                    return;
            }

        }

        foreach (var slot in itemSlots)
        {
            if(slot.itemSO == null)
            {
                int amountToAdd = Mathf.Min(itemSO.stackSize - quantity);
                slot.itemSO = itemSO;
                slot.quantity = quantity;
                slot.UpdateUI();
                return;
            }
        }

        if (quantity > 0)
        {
            DropLoot(itemSO, quantity);
        }
    }

    public void DropItem(InventorySlot slot)
    {
        DropLoot(slot.itemSO, 1);
        slot.quantity -= 1;
        if (slot.quantity <= 0)
        {
            slot.itemSO = null;
        }
        slot.UpdateUI();
    }

    private void DropLoot(ItemSO itemSO, int quantity)
    {
        Loot loot = Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>();
        loot.Initialize(itemSO, quantity);
    }

}
