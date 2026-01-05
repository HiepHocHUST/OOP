using UnityEngine;
using Gameplay.Entities;
using Gameplay.Combat;

namespace Gameplay.Skills
{
    // 👇 QUAN TRỌNG: Tên class phải là PaladinSkills (trùng tên file)
    public class PaladinSkills : HeroSkillSet
    {
        [Header("Paladin Settings")]
        public GameObject hammerPrefab; // Kéo Prefab cái búa (hoặc đạn) vào đây
        public float hammerSpeed = 8f;  // Tốc độ bay chậm nhưng chắc

        // Paladin đánh thường là cận chiến (Giống Warrior)
        public override void BasicAttack()
        {
            if (player == null || player.attackPoint == null) return;
            // Debug.Log("⚔️ Paladin đập búa!");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.attackPoint.position, player.attackRange, player.enemyLayers);
            int finalDmg = player.damage;

            foreach (var enemy in hitEnemies)
            {
                if (enemy != null) enemy.GetComponent<Enemy>()?.TakeDamage(finalDmg);
            }
        }

        // Skill Q: Ném búa thần (Bắn đạn)
        public override void CastSkillQ()
        {
            Debug.Log("✨ Paladin: NÉM BÚA!");

            if (hammerPrefab == null || player.castPoint == null)
            {
                Debug.LogWarning("❌ Chưa kéo Prefab Búa vào PaladinSkills!");
                return;
            }

            // 1. Tạo búa
            GameObject hammer = Instantiate(hammerPrefab, player.castPoint.position, Quaternion.identity);

            // 2. Hướng bắn
            Vector2 direction = player.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            // 3. Setup thông số (Dùng hàm Setup 3 tham số mới)
            Projectile proj = hammer.GetComponent<Projectile>();
            if (proj != null)
            {
                int dmg = Mathf.RoundToInt(player.damage * 1.2f);
                // Truyền: Hướng, Dame, Tốc độ
                proj.Setup(direction, dmg, hammerSpeed);
            }
        }
    }
}