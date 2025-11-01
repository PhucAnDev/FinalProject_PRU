using UnityEngine;

public class LoopingSpriteShape : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform groundA;
    public Transform groundB;

    [Header("Prefabs")]
    public GameObject spikePrefab;
    public GameObject heartPrefab;

    [Header("Settings")]
    public float stepDown = 0f;
    public float spawnSpacing = 25f; // khoảng cách giữa các vật thể
    public float spikeHeightOffset = 1.2f; // độ cao so với mặt đất
    float randomOffsetY = 3f; 

    private float groundLength;
    private int loopCount = 0;

    // Số lượng khởi đầu và tăng dần
    private const int startSpikes = 50;
    private const int startHearts = 50;
    private const int addSpikesPerLoop = 50;
    private const int addHeartsPerLoop = 15;
    private const int maxSpikes = 300;
    private const int maxHearts = 100;

    private void Start()
    {
        // Lấy chiều dài lớn nhất
        var a = groundA.GetComponent<EdgeCollider2D>();
        var b = groundB.GetComponent<EdgeCollider2D>();
        groundLength = Mathf.Max(a.bounds.size.x, b.bounds.size.x);

        // Spawn khởi tạo
        SpawnObjectsOnGround(groundA);
        SpawnObjectsOnGround(groundB);
    }

    private void Update()
    {
        if (player == null) return;

        Transform front = groundA.position.x > groundB.position.x ? groundA : groundB;
        Transform back = (front == groundA) ? groundB : groundA;

        // Khi player vượt qua ground trước
        if (player.position.x > front.position.x)
        {
            MoveGround(back, front);
        }
    }

    private void MoveGround(Transform back, Transform front)
    {
        EdgeCollider2D frontEdge = front.GetComponent<EdgeCollider2D>();
        float frontBottomY = frontEdge.bounds.min.y;

        // Di chuyển ground phía sau ra nối tiếp ground phía trước
        back.position = new Vector3(
            front.position.x + groundLength,
            frontBottomY - stepDown,
            back.position.z
        );

        // Xóa vật thể cũ (spike, heart cũ)
        foreach (Transform child in back)
        {
            Destroy(child.gameObject);
        }

        // Spawn vật thể mới trên ground vừa respawn
        SpawnObjectsOnGround(back);

        Debug.Log($"Respawn {back.name} -> new pos: {back.position}");
    }

    private void SpawnObjectsOnGround(Transform ground)
    {
        loopCount++;

        int spikeCount = Mathf.Min(startSpikes + addSpikesPerLoop * (loopCount - 1), maxSpikes);
        int heartCount = Mathf.Min(startHearts + addHeartsPerLoop * (loopCount - 1), maxHearts);

        Debug.Log($"[Loop {loopCount}] -> Spawning {spikeCount} spikes, {heartCount} hearts.");

        EdgeCollider2D edge = ground.GetComponent<EdgeCollider2D>();
        if (edge == null)
        {
            Debug.LogWarning($"No EdgeCollider2D found on {ground.name}");
            return;
        }

        Vector2[] points = edge.points;
        float offsetStartX = 40f; 
        float minX = edge.bounds.min.x + offsetStartX;
        float maxX = edge.bounds.max.x;


        // Hàm nội suy Y từ X
        float GetYAtX(float x)
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector2 p1 = ground.TransformPoint(points[i]);
                Vector2 p2 = ground.TransformPoint(points[i + 1]);

                if (x >= p1.x && x <= p2.x)
                {
                    float t = (x - p1.x) / (p2.x - p1.x);
                    return Mathf.Lerp(p1.y, p2.y, t);
                }
            }
            return ground.TransformPoint(points[points.Length - 1]).y;
        }

        // Danh sách lưu vị trí đã spawn
        System.Collections.Generic.List<float> usedX = new();

        float minDistance = 30f;

        // ==== Spawn Spike ====
        for (int i = 0; i < spikeCount; i++)
        {
            int safety = 0;
            float offsetX;
            do
            {
                offsetX = Random.Range(minX, maxX);
                safety++;
            } while (IsTooClose(offsetX, usedX, minDistance) && safety < 50);

            usedX.Add(offsetX);

            float y = GetYAtX(offsetX);
            Vector3 spawnPos = new Vector3(offsetX, y - 0.1f, 0);
            Instantiate(spikePrefab, spawnPos, Quaternion.identity, ground);
        }

        // ==== Spawn Heart ====
        for (int i = 0; i < heartCount; i++)
        {
            int safety = 0;
            float offsetX;
            do
            {
                offsetX = Random.Range(minX, maxX);
                safety++;
            } while (IsTooClose(offsetX, usedX, minDistance) && safety < 50);

            usedX.Add(offsetX);

            float y = GetYAtX(offsetX);
            Vector3 spawnPos = new Vector3(offsetX, y + randomOffsetY, 0);
            Instantiate(heartPrefab, spawnPos, Quaternion.identity, ground);
        }

        // ===== Hàm kiểm tra khoảng cách =====
        bool IsTooClose(float x, System.Collections.Generic.List<float> existing, float minDist)
        {
            foreach (float ex in existing)
            {
                if (Mathf.Abs(x - ex) < minDist)
                    return true;
            }
            return false;
        }
    }

}


