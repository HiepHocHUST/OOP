using UnityEngine;

namespace Gameplay.Entities
{
    public class AutoScaleStats : MonoBehaviour
    {
        [Header("--- CẤU HÌNH SỨC MẠNH (Ở MAP 1) ---")]
        public int baseHealth = 20;     // Máu gốc
        public int baseDamage = 5;      // Dame gốc
        public int baseExp = 10;        // Kinh nghiệm gốc

        [Header("--- TỐC ĐỘ TĂNG TRƯỞNG (MỖI MAP) ---")]
        public int healthPerLevel = 10; // Mỗi map cộng thêm 10 máu
        public int damagePerLevel = 2;  // Mỗi map cộng thêm 2 dame
        public int expPerLevel = 5;     // Mỗi map cho thêm 5 exp

        [Header("--- NGOẠI HÌNH ---")]
        public bool increaseSize = true; // Có muốn quái to dần lên không?
        public float sizePerLevel = 0.05f; // Mỗi map to thêm 5%

        private void Start()
        {
            ApplyStats();
        }

        void ApplyStats()
        {
            // 1. Lấy Level hiện tại từ GameManager
            // Nếu không tìm thấy GameManager (lúc test) thì mặc định là Level 1
            int currentLevel = 1;
            if (LevelManager.Instance != null)
            {
                // Giả sử bạn lưu level ở LevelManager (hoặc GameManager)
                // Bạn cần biến public static int CurrentLevel ở đó
                // currentLevel = LevelManager.Instance.CurrentLevel; 

                // Tạm thời mình lấy ví dụ Level 1 để code không báo đỏ
                // Khi nào có biến Level thật thì thay số 1 bằng biến đó nhé
                currentLevel = 1;
            }

            // Mẹo: Để test nhanh, bạn có thể gán cứng currentLevel = 5 ở đây xem quái có mạnh lên ko

            // 2. Tính toán chỉ số (Công thức: Gốc + (Tăng trưởng x (Level - 1)))
            int finalHealth = baseHealth + (healthPerLevel * (currentLevel - 1));
            int finalDamage = baseDamage + (damagePerLevel * (currentLevel - 1));
            int finalExp = baseExp + (expPerLevel * (currentLevel - 1));

            // 3. Bơm vào cơ thể (Script Enemy)
            Enemy enemyScript = GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.maxHp = finalHealth;
                enemyScript.currentHp = finalHealth; // Hồi đầy máu mới
                enemyScript.damage = finalDamage;
                enemyScript.expReward = finalExp;

                Debug.Log($"💪 {gameObject.name} đã được buff lên Lv {currentLevel}: HP={finalHealth}, DMG={finalDamage}");
            }

            // 4. Bơm kích thước (Cho ngầu)
            if (increaseSize)
            {
                // Lấy kích thước gốc (thường là 1) cộng thêm phần tăng trưởng
                float scaleMultiplier = 1f + (sizePerLevel * (currentLevel - 1));

                // Giữ nguyên hướng quay mặt (dấu - hoặc + của trục X)
                float currentXDir = Mathf.Sign(transform.localScale.x);

                transform.localScale = new Vector3(scaleMultiplier * currentXDir, scaleMultiplier, 1f);
            }
        }
    }
}