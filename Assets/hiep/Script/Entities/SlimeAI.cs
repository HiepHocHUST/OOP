using UnityEngine;
using Gameplay.Entities;
using Core;

public class SlimeAI : MonoBehaviour
{
    [Header("--- HÀNH VI & DI CHUYỂN ---")]
    public int levelToStartChasing = 2;
    public int levelToStartFlying = 5;
    public float moveSpeed = 3f;
    public float flySpeed = 4f;
    public float jumpForce = 12f;
    public LayerMask groundLayer;

    private Transform target;
    private Enemy enemyBody;
    private int currentLevel;

    void Start()
    {
        enemyBody = GetComponent<Enemy>();
        currentLevel = Mathf.Max(1, GameManager.CurrentMapLevel); // Đảm bảo level tối thiểu là 1

        // --- ĐOẠN CODE KẾT NỐI DB ---
        HistoryManager db = FindObjectOfType<HistoryManager>();

        // Chỉ chạy nếu có DB và có ID trong Enemy
        if (db != null && enemyBody != null && enemyBody.enemyID != 0)
        {
            var data = db.GetEnemyStats(enemyBody.enemyID);
            if (data != null)
            {
                // Công thức: Mỗi màn chơi quái mạnh thêm 20%
                float growth = 1f + ((currentLevel - 1) * 0.2f);

                int finalHp = Mathf.RoundToInt(data.hp * growth);
                int finalDmg = Mathf.RoundToInt(data.dmg * growth);

                // Nạp vào Enemy
                enemyBody.isBoss = data.isBoss;
                enemyBody.SetupData(data.name, finalHp, finalDmg, data.exp * currentLevel, data.minG * currentLevel, data.maxG * currentLevel);

                // Chỉnh kích thước theo DB
                transform.localScale = Vector3.one * data.scale;
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ {gameObject.name} chưa điền EnemyID hoặc không tìm thấy HistoryManager!");
        }

        SetupGravity();
        FindPlayer();
    }

    void SetupGravity()
    {
        if (enemyBody.rb != null)
            enemyBody.rb.gravityScale = (currentLevel >= levelToStartFlying) ? 0 : 1;
    }

    void FixedUpdate()
    {
        if (target == null) { FindPlayer(); return; }

        if (currentLevel >= levelToStartFlying) Fly();
        else if (currentLevel >= levelToStartChasing) Walk();
    }

    void Fly()
    {
        Vector2 dir = (target.position - transform.position).normalized;
        enemyBody.rb.linearVelocity = dir * flySpeed;
        enemyBody.spriteRenderer.flipX = dir.x < 0;
    }

    void Walk()
    {
        float dirX = Mathf.Sign(target.position.x - transform.position.x);
        enemyBody.rb.linearVelocity = new Vector2(dirX * moveSpeed, enemyBody.rb.linearVelocity.y);
        enemyBody.spriteRenderer.flipX = dirX < 0;
    }

    void FindPlayer() { GameObject p = GameObject.FindGameObjectWithTag("Player"); if (p) target = p.transform; }
}