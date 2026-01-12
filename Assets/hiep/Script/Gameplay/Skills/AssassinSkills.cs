using UnityEngine;
using Gameplay.Entities;
using Gameplay.Combat;

namespace Gameplay.Skills
{
    public class AssassinSkills : HeroSkillSet
    {
        [Header("--- CẤU HÌNH RIÊNG ---")]
        public GameObject projectilePrefab;
        public float damageMultiplier = 1.5f;
        public float fireballSpeed = 15f;

        public float radiusW = 2.5f;
        public float radiusE = 5.0f;
        public GameObject effectW;
        public GameObject effectE;

        private void Start()
        {
            // Assassin đánh nhanh, delay thấp
            attackSpeed = 0.4f;
            attackDelay = 0.1f; // Gần như gây dame ngay lập tức khi bấm nút

            manaQ = 10; cooldownQ = 0.5f;
            manaW = 25; cooldownW = 2.0f;
            manaE = 40; cooldownE = 10.0f;
        }

        // ==========================================================
        // 👇 ĐÁNH THƯỜNG (BASIC ATTACK) - PHIÊN BẢN CHẮC CHẮN TRÚNG 👇
        // ==========================================================
        public override void BasicAttack()
        {
            if (player == null || player.attackPoint == null) return;

            // Quét tất cả, không cần LayerMask
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(player.attackPoint.position, player.attackRange);

            bool hitSomething = false;
            foreach (var obj in hitObjects)
            {
                if (obj.gameObject == gameObject) continue;

                Enemy enemy = obj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(player.damage);
                    hitSomething = true;
                    Debug.Log($"🗡️ Assassin chém thường trúng: {enemy.name}");
                }
            }

            if (!hitSomething && hitObjects.Length == 0)
            {
                // Debug.Log("💨 Assassin chém gió...");
            }
        }

        // ==========================================================
        // CHIÊU Q, W, E (Logic tương tự Warrior)
        // ==========================================================
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

        public override void CastSkillW()
        {
            Debug.Log("⚡ Assassin W: Quét kiếm!");
            if (effectW != null) Instantiate(effectW, transform.position, Quaternion.identity);

            int skillDamage = Mathf.RoundToInt(player.damage * 2.0f);

            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, radiusW);

            foreach (var hit in hitObjects)
            {
                if (hit.gameObject == gameObject) continue;
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(skillDamage);
                    Debug.Log($"✅ [Assassin] Chém W trúng: {hit.name}");
                }
            }
        }

        public override void CastSkillE()
        {
            Debug.Log("☠️ Assassin E: Sát thủ tối thượng!");
            if (effectE != null) Instantiate(effectE, transform.position, Quaternion.identity);

            int skillDamage = Mathf.RoundToInt(player.damage * 5.0f);

            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, radiusE);

            foreach (var hit in hitObjects)
            {
                if (hit.gameObject == gameObject) continue;
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(skillDamage);
                    Debug.Log($"✅ [Assassin] Nổ E trúng: {hit.name}");
                }
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