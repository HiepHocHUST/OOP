using UnityEngine;
using System.Collections;
using Gameplay.Entities; // Để nhận diện script Player

namespace Gameplay.Skills
{
    public class HeroSkillSet : MonoBehaviour
    {
        protected Player player;

        [Header("--- CẤU HÌNH MANA & COOLDOWN ---")]
        public int manaQ = 10;
        public float cooldownQ = 1.0f;

        public int manaW = 20;
        public float cooldownW = 3.0f;

        public int manaE = 50;
        public float cooldownE = 10.0f;

        // Biến kiểm tra đang hồi chiêu
        protected bool isCooldownQ = false;
        protected bool isCooldownW = false;
        protected bool isCooldownE = false;

        // Khởi tạo: Nhận thông tin nhân vật để biết ai đang dùng chiêu
        public virtual void Initialize(Player _player)
        {
            player = _player;
        }

        public virtual void BasicAttack() { }

        // ==========================================================
        // CHIÊU Q (Trigger: Cast)
        // ==========================================================
        public virtual void TryCastQ()
        {
            if (!CanCastSkill(isCooldownQ, manaQ)) return;

            // Truyền hành động gán biến (val => isCooldownQ = val)
            ConsumeResources(val => isCooldownQ = val, manaQ, cooldownQ);

            if (player.anim != null) player.anim.SetTrigger("Cast");
            else CastSkillQ();
        }

        public virtual void CastSkillQ() { }


        // ==========================================================
        // CHIÊU W (Trigger: Strick)
        // ==========================================================
        public virtual void TryCastW()
        {
            if (!CanCastSkill(isCooldownW, manaW)) return;

            ConsumeResources(val => isCooldownW = val, manaW, cooldownW);

            if (player.anim != null) player.anim.SetTrigger("Strick");
            else CastSkillW();
        }

        public virtual void CastSkillW() { }


        // ==========================================================
        // CHIÊU E (Trigger: Until)
        // ==========================================================
        public virtual void TryCastE()
        {
            if (!CanCastSkill(isCooldownE, manaE)) return;

            ConsumeResources(val => isCooldownE = val, manaE, cooldownE);

            if (player.anim != null) player.anim.SetTrigger("Until");
            else CastSkillE();
        }

        public virtual void CastSkillE() { }


        // ==========================================================
        // CÁC HÀM HỖ TRỢ (CORE LOGIC)
        // ==========================================================

        // 1. Kiểm tra xem có đủ điều kiện tung chiêu không
        protected bool CanCastSkill(bool isCoolingDown, int manaCost)
        {
            if (isCoolingDown) return false; // Đang hồi chiêu -> Nghỉ

            if (player.currentMana < manaCost)
            {
                Debug.Log("💧 Không đủ Mana!");
                return false; // Hết tiền -> Nghỉ
            }
            return true;
        }

        // 2. Trừ tài nguyên và kích hoạt hồi chiêu
        // 👇 ĐÂY LÀ CHỖ QUAN TRỌNG NHẤT ĐÃ SỬA 👇
        protected void ConsumeResources(System.Action<bool> setCooldownState, int manaCost, float time)
        {
            if (player != null)
            {
                // Thay vì viết: player.currentMana -= manaCost (Code cũ - Sai vì không cập nhật UI)
                // Ta viết:
                player.UseMana(manaCost);
                // Hàm UseMana bên Player sẽ lo việc trừ tiền và gọi UIManager vẽ lại thanh mana
            }

            // Bắt đầu đếm ngược hồi chiêu
            StartCoroutine(CooldownRoutine(setCooldownState, time));
        }

        // 3. Bộ đếm thời gian hồi chiêu
        protected IEnumerator CooldownRoutine(System.Action<bool> setCooldownState, float time)
        {
            // Set biến thành true (Đang bận)
            setCooldownState(true);

            yield return new WaitForSeconds(time);

            // Set biến thành false (Đã rảnh)
            setCooldownState(false);
        }
    }
}