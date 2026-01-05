using UnityEngine;
using Gameplay.Entities;
using Gameplay.Combat;

namespace Gameplay.Skills
{
    public class ArcherSkills : HeroSkillSet
    {
        [Header("Archer Settings")]
        // 1. Thêm biến tốc độ riêng cho Cung Thủ
        public float arrowSpeed = 18f;

        public override void BasicAttack()
        {
            // Debug.Log("🏹 Archer bắn tên!");

            if (player.projectilePrefab == null || player.firePoint == null) return;

            // 1. Tạo mũi tên
            GameObject arrow = Instantiate(player.projectilePrefab, player.firePoint.position, Quaternion.identity);

            // 2. Xác định hướng bắn
            Vector2 direction = player.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            // 3. Setup (SỬA LẠI ĐỂ TRUYỀN ĐỦ 3 THAM SỐ) 👇
            Projectile proj = arrow.GetComponent<Projectile>();
            if (proj != null)
            {
                // Truyền: Hướng, Sát thương, Tốc độ (arrowSpeed)
                proj.Setup(direction, player.damage, arrowSpeed);
            }
        }

        public override void CastSkillQ()
        {
            base.CastSkillQ();
            // Nếu bạn có code skill Q ở đây, nhớ cũng thêm arrowSpeed vào hàm Setup tương tự nhé!
        }
    }
}