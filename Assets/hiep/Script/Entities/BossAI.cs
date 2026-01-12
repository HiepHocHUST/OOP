using UnityEngine;
using Gameplay.Entities; // Dùng namespace chứa Enemy
using Core;

public class BossAI : MonoBehaviour
{
    [Header("--- CẤU HÌNH AI (GIỮ NGUYÊN) ---")]
    public float moveSpeed = 4f;
    public float flySpeed = 5f;
    public float jumpForce = 12f;
    public int levelToStartFlying = 10; // Map 10 mới biết bay

    [Header("--- CHIẾN ĐẤU ---")]
    public float attackRange = 8f;
    public float attackCooldown = 3f;
    public GameObject fireballPrefab;
    public Transform firePoint;
    public LayerMask groundLayer;

    // Các biến nội bộ
    private Transform target;
    private Enemy enemyBody;
    private Animator anim;
    private Collider2D bossCollider;
    private int currentLevel;

    private bool isAttacking = false;
    private float nextAttackTime = 0f;
    private float jumpCooldown = 0f;

    void Awake()
    {
        // Lấy các component cần thiết
        enemyBody = GetComponent<Enemy>();
        anim = GetComponent<Animator>();
        bossCollider = GetComponent<Collider2D>();

        // Mặc định tắt trọng lực lúc mới sinh để tránh rơi tự do nếu ở trên trời
        if (enemyBody != null && enemyBody.rb != null)
        {
            enemyBody.rb.gravityScale = 0;
        }
    }

    void Start()
    {
        // 1. Xác định Level hiện tại
        currentLevel = GameManager.CurrentMapLevel;
        if (currentLevel < 1) currentLevel = 1;

        FindPlayer();

        // 2. KẾT NỐI DATABASE (Phần code mới)
        // Lấy chỉ số từ DB nạp vào, thay vì dùng biến cứng
        HistoryManager db = FindObjectOfType<HistoryManager>();
        if (db != null && enemyBody != null && enemyBody.enemyID != 0)
        {
            var data = db.GetEnemyStats(enemyBody.enemyID);
            if (data != null)
            {
                // Công thức Boss: Mạnh thêm 50% mỗi màn
                float growth = 1f + ((currentLevel - 1) * 0.5f);
                int finalHp = Mathf.RoundToInt(data.hp * growth);
                int finalDmg = Mathf.RoundToInt(data.dmg * growth);

                // Nạp dữ liệu
                enemyBody.isBoss = data.isBoss;
                string bossName = data.name + " (Lv." + currentLevel + ")";
                enemyBody.SetupData(bossName, finalHp, finalDmg, data.exp * currentLevel, data.minG * currentLevel, data.maxG * currentLevel);

                // Chỉnh kích thước theo DB
                transform.localScale = Vector3.one * data.scale;

                Debug.Log($"🔥 BOSS {bossName} đã hồi sinh với {finalHp} Máu!");
            }
        }

        // 3. Cài đặt trọng lực (Logic cũ)
        SetupGravity();
    }

    void SetupGravity()
    {
        if (enemyBody == null || enemyBody.rb == null) return;

        // Nếu level cao thì bay (gravity = 0), thấp thì đi bộ (gravity = 1)
        if (currentLevel >= levelToStartFlying)
        {
            enemyBody.rb.gravityScale = 0;
        }
        else
        {
            enemyBody.rb.gravityScale = 1;
        }
    }

    void Update()
    {
        if (target == null) FindPlayer();
        if (jumpCooldown > 0) jumpCooldown -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (target == null || enemyBody == null) return;

        // Nếu đang tấn công thì đứng yên để múa skill
        if (isAttacking)
        {
            if (enemyBody.rb != null) enemyBody.rb.linearVelocity = Vector2.zero;
            return;
        }

        // Kiểm tra khoảng cách để tấn công
        float distance = Vector2.Distance(transform.position, target.position);

        // Logic tấn công (Chỉ đánh khi đủ gần và hết cooldown)
        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            // Kiểm tra xem có đang đứng trên đất không (hoặc đang bay)
            bool isGrounded = CheckIsGrounded();
            if (currentLevel >= levelToStartFlying || isGrounded)
            {
                StartAttackSequence();
                return; // Ngắt di chuyển để tấn công
            }
        }

