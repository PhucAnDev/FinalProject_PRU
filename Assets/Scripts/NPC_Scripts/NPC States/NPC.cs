using UnityEngine;

public class NPC : MonoBehaviour
{
    // Tham chiếu đến các script trạng thái khác nhau
    public NPC_Patrol patrol; // Script tuần tra
    public NPC_Wander wander; // Script đi lang thang
    public NPC_Talk talk;     // Script nói chuyện

    // Enum để định nghĩa các trạng thái có thể có của NPC
    public enum NPCState
    {
        Default,
        Patrol,
        Wander,
        Talk,
        Idle
    }

    // Biến để theo dõi trạng thái hiện tại
    public NPCState currentState; 
    // Biến để lưu trạng thái mặc định
    private NPCState defaultState; 

    void Start()
    {
        // Lưu trạng thái ban đầu làm trạng thái mặc định
        defaultState = currentState; 
        // Chuyển sang trạng thái ban đầu khi game bắt đầu
        SwitchState(currentState); 
    }

    // Phương thức để chuyển đổi giữa các trạng thái
    public void SwitchState(NPCState newState)
    {
        // Cập nhật trạng thái hiện tại
        currentState = newState; 

        // Bật/tắt các script dựa trên trạng thái mới
        // Chỉ script tương ứng với trạng thái mới được bật (enabled)
        patrol.enabled = (currentState == NPCState.Patrol); 
        wander.enabled = (currentState == NPCState.Wander); 
        talk.enabled = (currentState == NPCState.Talk); 
    }

    // Được gọi khi một đối tượng khác đi vào trigger collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng có tag "Player" không
        if (other.CompareTag("Player"))
        {
            // Nếu là Player, chuyển sang trạng thái Talk
            SwitchState(NPCState.Talk);
        }
    }

    // Được gọi khi một đối tượng khác rời khỏi trigger collider
    private void OnTriggerExit2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng có tag "Player" không
        if (other.CompareTag("Player")) 
        {
            // Nếu là Player, chuyển về trạng thái mặc định
            SwitchState(defaultState); 
        }
    }
}