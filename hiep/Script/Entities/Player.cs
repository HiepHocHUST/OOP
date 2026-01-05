using UnityEngine;
using Gameplay.Skills; // Để dùng WarriorSkills, MageSkills...
using Gameplay.Core;   // Để dùng HeroData
using Core;            // Để dùng DataManager

namespace Gameplay.Entities
{
    [RequireComponent(typeof(PlayerMovement))]
    public class Player : Unit
    {
        [Header("--- CÀI ĐẶT HÌNH DẠNG (SKIN) ---")]
        [Tooltip("Kéo thả Animator Controller tương ứng với ID vào đây. Element 1 = Warrior, Element 2 = Mage...")]
        public RuntimeAnimatorController[] heroAnimators;

        [Header("Stats References")]
        public int heroID = 1;
        public int str, agi, intelligence;
        public int currentMana;
        public int maxMana;
        public float manaRegenRate = 5f; // Mana hồi mỗi giây
        public float manaTimer;    // Bộ đếm thời gian hồi mana
        [Header("Combat References")]
        public Transform attackPoint;
        public float attackRange = 1.0f;
        public LayerMask enemyLayers;

        [Header("Skill References")]
        public Transform firePoint;     // Điểm bắn (cho Assassin/Archer)
        public Transform castPoint;     // Điểm tung chiêu (cho Warrior/Mage)
        public GameObject projectilePrefab; // Prefab đạn cơ bản (nếu cần)
        public GameObject skillQ_VFX;

        // 👇 ĐÃ SỬA: Khai báo thẳng là HeroSkillSet để đỡ phải ép kiểu
        public HeroSkillSet mySkills;

        public Animator anim;
        private PlayerMovement movementScript;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            movementScript = GetComponent<PlayerMovement>();
        }

        private void Start()
        {
            // --- BƯỚC 1: NHẬN DIỆN TƯỚNG TỪ MENU ---
            int selectedID = PlayerPrefs.GetInt("SelectedHeroID", 1);
            Debug.Log($"🎮 Đang khởi tạo nhân vật với ID: {selectedID}");

            // --- BƯỚC 2: LẤY DỮ LIỆU TỪ SQLITE ---
            if (DataManager.Instance != null)
            {
                var allHeroes = DataManager.Instance.GetAllHeroesList();
                HeroData myData = allHeroes.Find(x => x.HeroID == selectedID);
                if (myData != null)
                {
                    this.heroID = myData.HeroID;
                    this.unitName = myData.Name;
                    this.maxHp = myData.BaseHP;
                    this.currentHp = myData.BaseHP;
                    this.damage = myData.BaseAtk;
                    this.maxMana = myData.BaseMana;
                    this.currentMana = this.maxMana;
                }
            }
            else
            {
                Debug.LogWarning("⚠️ Không tìm thấy DataManager! Dùng chỉ số mặc định.");
                SetupData(selectedID, "Test Hero", 10, 5, 5, 20, 5);
            }

            // --- BƯỚC 3: THAY ĐỔI HÌNH DẠNG (ANIMATOR) ---
            ChangeVisuals(selectedID);

            // --- BƯỚC 4: TỰ ĐỘNG LẤY SKILL ---
            // Tìm script skill (WarriorSkills/AssassinSkills) gắn trên người
            mySkills = GetComponent<HeroSkillSet>();

            if (mySkills != null)
            {
                Debug.Log("✅ Đã tìm thấy bộ kỹ năng: " + mySkills.GetType().Name);
                // Khởi động Skill (Nạp thông tin Player vào cho Skill dùng)
                mySkills.Initialize(this);
            }
            else
            {
                Debug.LogError("❌ LỖI: Prefab này chưa được gắn Script Skill (AssassinSkills/WarriorSkills...) trong Inspector!");
            }

            if (UIManager.Instance != null)
            {
                // Cập nhật thanh máu và mana ngay khi game bắt đầu
                UIManager.Instance.UpdateHP(currentHp, maxHp);
                UIManager.Instance.UpdateMana(currentMana, maxMana);
            }
        }

        // Hàm đổi Animator Controller
        void ChangeVisuals(int id)
        {
            if (heroAnimators != null && id < heroAnimators.Length && heroAnimators[id] != null)
            {
                this.anim.runtimeAnimatorController = heroAnimators[id];
            }
        }

        public void SetupData(int id, string name, int s, int a, int i, int bAtk, int bDef)
        {
            heroID = id; unitName = name;
            str = s; agi = a; intelligence = i;
            CalculateStats();
            currentHp = maxHp;
            maxMana = intelligence * 10;
            currentMana = maxMana;
        }

        public override void CalculateStats()
        {
            maxHp = 200 + (str * 20);
            damage = 20 + (str * 5);
        }

        // 👇👇👇 KHU VỰC QUAN TRỌNG NHẤT: XỬ LÝ PHÍM BẤM 👇👇👇
        private void Update()
        {
            // 1. ĐÁNH THƯỜNG (Phím A hoặc Chuột trái)
            if (Input.GetKeyDown(KeyCode.A) || Input.GetMouseButtonDown(0))
            {
                // Gọi Animation đánh thường (Nếu có)
                if (anim != null) anim.SetTrigger("Attack");

                // Gọi logic gây sát thương
                if (mySkills != null) mySkills.BasicAttack();
            }

            // 2. SKILL Q (Phím Q)
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (mySkills != null)
                {
                    // ⚠️ LƯU Ý: Gọi TryCastQ (để kiểm tra mana/cooldown) 
                    // CHỨ KHÔNG gọi CastSkillQ (hàm này chỉ để animation gọi)
                    mySkills.TryCastQ();
                }
            }

            // 3. SKILL W (Phím W) - Đã thêm mới
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (mySkills != null)
                {
                    mySkills.TryCastW();
                }
            }

            // 4. SKILL E (Phím E) - Đã thêm mới
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (mySkills != null)
                {
                    mySkills.TryCastE();
                }
            }

            if (currentMana < maxMana)
            {
                manaTimer += Time.deltaTime;
                if (manaTimer >= 1f) // Cứ mỗi 1 giây thì hồi 1 lần
                {
                    currentMana += Mathf.RoundToInt(manaRegenRate);

                    // Đảm bảo không vượt quá Max
                    if (currentMana > maxMana) currentMana = maxMana;

                    // Cập nhật UI (Tí nữa mình viết hàm này)
                    if (UIManager.Instance != null) UIManager.Instance.UpdateMana(currentMana, maxMana);

                    manaTimer = 0; // Reset đồng hồ
                }
            }
        }

        protected override void Die()
        {
            base.Die();
            Debug.Log("Player chết!");
            if (movementScript != null) movementScript.SetMobility(false);
        }

        void OnDrawGizmosSelected()
        {
            if (attackPoint != null) Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        // Ghi đè hàm TakeDamage từ Unit.cs
        public override void TakeDamage(int dmg)
        {
            base.TakeDamage(dmg); // Gọi hàm cha để trừ số liệu máu

            // Gọi UI cập nhật hiển thị
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHP(currentHp, maxHp);
            }
        }

        public bool UseMana(int cost)
        {
            if (currentMana >= cost)
            {
                currentMana -= cost;

                // Cập nhật UI ngay lập tức
                if (UIManager.Instance != null)
                    UIManager.Instance.UpdateMana(currentMana, maxMana);

                return true; // Đủ mana, cho phép tung chiêu
            }
            else
            {
                Debug.Log("⚠️ Không đủ Mana!");
                return false; // Hết mana, cấm tung chiêu
            }
        }
    }
}