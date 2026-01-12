using UnityEngine;
using System.Collections;
using Core;

namespace Gameplay.Entities
{
    public class Enemy : Unit
    {
        [Header("--- KẾT NỐI DATABASE ---")]
        public int enemyID; // ⚠️ QUAN TRỌNG: ĐIỀN ID (101, 1001...) VÀO ĐÂY

        // Các biến này sẽ được nạp từ DB, ẩn đi cho đỡ rối Inspector
        [HideInInspector] public bool isBoss = false;
        [HideInInspector] public int expReward;
        [HideInInspector] public int goldDropMin;
        [HideInInspector] public int goldDropMax;

        [HideInInspector] public Rigidbody2D rb;
        [HideInInspector] public SpriteRenderer spriteRenderer;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Hàm nhận dữ liệu từ AI
        public void SetupData(string name, int hp, int dmg, int exp, int minGold, int maxGold)
        {
            unitName = name;
            maxHp = hp;
            currentHp = hp; // Hồi đầy máu
            damage = dmg;
            expReward = exp;
            goldDropMin = minGold;
            goldDropMax = maxGold;

            Debug.Log($"✅ [DB LOAD] {unitName} (ID:{enemyID}) - HP: {maxHp}");
        }

        public override void TakeDamage(int dmg)
        {
            if (maxHp <= 0) return;
            currentHp -= dmg;
            if (spriteRenderer != null) StartCoroutine(FlashRed());
            if (currentHp <= 0) Die();
        }

        protected override void Die()
        {
            // Tính vàng rơi ngẫu nhiên
            int finalGold = Random.Range(goldDropMin, goldDropMax + 1);
            if (GameManager.Instance != null) GameManager.Instance.AddGold(finalGold);

            // Nếu là Boss -> Thắng game
            if (isBoss)
            {
                Debug.Log("🔥 BOSS DIED! VICTORY!");
                if (GameManager.Instance != null) GameManager.Instance.Victory();
            }

            Destroy(gameObject);
        }

        IEnumerator FlashRed()
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }

        public override void CalculateStats() { }
    }
}