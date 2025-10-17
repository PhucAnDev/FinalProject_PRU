using System;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public ItemSO itemSO;   //Tham chiếu dữ liệu item (đặt trong Inspector)
    public SpriteRenderer sr;   // Renderer để hiển thị icon của item rơi trên đất
    public Animator anim;   // (Không bắt buộc) Animator để chơi animation nhặt

    public bool canBePickUp = true;
    public int quantity;
    public static event Action<ItemSO, int> OnItemLooted; //Sự kiện nhặt item

    private void OnValidate()
    {
        if (itemSO == null) 
            return;
        UpdateAppearance();
    }

    public void Initialize(ItemSO itemSO, int quantity)
    {
        this.itemSO = itemSO;
        this.quantity = quantity;
        canBePickUp = false;
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        sr.sprite = itemSO.icon;
        this.name = itemSO.itemName;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canBePickUp == true)
        {
            anim.Play("LootPickup");
            OnItemLooted?.Invoke(itemSO, quantity); //Giả sử mỗi lần nhặt 1 cái
            Destroy(this.gameObject, 0.5f); //Chờ animation chạy xong rồi hủy
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canBePickUp = true;
        }
    }
}
