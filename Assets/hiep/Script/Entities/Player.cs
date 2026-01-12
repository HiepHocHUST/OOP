using UnityEngine;
using Gameplay.Skills; // Để dùng WarriorSkills, MageSkills...
using Gameplay.Core;   // Để dùng HeroData
using Core;            // Để dùng DataManager (GameManager)

namespace Gameplay.Entities
{
    // --- CLASS ĐỊNH NGHĨA TRANG BỊ ---
    // (Giúp bạn tạo vũ khí/giáp ngay trên Inspector hoặc load từ DB)
    [System.Serializable]
    public class EquipmentItem
    {
        public string itemName;
        public int bonusHp;   // Máu cộng thêm
        public int bonusMana; // Mana cộng thêm
        public int bonusStr;  // Sức mạnh cộng thêm (Tăng damage)
    }

    [RequireComponent(typeof(PlayerMovement))]
    public class Player : Unit
    {
        [Header("--- CÀI ĐẶT HÌNH DẠNG (SKIN) ---")]
        [Tooltip("Kéo thả Animator Controller tương ứng với ID vào đây.")]
        public RuntimeAnimatorController[] heroAnimators;

        [Header("--- TRANG BỊ (EQUIPMENT) ---")]
        public EquipmentItem weapon; // Kéo thả hoặc nhập số trực tiếp trên Inspector để test
        public EquipmentItem armor;

        [Header("Stats References")]
        public int heroID = 1;
        public int str, agi, intelligence;
        public int currentMana;
        public int maxMana;
        public float manaRegenRate = 5f;
        public float manaTimer;

        [Header("Combat References")]
        public Transform attackPoint;
        public float attackRange = 1.0f;
        public LayerMask enemyLayers;

        [Header("Skill References")]
        public Transform firePoint;
        public Transform castPoint;
        public GameObject projectilePrefab;
        public GameObject skillQ_VFX;

        // Script kỹ năng (WarriorSkills, AssassinSkills...)
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

                    // Gán chỉ số cơ bản
                    this.str = 10; // (Ví dụ: Lấy từ DB nếu có cột Str)
                    this.intelligence = 5;

