using UnityEngine;


public class NPC_Talk : MonoBehaviour
{
    // Tham chiếu đến các component
    private Rigidbody2D rb;
    private Animator anim;
    public Animator interactAnim; // Animator cho biểu tượng tương tác

    private void Awake()
    {
        // Lấy component Rigidbody2D từ chính đối tượng này
        rb = GetComponent<Rigidbody2D>(); 
        // Lấy component Animator từ đối tượng con
        anim = GetComponentInChildren<Animator>(); 
    }

    private void OnEnable()
    {
        // Khi script này được bật, dừng NPC lại
        rb.linearVelocity = Vector2.zero; 
        // Đặt Rigidbody thành isKinematic để nó không bị đẩy đi
        rb.isKinematic = true; 

        // Phát animation "idle" cho NPC
        anim.Play("Idle"); 
        // Phát animation "open" cho biểu tượng tương tác
        interactAnim.Play("Open"); 
    }

    private void OnDisable()
    {
        // Khi script này bị tắt, cho phép NPC di chuyển lại
        rb.isKinematic = false; 
        // Phát animation "close" để ẩn biểu tượng tương tác
        interactAnim.Play("Close"); 
    }
}