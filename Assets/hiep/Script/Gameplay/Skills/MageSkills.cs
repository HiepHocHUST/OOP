using UnityEngine;
using Gameplay.Entities;
using Gameplay.Combat;

namespace Gameplay.Skills
{
    public class MageSkills : HeroSkillSet
    {
        [Header("Mage Settings")]
        // Thêm biến tốc độ cho đạn của Mage
        public float projectileSpeed = 12f;

        // Mage đánh thường là bắn xa
        public override void BasicAttack()
        {
            Debug.Log("🔮 Mage bắn thường!");

            // Kiểm tra an toàn
            if (player.projectilePrefab == null || player.firePoint == null)
            {
                Debug.LogWarning("Mage thiếu Projectile Prefab hoặc FirePoint!");
                return;
            }

            // 1. Tạo đạn (Lấy từ Player hoặc biến riêng đều được, ở đây dùng Player cho tiện)
            GameObject spell = Instantiate(player.projectilePrefab, player.firePoint.position, Quaternion.identity);

            // 2. Hướng bắn
            Vector2 direction = player.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            // 3. Setup (THÊM THAM SỐ TỐC ĐỘ VÀO CUỐI) 👇
            Projectile proj = spell.GetComponent<Projectile>();
            if (proj != null)
            {
                // Truyền: Hướng, Dame, Tốc độ
                proj.Setup(direction, player.damage, projectileSpeed);
            }
        }

        public override void CastSkillQ()
        {
            // Nếu bạn có code skill Q cho Mage, nhớ cũng thêm speed vào hàm Setup tương tự nhé!
            base.CastSkillQ();
        }
    }
}