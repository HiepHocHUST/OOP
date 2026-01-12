using UnityEngine;
using Gameplay.Entities;
using Gameplay.Combat;

namespace Gameplay.Skills
{
    public class WarriorSkills : HeroSkillSet
    {
        [Header("--- CẤU HÌNH ĐẠN (SKILL Q) ---")]
        public GameObject fireballPrefab;
        public float fireballSpeed = 10f;

        [Header("--- CẤU HÌNH DIỆN RỘNG (SKILL W & E) ---")]
        public float radiusW = 3.0f;
        public float radiusE = 6.0f;
        public GameObject effectW;
        public GameObject effectE;

        private void Start()
        {
            attackSpeed = 0.8f;
            attackDelay = 0.3f;

            manaQ = 10; cooldownQ = 1.0f;
            manaW = 20; cooldownW = 4.0f;
            manaE = 60; cooldownE = 12.0f;
        }

        // ==========================================================
        // 👇 ĐÁNH THƯỜNG CÓ LOG CHI TIẾT 👇
        // ==========================================================
        public override void BasicAttack()
        {
            Debug.Log("⚔️ [4] Warrior BasicAttack: Đã được gọi! Bắt đầu xử lý...");

            // Kiểm tra Player
            if (player == null)
            {
                Debug.LogError("❌ [LỖI] Biến 'player' bị Null! Kiểm tra lại Initialize.");
                return;
            }

            // Kiểm tra Attack Point
            if (player.attackPoint == null)
            {
                Debug.LogError("🛑 [LỖI TO] Bạn chưa kéo GameObject 'AttackPoint' vào Inspector của Player!");
                return;
            }

            // Log vị trí và phạm vi quét
            Debug.Log($"🔍 Đang quét tại: {player.attackPoint.position} - Bán kính: {player.attackRange}");

            // Quét (Không dùng LayerMask)
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(player.attackPoint.position, player.attackRange);

            if (hitObjects.Length == 0)
            {
                Debug.LogWarning("⚠️ [KẾT QUẢ] Không quét trúng bất kỳ cái gì! (Check AttackRange hoặc vị trí AttackPoint)");
            }
            else
            {
                Debug.Log($"✅ [KẾT QUẢ] Quét trúng {hitObjects.Length} vật thể.");
            }

            foreach (var obj in hitObjects)
            {
                // Bỏ qua chính mình
                if (obj.gameObject == gameObject) continue;

                // Log những gì chạm vào
                // Debug.Log($"👉 Chạm: {obj.name} (Layer: {LayerMask.LayerToName(obj.gameObject.layer)})");

                Enemy enemy = obj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Debug.Log($"🩸 [HIT] Tìm thấy Enemy: {obj.name}. Gây {player.damage} sát thương!");
                    enemy.TakeDamage(player.damage);
                }
            }
        }

        // ==========================================================
        // CHIÊU Q, W, E (Giữ nguyên)
        // ==========================================================
        public override void CastSkillQ()
        {
            if (fireballPrefab == null || player.firePoint == null) return;
            GameObject fireball = Instantiate(fireballPrefab, player.firePoint.position, Quaternion.identity);
            Vector2 facingDir = player.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            if (facingDir == Vector2.left)
            {
                Vector3 scale = fireball.transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                fireball.transform.localScale = scale;
            }
            Projectile proj = fireball.GetComponent<Projectile>();
            if (proj != null)
            {
                int dmg = Mathf.RoundToInt(player.damage * 1.5f);
                proj.Setup(facingDir, dmg, fireballSpeed);
            }
        }

        public override void CastSkillW()
        {
            Debug.Log("⚔️ Warrior W: Dậm đất!");
            if (effectW != null) Instantiate(effectW, transform.position, Quaternion.identity);
            int skillDamage = Mathf.RoundToInt(player.damage * 2.0f);
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, radiusW);
            foreach (var hit in hitObjects)
            {
                if (hit.gameObject == gameObject) continue;
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null) enemy.TakeDamage(skillDamage);
            }
        }

        public override void CastSkillE()
        {
            Debug.Log("😡 Warrior E: ĐỊA CHẤN!");
            if (effectE != null) Instantiate(effectE, transform.position, Quaternion.identity);
            int skillDamage = Mathf.RoundToInt(player.damage * 4.0f);
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, radiusE);
            foreach (var hit in hitObjects)
            {
                if (hit.gameObject == gameObject) continue;
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null) enemy.TakeDamage(skillDamage);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radiusW);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radiusE);
        }
    }
}