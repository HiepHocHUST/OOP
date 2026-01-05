using UnityEngine;
using Gameplay.Entities;

public class SlimeAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Transform target;
    private Enemy enemyBody;

    void Start()
    {
        enemyBody = GetComponent<Enemy>();
        if (enemyBody == null)
            Debug.LogError("❌ Thiếu script Enemy trên con " + gameObject.name);

        // Lưu ý: Không tìm Player ở đây nữa vì có thể Player chưa sinh ra kịp
    }

    void FixedUpdate()
    {
        // 1. Nếu chưa có mục tiêu -> Đi tìm ngay!
        if (target == null)
        {
            FindPlayer();
            return; // Tìm chưa thấy thì đứng yên, chưa chạy vội
        }

        // 2. Nếu thiếu cơ thể -> Nghỉ
        if (enemyBody == null) return;

        // 3. Logic di chuyển (Khi đã có target)
        float direction = Mathf.Sign(target.position.x - transform.position.x);
        enemyBody.rb.linearVelocity = new Vector2(direction * moveSpeed, enemyBody.rb.linearVelocity.y);

        if (enemyBody.spriteRenderer != null)
            enemyBody.spriteRenderer.flipX = (direction < 0);
    }

    // Hàm riêng để tìm Player
    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
            Debug.Log("👀 A ha! Slime đã thấy Player rồi: " + playerObj.name);
        }
    }
}