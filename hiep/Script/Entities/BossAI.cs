using UnityEngine;
using Gameplay.Entities;

public class BossAI : MonoBehaviour
{
    private Transform target;
    private Enemy enemyBody;
    private int currentLevel;

    [Header("Chỉ số AI")]
    public float moveSpeed = 4f;
    public float jumpForce = 10f;
    private float nextJumpTime = 0f;

    void Start()
    {
        enemyBody = GetComponent<Enemy>();

        // Lấy Level hiện tại để quyết định độ khôn
        // (Giả sử bạn có biến GameManager.CurrentMapLevel như bài trước)
        currentLevel = GameManager.CurrentMapLevel;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) target = playerObj.transform;

        Debug.Log($"🤖 Boss khởi động AI cấp độ: {currentLevel}");
    }

    void FixedUpdate()
    {
        if (target == null || enemyBody == null) return;

        // --- CẤP ĐỘ 1-5: AI NGU NGƠ (Chỉ biết chạy theo) ---
        if (currentLevel <= 5)
        {
            SimpleChase();
        }
        // --- CẤP ĐỘ 6+: AI BIẾT NHẢY (Vượt chướng ngại vật) ---
        else
        {
            SmartChase();
        }
    }

    void SimpleChase()
    {
        float direction = Mathf.Sign(target.position.x - transform.position.x);
        enemyBody.rb.linearVelocity = new Vector2(direction * moveSpeed, enemyBody.rb.linearVelocity.y);

        // Quay mặt
        enemyBody.spriteRenderer.flipX = (direction < 0);
    }

    void SmartChase()
    {
        // 1. Vẫn đuổi theo nhưng nhanh hơn tí
        float smartSpeed = moveSpeed * 1.5f;
        float direction = Mathf.Sign(target.position.x - transform.position.x);
        enemyBody.rb.linearVelocity = new Vector2(direction * smartSpeed, enemyBody.rb.linearVelocity.y);
        enemyBody.spriteRenderer.flipX = (direction < 0);

        // 2. Kỹ năng nhảy: Nếu Player ở cao hơn -> Tự nhảy lên
        if (target.position.y > transform.position.y + 2f && Time.time > nextJumpTime)
        {
            Debug.Log("🦘 Boss nhảy lên bắt Player!");
            enemyBody.rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            nextJumpTime = Time.time + 2f; // Nhảy mỗi 2 giây thôi ko nó nhảy liên tục
        }

        // (Bạn có thể thêm: Nếu Level > 10 thì Boss biết đi lùi khi Player đánh...)
    }
}