/*using UnityEngine;
using Gameplay.Entities; // Để gọi được Player và Enemy
using Core; // Để gọi DataManager
using System.Data; // Để dùng IDataReader

public class GameManager : MonoBehaviour
{
    // Singleton để gọi từ bất cứ đâu
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public Player playerScript;    // Kéo nhân vật Player vào đây
    public GameObject enemyPrefab; // Kéo Prefab Enemy vào đây
    public Transform enemySpawnPoint; // Vị trí sinh quái

    // Giả định User đang chơi là UserID = 1 (Admin)
    private int currentUserID = 1;

    private void Awake()
    {
        // Setup Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // --- SỬA LẠI ĐOẠN NÀY ---
    private void Start()
    {
        Debug.Log("=== GAMEMANAGER KHỞI ĐỘNG ===");

        // Kiểm tra an toàn trước khi chạy
        if (playerScript == null || enemyPrefab == null || enemySpawnPoint == null)
        {
            Debug.LogError("THIẾU KẾT NỐI! Hãy kiểm tra lại Inspector của GameManager.");
            return;
        }

        // Bắt đầu nạp game
        InitGame();
    }

    public void InitGame()
    {
        Debug.Log("--- BẮT ĐẦU LOAD DỮ LIỆU TỪ DB ---");

        // 1. Load chỉ số Player (Từ UserHeroes + Items)
        LoadPlayerStats();

        // 2. Sinh ra một con quái (VD: ID 101 - Slime Xanh)
        SpawnEnemy(101);
    }

    // --- PHẦN 1: XỬ LÝ PLAYER ---
    private void LoadPlayerStats()
    {
        // B1: Tính tổng chỉ số cộng thêm từ trang bị (Items)
        int bonusAtk = 0;
        int bonusDef = 0;

        string sqlGear = $@"
            SELECT I.StatBonus, I.Type
            FROM UserInventory UI
            JOIN Items I ON UI.ItemID = I.ItemID
            WHERE UI.UserID = {currentUserID} AND UI.IsEquipped = 1";

        IDataReader gearReader = DataManager.Instance.ExecuteQuery(sqlGear);
        while (gearReader.Read())
        {
            int val = gearReader.GetInt32(0); // StatBonus
            int type = gearReader.GetInt32(1); // Type (1: Vũ khí, 2: Giáp)

            if (type == 1) bonusAtk += val;
            if (type == 2) bonusDef += val;
        }
        gearReader.Close();

        // B2: Lấy thông tin Tướng và UserHeroes
        string sqlHero = $@"
            SELECT US.CurrentHeroID,H.Name, UH.Str, UH.Agi, UH.Intelligence, H.BaseAtk
            FROM UserHeroes UH
            JOIN Heroes H ON UH.HeroID = H.HeroID
            JOIN UserStats US ON UH.UserID = US.UserID
            WHERE UH.UserID = {currentUserID} AND UH.HeroID = US.CurrentHeroID";

        IDataReader heroReader = DataManager.Instance.ExecuteQuery(sqlHero);
        if (heroReader.Read())
        {
            int currentHeroID = heroReader.GetInt32(0);
            string name = heroReader.GetString(1);
            int s = heroReader.GetInt32(2); // Str
            int a = heroReader.GetInt32(3); // Agi
            int i = heroReader.GetInt32(4); // Intelligence
            int bAtk = heroReader.GetInt32(5); // BaseAtk từ bảng Heroes

            // Gọi hàm SetupData của class Player (Đa hình)
            playerScript.SetupData(currentHeroID, name, s, a, i, bonusAtk + bAtk, bonusDef);
            Debug.Log("Load Player thành công!");
        }
        else
        {
            Debug.LogError("Lỗi: Không tìm thấy Hero nào được chọn trong UserStats!");
        }
        heroReader.Close();
    }

    // --- PHẦN 2: XỬ LÝ ENEMY ---
    public void SpawnEnemy(int enemyID)
    {
        string sqlEnemy = $"SELECT Name, BaseHP, BaseDamage, ExpReward, MinGoldDrop, MaxGoldDrop FROM Enemies WHERE EnemyID = {enemyID}";
        IDataReader reader = DataManager.Instance.ExecuteQuery(sqlEnemy);

        if (reader.Read())
        {
            string name = reader.GetString(0);
            int hp = reader.GetInt32(1);
            int dmg = reader.GetInt32(2);
            int exp = reader.GetInt32(3);
            int minGold = reader.GetInt32(4);
            int maxGold = reader.GetInt32(5);

            // Tạo Object quái mới từ Prefab
            GameObject newMob = Instantiate(enemyPrefab, enemySpawnPoint.position, Quaternion.identity);

            // Lấy script Enemy (Con của Unit)
            Enemy enemyScript = newMob.GetComponent<Enemy>();

            if (enemyScript != null)
            {
                enemyScript.SetupData(name, hp, dmg, exp, minGold, maxGold);
                Debug.Log($"Đã sinh ra quái: {name}");
            }
        }
        else
        {
            Debug.LogError($"Không tìm thấy quái có ID {enemyID} trong DB!");
        }
        reader.Close();
    }
}*/