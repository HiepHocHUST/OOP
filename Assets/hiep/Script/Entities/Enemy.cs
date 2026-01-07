using UnityEngine;

namespace Gameplay.Entities
{
    public class Enemy : Unit
    {
        [Header("Loot Info")]
        public int expReward;
        public int goldDropMin;
        public int goldDropMax;

        // Cho phép các script AI bên ngoài điều khiển cơ thể này
        [HideInInspector] public Rigidbody2D rb;
        [HideInInspector] public SpriteRenderer spriteRenderer;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetupData(string name, int hp, int dmg, int exp, int minGold, int maxGold)
        {
            unitName = name;
            maxHp = hp;
            damage = dmg;
            currentHp = maxHp;
            expReward = exp;
            goldDropMin = minGold;
            goldDropMax = maxGold;

            // Không tự tìm target ở đây nữa, để AI lo
        }

        // Xử lý va chạm gây damage (Giữ nguyên)
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Player player = collision.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    // Đẩy lùi Player
                    Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        Vector2 pushDir = (player.transform.position - transform.position).normalized;
                        playerRb.AddForce(pushDir * 5f, ForceMode2D.Impulse);
                    }
                    // Trừ máu
                    player.TakeDamage(this.damage);
                }
            }
        }

        protected override void Die()
        {
            // Báo cáo chết (như bài trước)
            if (LevelManager.Instance != null) LevelManager.Instance.QuaiChet();

            int gold = Random.Range(goldDropMin, goldDropMax);
            Debug.Log($"☠️ {unitName} chết! Rớt {gold} Vàng.");
            Destroy(gameObject);
        }

        public override void CalculateStats() { }
    }
}