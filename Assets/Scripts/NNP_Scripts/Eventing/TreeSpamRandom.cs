using UnityEngine;
using UnityEngine.U2D;
using System.Collections.Generic;

public class TreeSpawnerOnGround : MonoBehaviour
{
    [Header("Ground References")]
    public SpriteShapeController[] Grounds; 

    [Header("Tree Settings")]
    public GameObject TreePrefab;
    public int totalTreeCount = 1000;
    public float randomXOffset = 0.5f;
    public float yOffsetAboveGround = 0.2f;
    public int maxAttemptsPerTree = 8;
    public float minDistanceX = 5f; 

    private void Start()
    {
        if (Grounds == null || Grounds.Length == 0 || TreePrefab == null)
        {
            Debug.LogWarning("TreeSpawner: thiếu Ground hoặc TreePrefab.");
            return;
        }

        int treesPerGround = Mathf.Max(1, totalTreeCount / Grounds.Length);

        foreach (var ground in Grounds)
        {
            SpawnTreesOnGround(ground, treesPerGround);
        }
    }

    private void SpawnTreesOnGround(SpriteShapeController ground, int count)
    {
        var spline = ground.spline;
        int pointCount = spline.GetPointCount();

        if (pointCount < 2)
        {
            Debug.LogWarning($"Ground {ground.name} có quá ít point: {pointCount}");
            return;
        }

        var edge = ground.GetComponent<EdgeCollider2D>();

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 worldP = ground.transform.TransformPoint(spline.GetPosition(i));
            minX = Mathf.Min(minX, worldP.x);
            maxX = Mathf.Max(maxX, worldP.x);
        }

        List<float> usedX = new List<float>(); // 🌲 Lưu danh sách X của các cây đã spawn

        int spawned = 0;
        int safety = 0;

        while (spawned < count && safety < count * 10)
        {
            safety++;

            int idx = Random.Range(0, pointCount - 1);
            Vector3 localP1 = spline.GetPosition(idx);
            Vector3 localP2 = spline.GetPosition(idx + 1);

            float t = Random.value;
            Vector3 localPos = Vector3.Lerp(localP1, localP2, t);
            localPos.x += Random.Range(-randomXOffset, randomXOffset);
            Vector3 worldPos = ground.transform.TransformPoint(localPos);

            if (worldPos.x < minX || worldPos.x > maxX)
                continue;

            // 🔎 Kiểm tra cách X
            bool tooClose = false;
            foreach (float x in usedX)
            {
                if (Mathf.Abs(worldPos.x - x) < minDistanceX)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose) continue;

            float groundY = worldPos.y;
            float slopeAngle = 0f;

            if (edge != null)
            {
                Vector2[] edgePoints = edge.points;
                for (int j = 0; j < edgePoints.Length - 1; j++)
                {
                    Vector2 e1 = ground.transform.TransformPoint(edgePoints[j]);
                    Vector2 e2 = ground.transform.TransformPoint(edgePoints[j + 1]);

                    if (worldPos.x >= e1.x && worldPos.x <= e2.x)
                    {
                        float tt = (worldPos.x - e1.x) / (e2.x - e1.x);
                        groundY = Mathf.Lerp(e1.y, e2.y, tt);

                        Vector2 slopeDir = (e2 - e1).normalized;
                        slopeAngle = Mathf.Atan2(slopeDir.y, slopeDir.x) * Mathf.Rad2Deg;
                        break;
                    }
                }
            }

            Vector3 spawnWorldPos = new Vector3(worldPos.x, groundY + yOffsetAboveGround, 0f);
            GameObject tree = Instantiate(TreePrefab, spawnWorldPos, Quaternion.identity, ground.transform);

            float scale = Random.Range(0.8f, 1.3f);
            tree.transform.localScale = new Vector3(scale, scale, 1f);
            tree.transform.rotation = Quaternion.Euler(0f, 0f, slopeAngle + Random.Range(-5f, 5f));

            usedX.Add(worldPos.x);
            spawned++;
        }

        Debug.Log($"🌲 Spawned {spawned} trees on {ground.name}");
    }
}
