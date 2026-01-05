using UnityEngine;

namespace Gameplay.Entities
{
    // Để abstract để không ai được phép tạo new Unit() khơi khơi
    public abstract class Unit : MonoBehaviour
    {
        [Header("Common Stats")]
        public string unitName;
        public int currentHp;
        public int maxHp;      // Quái: Cố định. Player: Tính toán.
        public int damage;     // Quái: Cố định. Player: Tính toán.
        public int defense;    // Quái: 0. Player: Tính toán.

        // Hàm này bắt buộc các con phải tự định nghĩa lại (Override)
        public abstract void CalculateStats();

        // Hàm nhận sát thương (Dùng chung, nhưng có thể Override nếu muốn)
        public virtual void TakeDamage(int incomingDamage)
        {
            int finalDamage = incomingDamage - defense;
            if (finalDamage < 1) finalDamage = 1;

            currentHp -= finalDamage;
            Debug.Log($"{unitName} mất {finalDamage} máu. Còn: {currentHp}");

            if (currentHp <= 0) Die();
        }

        protected virtual void Die()
        {
            Debug.Log($"{unitName} đã bị tiêu diệt!");
            Destroy(gameObject);
        }
    }
}