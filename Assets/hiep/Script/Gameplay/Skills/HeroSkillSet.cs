using UnityEngine;
using System.Collections;
using Gameplay.Entities;

namespace Gameplay.Skills
{
    public class HeroSkillSet : MonoBehaviour
    {
        protected Player player;

        [Header("--- CẤU HÌNH ĐÁNH THƯỜNG ---")]
        public float attackSpeed = 0.5f;
        public float attackDelay = 0.2f;
        protected bool isAttacking = false;

        [Header("--- CẤU HÌNH MANA & COOLDOWN SKILL ---")]
        public int manaQ = 10; public float cooldownQ = 1.0f;
        public int manaW = 20; public float cooldownW = 3.0f;
        public int manaE = 40; public float cooldownE = 10.0f;

        protected bool isCooldownQ = false;
        protected bool isCooldownW = false;
        protected bool isCooldownE = false;

        public virtual void Initialize(Player _player)
        {
            player = _player;
            Debug.Log($"✅ HeroSkillSet đã Initialize với Player: {_player.name}");
        }

        // ==========================================================
        // 👇 ĐÁNH THƯỜNG (BASIC ATTACK)
        // ==========================================================
        public virtual void TryBasicAttack()
        {
            // [LOG 1] Kiểm tra đầu vào
            Debug.Log("📌 [1] TryBasicAttack: Đã nhận lệnh bấm nút!");

            // 1. Kiểm tra Cooldown
            if (isAttacking)
            {
                Debug.LogWarning("⚠️ [Cooldown] Đang chờ hồi chiêu (Attack Speed). Bỏ qua.");
                return;
            }

            // 2. Kích hoạt Cooldown tổng
            StartCoroutine(AttackCooldownRoutine());

            // 3. Chạy Animation
            if (player.anim != null)
            {
                player.anim.ResetTrigger("Attack");
                player.anim.SetTrigger("Attack");
                Debug.Log("🎬 [Animation] Đã kích hoạt Trigger 'Attack'");
            }
            else
            {
                Debug.LogError("❌ [LỖI] Không tìm thấy Animator trên Player!");
            }

            // 4. Gọi bộ đếm giờ
            Debug.Log($"⏳ [2] Bắt đầu đếm ngược {attackDelay}s để gây damage...");
            StartCoroutine(DelayDamageRoutine());
        }

        // Coroutine: Chờ xong mới gọi hàm trừ máu
        protected IEnumerator DelayDamageRoutine()
        {
            yield return new WaitForSeconds(attackDelay);

            Debug.Log("⏰ [3] Hết thời gian chờ (Delay). Gọi hàm BasicAttack() ngay bây giờ!");
            BasicAttack();
        }

        // Coroutine: Quản lý tốc độ đánh
        protected IEnumerator AttackCooldownRoutine()
        {
            isAttacking = true;
            yield return new WaitForSeconds(attackSpeed);
            isAttacking = false;
        }

        // Hàm này Warrior/Assassin sẽ ghi đè
        public virtual void BasicAttack()
        {
            Debug.LogError("❌ [LỖI] Hàm BasicAttack gốc đang chạy! Có vẻ WarriorSkills chưa override hàm này?");
        }

        // ==========================================================
        // CÁC SKILL KHÁC (Giữ nguyên)
        // ==========================================================
        public virtual void TryCastQ()
        {
            if (!CanCastSkill(isCooldownQ, manaQ)) return;
            ConsumeResources(val => isCooldownQ = val, manaQ, cooldownQ);
            if (player.anim != null) player.anim.SetTrigger("Cast");
            CastSkillQ();
        }
        public virtual void CastSkillQ() { }

        public virtual void TryCastW()
        {
            if (!CanCastSkill(isCooldownW, manaW)) return;
            ConsumeResources(val => isCooldownW = val, manaW, cooldownW);
            if (player.anim != null) player.anim.SetTrigger("Strick");
            CastSkillW();
        }
        public virtual void CastSkillW() { }

        public virtual void TryCastE()
        {
            if (!CanCastSkill(isCooldownE, manaE)) return;
            ConsumeResources(val => isCooldownE = val, manaE, cooldownE);
            if (player.anim != null) player.anim.SetTrigger("Until");
            CastSkillE();
        }
        public virtual void CastSkillE() { }

        protected bool CanCastSkill(bool isCoolingDown, int manaCost)
        {
            if (isCoolingDown) return false;
            if (player.currentMana < manaCost)
            {
                Debug.Log("⚠️ Không đủ Mana!");
                return false;
            }
            return true;
        }

        protected void ConsumeResources(System.Action<bool> setCooldownState, int manaCost, float time)
        {
            if (player != null && manaCost > 0) player.UseMana(manaCost);
            StartCoroutine(CooldownRoutine(setCooldownState, time));
        }

        protected IEnumerator CooldownRoutine(System.Action<bool> setCooldownState, float time)
        {
            setCooldownState(true);
            yield return new WaitForSeconds(time);
            setCooldownState(false);
        }
    }
}