                    // Lưu ý: Các chỉ số maxHp, damage sẽ được tính lại trong hàm CalculateStats()
                    // dựa trên Str/Int và Trang bị.
                }
            }
            else
            {
                Debug.LogWarning("⚠️ Không tìm thấy DataManager! Dùng chỉ số mặc định.");
                SetupData(selectedID, "Test Hero", 10, 5, 5, 20, 5);
            }

            // --- BƯỚC 3: TÍNH TOÁN CHỈ SỐ LẦN ĐẦU ---
            // (Phải gọi sau khi đã có Str/Int và Weapon/Armor)
            CalculateStats();
            this.currentHp = this.maxHp;
            this.currentMana = this.maxMana;

            // --- BƯỚC 4: THAY ĐỔI HÌNH DẠNG ---
            ChangeVisuals(selectedID);

            // --- BƯỚC 5: TỰ ĐỘNG LẤY SKILL ---
            mySkills = GetComponent<HeroSkillSet>();
            if (mySkills != null)
            {
                Debug.Log("✅ Đã tìm thấy bộ kỹ năng: " + mySkills.GetType().Name);
                mySkills.Initialize(this);
            }
            else
            {
                Debug.LogError("❌ LỖI: Prefab chưa gắn Script Skill!");
            }

            // Cập nhật UI ban đầu
            UpdateUI();
        }

        private void Update()
        {
            // 🛑 QUAN TRỌNG: Nếu game đã kết thúc (Thắng/Thua) thì ngừng điều khiển
            if (GameManager.Instance != null && GameManager.Instance.IsGameEnded) return;

            // Kiểm tra null để tránh lỗi
            if (mySkills == null) return;

            // 1. ĐÁNH THƯỜNG
            if (Input.GetKeyDown(KeyCode.A) || Input.GetMouseButtonDown(0))
            {
                mySkills.TryBasicAttack();
            }

            // 2. CÁC SKILL
            if (Input.GetKeyDown(KeyCode.Q)) mySkills.TryCastQ();
            if (Input.GetKeyDown(KeyCode.W)) mySkills.TryCastW();
            if (Input.GetKeyDown(KeyCode.E)) mySkills.TryCastE();

            // Hồi Mana
            HandleManaRegen();
        }

        // --- TÍNH TOÁN CHỈ SỐ (ĐÃ NÂNG CẤP ĐỂ TÍNH CẢ ĐỒ) ---
        public override void CalculateStats()
        {
            int finalStr = str;
            int finalInt = intelligence;
            int addedHp = 0;
            int addedMana = 0;

            // 1. Cộng chỉ số từ Vũ khí
            if (weapon != null)
            {
                finalStr += weapon.bonusStr;
                addedHp += weapon.bonusHp;
                addedMana += weapon.bonusMana;
            }

            // 2. Cộng chỉ số từ Giáp
            if (armor != null)
            {
                finalStr += armor.bonusStr;
                addedHp += armor.bonusHp;
                addedMana += armor.bonusMana;
            }

            // 3. Áp dụng công thức RPG
            // Máu = 200 gốc + (Sức mạnh * 20) + Máu từ đồ
            maxHp = 200 + (finalStr * 20) + addedHp;

            // Damage = 20 gốc + (Sức mạnh * 5)
            damage = 20 + (finalStr * 5);

            // Mana = Trí tuệ * 10 + Mana từ đồ
            maxMana = (finalInt * 10) + addedMana;

            // Debug để kiểm tra xem mặc đồ vào có mạnh lên không
            Debug.Log($"🛡️ PLAYER STATS: Str={finalStr} | HP={maxHp} | Dmg={damage}");
        }

        // --- XỬ LÝ CHẾT (BÁO THUA) ---
        protected override void Die()
        {
            base.Die(); // Gọi hàm cha để hủy object/hiệu ứng
            Debug.Log("💀 PLAYER ĐÃ CHẾT!");

            // Dừng di chuyển
            if (movementScript != null) movementScript.SetMobility(false);

            // Báo cho GameManager biết là Thua
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Defeat();
            }
        }

        // --- CÁC HÀM HỖ TRỢ ---
        public void SetupData(int id, string name, int s, int a, int i, int bAtk, int bDef)
        {
            heroID = id; unitName = name;
            str = s; agi = a; intelligence = i;
            CalculateStats(); // Tính lại ngay khi set data
        }

        void HandleManaRegen()
        {
            if (currentMana < maxMana)
            {
                manaTimer += Time.deltaTime;
                if (manaTimer >= 1f)
                {
                    currentMana += Mathf.RoundToInt(manaRegenRate);
                    if (currentMana > maxMana) currentMana = maxMana;
                    UpdateUI();
                    manaTimer = 0;
                }
            }
        }

        public bool UseMana(int cost)
        {
            if (currentMana >= cost)
            {
                currentMana -= cost;
                UpdateUI();
                return true;
            }
            else
            {
                Debug.Log("⚠️ Không đủ Mana!");
                return false;
            }
        }

        void UpdateUI()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHP(currentHp, maxHp);
                UIManager.Instance.UpdateMana(currentMana, maxMana);
            }
        }

        void ChangeVisuals(int id)
        {
            if (heroAnimators != null && id < heroAnimators.Length && heroAnimators[id] != null)
            {
                this.anim.runtimeAnimatorController = heroAnimators[id];
            }
        }

        // Ghi đè hàm TakeDamage để cập nhật UI ngay khi mất máu
        public override void TakeDamage(int dmg)
        {
            base.TakeDamage(dmg);
            UpdateUI();
        }

        void OnDrawGizmosSelected()
        {
            if (attackPoint != null) Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}