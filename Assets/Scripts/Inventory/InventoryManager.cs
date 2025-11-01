using System.Linq;
using UnityEngine;

public class InventoryManagemant : MonoBehaviour
{
    public static InventoryManagemant Instance;
    public InventorySlot[] itemSlots;
    public GameObject lootPrefab;
    public Transform player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (var slot in itemSlots)
            slot.UpdateUI();
    }

    private void OnEnable() => Loot.OnItemLooted += AddItem;
    private void OnDisable() => Loot.OnItemLooted -= AddItem;

    // --- ADD ITEM (đã fix logic) ---
    private void AddItem(ItemSO itemSO, int quantity)
    {
        if (itemSO == null || quantity <= 0) return;

        // 1) Ưu tiên cộng vào các stack đã có cùng item
        foreach (var slot in itemSlots)
        {
            if (quantity <= 0) break;
            if (slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
            {
                int space = itemSO.stackSize - slot.quantity;
                int toAdd = Mathf.Min(space, quantity);

                slot.quantity += toAdd;
                quantity -= toAdd;
                slot.UpdateUI();
            }
        }

        // 2) Đổ vào các slot trống
        foreach (var slot in itemSlots)
        {
            if (quantity <= 0) break;
            if (slot.itemSO == null)
            {
                int toAdd = Mathf.Min(itemSO.stackSize, quantity);
                slot.itemSO = itemSO;
                slot.quantity = toAdd;
                quantity -= toAdd;
                slot.UpdateUI();
            }
        }

        // 3) Nếu vẫn còn dư, thả ra đất
        if (quantity > 0)
            DropLoot(itemSO, quantity);
    }

    public void DropItem(InventorySlot slot)
    {
        if (slot == null || slot.itemSO == null || slot.quantity <= 0) return;

        DropLoot(slot.itemSO, 1);
        slot.quantity -= 1;

        if (slot.quantity <= 0)
            slot.itemSO = null;

        slot.UpdateUI();
    }

    private void DropLoot(ItemSO itemSO, int quantity)
    {
        Loot loot = Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>();
        loot.Initialize(itemSO, quantity); // Đảm bảo Loot có hàm Initialize(item, qty)
    }

    public bool HasItem(ItemSO itemSO)
    {
        foreach (var slot in itemSlots)
            if (slot.itemSO == itemSO && slot.quantity > 0)
                return true;
        return false;
    }

    public bool HasAtLeast(ItemSO itemSO, int amount)
        => GetItemQuantity(itemSO) >= amount;

    public int GetItemQuantity(ItemSO itemSO)
    {
        int total = 0;
        foreach (var slot in itemSlots)
            if (slot.itemSO == itemSO)
                total += slot.quantity;
        return total;
    }

    // --- CONSUME / REMOVE ---
    /// <summary>
    /// Cố gắng trừ 'quantity' item. Chỉ trừ khi đủ số lượng. Trả về true nếu thành công.
    /// </summary>
    public bool TryConsume(ItemSO itemSO, int quantity)
    {
        if (itemSO == null || quantity <= 0) return true;

        int total = GetItemQuantity(itemSO);
        if (total < quantity) return false;

        int remaining = quantity;
        foreach (var slot in itemSlots)
        {
            if (remaining == 0) break;
            if (slot.itemSO != itemSO || slot.quantity <= 0) continue;

            int take = Mathf.Min(slot.quantity, remaining);
            slot.quantity -= take;
            remaining -= take;

            if (slot.quantity <= 0)
                slot.itemSO = null;

            slot.UpdateUI();
        }
        return true;
    }

    /// <summary>
    /// Alias của TryConsume để tiện gọi ở nơi khác.
    /// </summary>
    public bool RemoveItem(ItemSO itemSO, int quantity) => TryConsume(itemSO, quantity);
}
