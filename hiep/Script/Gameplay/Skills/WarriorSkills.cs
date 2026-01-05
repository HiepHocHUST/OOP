using UnityEngine;
using Gameplay.Entities;
using Gameplay.Combat;

namespace Gameplay.Skills
{
    public class WarriorSkills : HeroSkillSet
    {
        [Header("--- CẤU HÌNH ĐẠN (SKILL Q) ---")]
        public GameObject fireballPrefab; // Kéo Prefab Cầu Lửa
        public float fireballSpeed = 10f;

        [Header("--- CẤU HÌNH DIỆN RỘNG (SKILL W & E) ---")]
        public float radiusW = 3.0f;  // Phạm vi chiêu W
        public float radiusE = 6.0f;  // Phạm vi chiêu E (Rất rộng)
        public GameObject effectW;    // Kéo VFX Dậm đất/Xoay kiếm (nếu có)
        public GameObject effectE;    // Kéo VFX Nổ lớn (nếu có)

        private void Start()
        {
            // 1. CẤU HÌNH MANA VÀ HỒI CHIÊU
            manaQ = 10; cooldownQ = 1.0f;
            manaW = 20; cooldownW = 4.0f;  // Hồi chiêu trung bình
            manaE = 60; cooldownE = 12.0f; // Chiêu cuối hồi lâu
        }

        // --- TEST NHANH (Xóa sau khi gắn Animation Event xong) ---
        private void Update()
        {
            // Nếu lười gắn Event, bấm T và Y để test damge luôn
            if (Input.GetKeyDown(KeyCode.T)) CastSkillW();
            if (Input.GetKeyDown(KeyCode.Y)) CastSkillE();
        }

        // --- ĐÁNH THƯỜNG ---
        public override void BasicAttack()
        {
            if (player == null || player.attackPoint == null) return;
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.attackPoint.position, player.attackRange, player.enemyLayers);
            foreach (var enemy in hitEnemies)
            {
                if (enemy != null) enemy.GetComponent<Enemy>()?.TakeDamage(player.damage);
            }
        }

        // ==========================================================
        // CHIÊU Q - CẦU LỬA (Bắn xa)
        // ==========================================================
        public override void CastSkillQ()
        {
            // Kiểm tra null để tránh lỗi đỏ lòm
            if (fireballPrefab == null || player.firePoint == null) return;

            // Tạo đạn
            GameObject fireball = Instantiate(fireballPrefab, player.firePoint.position, Quaternion.identity);

            // Xác định hướng (Trái/Phải)
            Vector2 facingDir = player.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            // Xoay đầu đạn nếu bắn sang trái
            if (facingDir == Vector2.left)
            {
                Vector3 scale = fireball.transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                fireball.transform.localScale = scale;
            }

            // Setup đạn
            Projectile proj = fireball.GetComponent<Projectile>();
            if (proj != null)
            {
                int dmg = Mathf.RoundToInt(player.damage * 1.5f); // Dame 1.5 lần
                proj.Setup(facingDir, dmg, fireballSpeed);
            }
        }

        // ==========================================================
        // CHIÊU W - DẬM ĐẤT (Sát thương quanh người)
        // ==========================================================
        public override void CastSkillW()
        {
            Debug.Log("⚔️ Warrior W: Dậm đất!");

            // 1. Hiệu ứng
            if (effectW != null) Instantiate(effectW, transform.position, Quaternion.identity);

            // 2. Quét quái xung quanh (Radius W)
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radiusW, player.enemyLayers);

            // 3. Tính dame (Gấp 2 lần công cơ bản)
            int skillDamage = Mathf.RoundToInt(player.damage * 2.0f);

            // 4. Trừ máu
            foreach (var hit in hitEnemies)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(skillDamage);
                    Debug.Log($"-> Dậm trúng: {hit.name}");
                }
            }
        }

        // ==========================================================
        // CHIÊU E - ĐỊA CHẤN (Chiêu cuối diện rộng)
        // ==========================================================
        public override void CastSkillE()
        {
            Debug.Log("😡 Warrior E: ĐỊA CHẤN!");

            if (effectE != null) Instantiate(effectE, transform.position, Quaternion.identity);

            // Quét phạm vi cực rộng (Radius E)
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radiusE, player.enemyLayers);

            // Dame cực to (Gấp 4 lần)
            int skillDamage = Mathf.RoundToInt(player.damage * 4.0f);

            foreach (var hit in hitEnemies)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(skillDamage);
                    Debug.Log($"-> Nổ chết: {hit.name}");
                }
            }
        }

        // Vẽ vòng tròn đỏ/vàng để căn chỉnh tầm đánh trong Scene
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radiusW);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radiusE);
        }
    }
}