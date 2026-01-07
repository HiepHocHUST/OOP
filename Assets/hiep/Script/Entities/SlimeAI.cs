using UnityEngine;
using Gameplay.Entities;

public class SlimeAI : MonoBehaviour
{
    [Header("--- CẤU HÌNH CƠ BẢN ---")]
    public float moveSpeed = 3f;
    private Transform target;
    private Enemy enemyBody;

    [Header("--- CHẾ ĐỘ THÔNG MINH (CHO MÀN KHÓ) ---")]
    public bool enableJumping = true; // Bật cái này lên thì quái mới biết nhảy
    public float jumpForce = 7f;      // Lực nhảy cao bao nhiêu
    public float obstacleCheckDist = 1.0f; // Khoảng cách nhìn thấy vật cản
    public LayerMask groundLayer;     // Lớp đất/đá (để nó biết cái gì cần nhảy qua)

    [Header("--- Cảm Biến ---")]
    public Transform wallCheckPoint;  // Vị trí con mắt (đặt ở ngang bụng/đầu gối quái)
    public Transform groundCheckPoint; // Vị trí bàn chân (để biết đang đứng dưới đất)
    private bool isGrounded;

    void Start()
    {
        enemyBody = GetComponent<Enemy>();
        if (enemyBody == null) Debug.LogError("❌ Thiếu script Enemy!");
    }

    void FixedUpdate()
    {
        // 1. Tìm Player nếu chưa có
        if (target == null)
        {
            FindPlayer();
            return;
        }
        if (enemyBody == null) return;

        // 2. Logic di chuyển
        float direction = Mathf.Sign(target.position.x - transform.position.x);

        // Giữ nguyên vận tốc Y (để rơi tự do), chỉ thay đổi X
        enemyBody.rb.linearVelocity = new Vector2(direction * moveSpeed, enemyBody.rb.linearVelocity.y);

        // Quay mặt
        if (enemyBody.spriteRenderer != null)
            enemyBody.spriteRenderer.flipX = (direction < 0);

        // 3. LOGIC NHẢY (CHỈ DÀNH CHO QUÁI THÔNG MINH)
        if (enableJumping)
        {
            CheckObstacleAndJump(direction);
        }
    }

    void CheckObstacleAndJump(float moveDir)
    {
        // A. Kiểm tra xem có đang đứng dưới đất không? (Không được nhảy 2 bước trên không)
        // Tạo một vòng tròn nhỏ ở chân để check đất
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, 0.2f, groundLayer);

        if (!isGrounded) return; // Đang bay thì thôi, không xử lý tiếp

        // B. Bắn tia laser (Raycast) ra phía trước mặt để tìm cục đá
        Vector2 rayOrigin = wallCheckPoint.position;
        Vector2 rayDir = (moveDir > 0) ? Vector2.right : Vector2.left; // Hướng bắn theo hướng di chuyển

        RaycastHit2D hitInfo = Physics2D.Raycast(rayOrigin, rayDir, obstacleCheckDist, groundLayer);

        // Vẽ tia ra màn hình để bạn dễ chỉnh (Màu đỏ: chạm tường, Màu xanh: không chạm)
        Debug.DrawRay(rayOrigin, rayDir * obstacleCheckDist, hitInfo.collider ? Color.red : Color.green);

        // C. Nếu tia laser chạm vào Đất/Đá -> NHẢY!
        if (hitInfo.collider != null)
        {
            Debug.Log("🧱 Thấy cục đá! Nhảy thôi!");
            enemyBody.rb.linearVelocity = new Vector2(enemyBody.rb.linearVelocity.x, jumpForce);
        }
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) target = playerObj.transform;
    }

    // Hàm hỗ trợ vẽ Gizmos trong Editor để dễ set điểm check
    void OnDrawGizmos()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckPoint.position, 0.2f);
        }
    }
}