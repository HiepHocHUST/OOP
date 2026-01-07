using UnityEngine;
using Gameplay.Entities;
using Gameplay.Combat;

namespace Gameplay.Skills
{
    public class AssassinSkills : HeroSkillSet
    {
        [Header("--- CÀI ĐẶT RIÊNG CHO ASSASSIN ---")]
        public GameObject projectilePrefab; // Kéo Prefab Dao vào đây
        public float damageMultiplier = 1.5f;
        public float fireballSpeed = 15f;

        [Header("--- TẦM ĐÁNH SKILL (Chỉnh ở đây) ---")]
        public float radiusW = 2.5f; // Phạm vi quét chiêu W
        public float radiusE = 5.0f; // Phạm vi quét chiêu E (Rộng hơn)
        public GameObject effectW;   // Kéo VFX Lướt (nếu có)
        public GameObject effectE;   // Kéo VFX Nổ (nếu có)

        private void Start()
        {
            // Cấu hình Mana/Cooldown
            manaQ = 10; cooldownQ = 0.5f;
            manaW = 25; cooldownW = 2.0f; // Giảm hồi chiêu W chút cho sướng tay
            manaE = 80; cooldownE = 10.0f; // Giảm hồi chiêu E để test cho nhanh
        }

        // --- HÀM ĐÁNH THƯỜNG (Giữ nguyên của bạn) ---
        public override void BasicAttack()
        {
            if (player == null || player.attackPoint == null) return;
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.attackPoint.position, player.attackRange, player.enemyLayers);
            foreach (var enemy in hitEnemies)
            {
                if (enemy != null) enemy.GetComponent<Enemy>()?.TakeDamage(player.damage);
            }
        }

        // --- CHIÊU Q (Giữ nguyên của bạn) ---
        public override void CastSkillQ()
        {
            if (player.firePoint == null || projectilePrefab == null) return;
            GameObject spell = Instantiate(projectilePrefab, player.firePoint.position, Quaternion.identity);
            Vector2 direction = player.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            if (direction == Vector2.left)
            {
                Vector3 scale = spell.transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                spell.transform.localScale = scale;
            }

            Projectile projScript = spell.GetComponent<Projectile>();
            if (projScript != null)
            {
                int finalDamage = Mathf.RoundToInt(player.damage * damageMultiplier);
                projScript.Setup(direction, finalDamage, fireballSpeed);
            }
        }

        // ==========================================================
        // CHIÊU W - (ĐÃ THÊM LOGIC SÁT THƯƠNG)
        // ==========================================================
        public override void CastSkillW()
        {
            Debug.Log("⚡ Assassin W: Quét kiếm!");

            // 1. Tạo hiệu ứng (nếu có)
            if (effectW != null) Instantiate(effectW, transform.position, Quaternion.identity);

            // 2. Tìm quái xung quanh người (dùng radiusW)
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radiusW, player.enemyLayers);

            // 3. Tính dame (Mạnh gấp 2 lần cơ bản)
            int skillDamage = Mathf.RoundToInt(player.damage * 2.0f);

            // 4. Trừ máu
            foreach (var hit in hitEnemies)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(skillDamage);
                    Debug.Log($"-> Chém trúng W vào: {hit.name}");
                }
            }
        }

        // ==========================================================
        // CHIÊU E - (ĐÃ THÊM LOGIC SÁT THƯƠNG)
        // ==========================================================
        public override void CastSkillE()
        {
            Debug.Log("☠️ Assassin E: Sát thủ tối thượng!");

            if (effectE != null) Instantiate(effectE, transform.position, Quaternion.identity);

            // Tìm quái vùng rộng (radiusE)
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radiusE, player.enemyLayers);

            // Dame cực to (Gấp 5 lần)
            int skillDamage = Mathf.RoundToInt(player.damage * 5.0f);

            foreach (var hit in hitEnemies)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(skillDamage);
                    Debug.Log($"-> Nổ E chết: {hit.name}");
                }
            }
        }

        // Vẽ vòng tròn để căn chỉnh trong Scene
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radiusW);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radiusE);
        }
    }
}