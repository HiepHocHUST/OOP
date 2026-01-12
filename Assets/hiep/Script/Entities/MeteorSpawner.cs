using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    [Header("--- CẤU HÌNH ---")]
    public GameObject meteorPrefab;   // Kéo Prefab Thiên thạch vào đây
    public float spawnRate = 0.5f;    // Tốc độ rơi (0.5 giây rớt 1 cục)
    public float spawnHeight = 10f;   // Độ cao sinh ra (so với Spawner)

    [Header("--- PHẠM VI ---")]
    public float minX = -10f;         // Điểm bên trái map
    public float maxX = 10f;          // Điểm bên phải map

    private float nextSpawnTime = 0f;

    void Update()
    {
        // Kiểm tra thời gian để thả cục tiếp theo
        if (Time.time >= nextSpawnTime)
        {
            SpawnMeteor();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnMeteor()
    {
        if (meteorPrefab == null) return;

        // 1. Chọn vị trí ngẫu nhiên theo trục X
        float randomX = Random.Range(minX, maxX);

        // 2. Vị trí sinh ra: X ngẫu nhiên, Y lấy theo vị trí Spawner
        Vector2 spawnPos = new Vector2(randomX, transform.position.y);

        // 3. Tạo ra thiên thạch
        Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
    }

    // Vẽ cái hộp trên màn hình Scene để dễ căn chỉnh phạm vi
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Vẽ đường kẻ ngang thể hiện phạm vi thả bom
        Gizmos.DrawLine(new Vector3(minX, transform.position.y, 0), new Vector3(maxX, transform.position.y, 0));
    }
}