        // Logic Di chuyển (Khôi phục lại như cũ)
        if (currentLevel >= levelToStartFlying)
            FlyToPlayer();
        else
            WalkToPlayer();
    }

    // --- CÁC HÀM DI CHUYỂN (ĐÃ KHÔI PHỤC) ---

    void FlyToPlayer()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        enemyBody.rb.AddForce(direction * flySpeed * 10f); // Dùng AddForce cho mượt

        // Giới hạn tốc độ
        if (enemyBody.rb.linearVelocity.magnitude > flySpeed)
            enemyBody.rb.linearVelocity = enemyBody.rb.linearVelocity.normalized * flySpeed;

        FlipFace(target.position.x - transform.position.x);
    }

    void WalkToPlayer()
    {
        // Đi bộ hướng về phía Player
        float directionX = Mathf.Sign(target.position.x - transform.position.x);
        enemyBody.rb.linearVelocity = new Vector2(directionX * moveSpeed, enemyBody.rb.linearVelocity.y);
        FlipFace(directionX);

        // Gặp tường thì nhảy
        if (CheckWallAhead()) ThucHienNhay();
    }

    void ThucHienNhay()
    {
        enemyBody.rb.linearVelocity = new Vector2(enemyBody.rb.linearVelocity.x, 0);
        enemyBody.rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpCooldown = 1.5f;
    }

    // --- CÁC HÀM TẤN CÔNG (ĐÃ KHÔI PHỤC ĐỂ FIX LỖI ANIMATION) ---

    void StartAttackSequence()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        // Quay mặt về hướng bắn
        FlipFace(target.position.x - transform.position.x);

        if (anim != null)
        {
            anim.SetTrigger("Attack"); // Gọi Animation
        }
        else
        {
            // Nếu không có Animation thì bắn luôn (fallback)
            SpawnFireball();
            EndAttack();
        }
    }

    // HÀM NÀY ĐƯỢC ANIMATION GỌI (Animation Event)
    public void SpawnFireball()
    {
        if (firePoint == null || fireballPrefab == null) return;
        if (target == null) return;

        GameObject ball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

        // Tính hướng bắn chính xác vào Player
        Vector2 dir = (target.position - firePoint.position).normalized;

        // Thêm chút ngẫu nhiên cho ảo (nếu muốn)
        // dir = Quaternion.Euler(0, 0, Random.Range(-5f, 5f)) * dir;

        // Xoay viên đạn theo hướng bắn
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        ball.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Gửi hướng bắn vào script FireballController (nếu có)
        // Lưu ý: Bạn cần có script FireballController gắn trên Prefab viên đạn nhé
        var fb = ball.GetComponent<FireballController>();
        if (fb != null) fb.SetDirection(dir);
    }

    // HÀM NÀY ĐƯỢC ANIMATION GỌI KHI MÚA XONG
    public void EndAttack()
    {
        isAttacking = false;
    }

    // --- CÁC HÀM PHỤ TRỢ ---

    void FlipFace(float direction)
    {
        // Lật hình Boss trái/phải
        float currentScaleX = Mathf.Abs(transform.localScale.x);
        if (direction > 0) transform.localScale = new Vector3(currentScaleX, transform.localScale.y, transform.localScale.z);
        else transform.localScale = new Vector3(-currentScaleX, transform.localScale.y, transform.localScale.z);
    }

    bool CheckWallAhead()
    {
        if (jumpCooldown > 0) return false;
        float dir = Mathf.Sign(transform.localScale.x);
        Vector2 origin = (Vector2)transform.position + new Vector2(dir * 0.7f, 0.8f); // Bắn Raycast phía trước mặt
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * dir, 1.5f, groundLayer);
        return hit.collider != null;
    }

    bool CheckIsGrounded()
    {
        if (bossCollider == null) return true;
        float extraHeight = 0.2f;
        Bounds bounds = bossCollider.bounds;
        RaycastHit2D hit = Physics2D.BoxCast(bounds.center, bounds.size, 0f, Vector2.down, extraHeight, groundLayer);
        return hit.collider != null;
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) target = playerObj.transform;
    }
}