using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Gameplay.Entities;
using System.Collections;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public static int CurrentUserID = 0; // Giả sử UserID hiện tại là 0
        [Header("--- CẤU HÌNH MAP ---")]
        public static int CurrentMapLevel = 1;

        [Header("--- CẤU HÌNH SPAWN ---")]
        public Transform slimeSpawnPoint;
        public Transform bossSpawnPoint;

        // 👇 ĐÃ SỬA: Thay đổi từ 1 Boss thành Mảng Boss (GameObject[])
        public GameObject[] bossPrefabs; // Kéo thả Boss 1, Boss 2, Boss 3 vào đây

        public GameObject[] slimePrefabs;

        [Header("--- THỜI GIAN ---")]
        public float bossSpawnTime = 90f;
        public float slimeSpawnInterval = 5f; // Thời gian giữa các lần đẻ

        private float gameTimer = 0f;
        private bool isBossSpawned = false;
        public bool IsGameEnded { get; private set; } = false;

        [Header("--- UI ---")]
        public GameObject victoryPanel;
        public GameObject defeatPanel;
        public Text timerText;

        [Header("--- REFERENCES ---")]
        public Player playerScript;

        [Header("--- KẾT NỐI LƯU TRỮ ---")]
        public HistoryManager historyManager; // 👈 Kéo script HistoryManager vào đây

        // Biến để cộng dồn vàng khi đánh quái (nếu bạn chưa có)
        public int currentLevelGold = 0; // Biến này nãy bạn tạo rồi
        public Text goldUIText; // 👇 Kéo cái Text hiển thị vàng trên màn hình chơi vào đây (nếu có)

        // 👇 THÊM HÀM NÀY: Để Enemy gọi khi nó chết
        public void AddGold(int amount)
        {
            currentLevelGold += amount;

            // Cập nhật lên màn hình ngay lập tức cho sướng mắt
            if (goldUIText != null)
            {
                goldUIText.text = "Gold: " + currentLevelGold.ToString();
            }

            Debug.Log("💰 Đã cộng " + amount + " vàng! Tổng hiện tại: " + currentLevelGold);
        }
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            AutoUpdateLevelFromScene();
        }

        private void Start()
        {
            if (playerScript == null) playerScript = FindObjectOfType<Player>();

            // InvokeRepeating để đẻ Slime (Gọi hàm SpawnSlime mỗi 5 giây)
            InvokeRepeating(nameof(SpawnSlime), 2f, slimeSpawnInterval);

            if (victoryPanel) victoryPanel.SetActive(false);
            if (defeatPanel) defeatPanel.SetActive(false);

            Debug.Log($"=== GAME BẮT ĐẦU (Map Level: {CurrentMapLevel}) ===");
        }

        private void Update()
        {
            if (IsGameEnded) return;

            gameTimer += Time.deltaTime;

            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(gameTimer / 60F);
                int seconds = Mathf.FloorToInt(gameTimer - minutes * 60);
                timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
            }

            if (gameTimer >= bossSpawnTime && !isBossSpawned)
            {
                SpawnBoss();
            }
        }

        // 👇👇👇 HÀM NÀY ĐÃ ĐƯỢC SỬA LẠI LOGIC 👇👇👇
        void SpawnSlime()
        {
            if (IsGameEnded || slimePrefabs == null || slimePrefabs.Length == 0) return;
            if (slimeSpawnPoint == null) return;

            // 1. Đếm số lượng Enemy đang có trên màn hình
            // ⚠️ QUAN TRỌNG: Bạn nhớ set Tag cho Slime Prefab là "Enemy" nhé!
            GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");

            // 2. Tính giới hạn số lượng (Level càng cao -> Giới hạn càng thấp)
            // Công thức: Map 1 cho phép 7 con. Cứ lên 2 map thì giảm đi 1 con limit.
            int maxSlimeAllowed = 7 - (CurrentMapLevel / 2);

            // Giới hạn cứng: Dù Level 100 thì vẫn phải cho ra ít nhất 2 con để đánh
            if (maxSlimeAllowed < 2) maxSlimeAllowed = 2;

            // 3. Kiểm tra: Nếu đông quá rồi thì thôi, không đẻ nữa
            if (existingEnemies.Length >= maxSlimeAllowed)
            {
                // Debug.Log($"Đông quá ({existingEnemies.Length}/{maxSlimeAllowed}), ngưng đẻ!");
                return;
            }

            // 4. Nếu chưa đủ số lượng thì đẻ tiếp
            int randomIndex = Random.Range(0, slimePrefabs.Length);
            Instantiate(slimePrefabs[randomIndex], slimeSpawnPoint.position, Quaternion.identity);
        }
        // 👆👆👆 HẾT PHẦN SỬA 👆👆👆

        void SpawnBoss()
        {
            if (bossPrefabs == null || bossPrefabs.Length == 0)
            {
                Debug.LogError("❌ LỖI: Chưa kéo con Boss nào vào danh sách 'Boss Prefabs' trong GameManager!");
                return;
            }

            Debug.Log("⚠️ CẢNH BÁO: BOSS ĐÃ XUẤT HIỆN!");
            isBossSpawned = true;

            // Chọn Boss theo Level Map (Map 1 -> Boss 0, Map 2 -> Boss 1...)
            int bossIndex = (CurrentMapLevel - 1) % bossPrefabs.Length;

            if (bossIndex < bossPrefabs.Length && bossPrefabs[bossIndex] != null)
            {
                Debug.Log($"😈 Map {CurrentMapLevel} -> Triệu hồi Boss ID: {bossIndex}");
                Instantiate(bossPrefabs[bossIndex], bossSpawnPoint.position, Quaternion.identity);
            }
            else
            {
                // Fallback: Nếu tính toán sai thì cứ lấy con đầu tiên
                Instantiate(bossPrefabs[0], bossSpawnPoint.position, Quaternion.identity);
            }
        }

        public void Victory()
        {
            if (IsGameEnded) return;

            // 1. Tính toán số liệu THỰC TẾ
            int finalGold = currentLevelGold + 500; // Ví dụ: Vàng nhặt được + 500 vàng thưởng thắng
            int result = 1; // 1 là Thắng

            // 2. GỌI LỆNH LƯU TỰ ĐỘNG
            // Kiểm tra xem có HistoryManager không để tránh lỗi
            if (historyManager != null)
            {
                // Lưu: (ID người chơi, Map hiện tại, Thắng/Thua, Tổng vàng)
                historyManager.SaveMatch(CurrentUserID, CurrentMapLevel, result, finalGold);
                Debug.Log($"✅ Đã tự động lưu trận thắng cho User {CurrentUserID}!");
            }
            else
            {
                Debug.LogWarning("⚠️ Chưa gắn HistoryManager vào GameManager nên không lưu được!");
            }

            // 3. Hiện bảng thắng (Code cũ của bạn)
            IsGameEnded = true;
            if (victoryPanel) victoryPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        public void Defeat()
        {
            if (IsGameEnded) return;

            // 1. Tính toán số liệu THỰC TẾ
            int finalGold = currentLevelGold; // Thua thì chỉ lấy vàng nhặt được, không có thưởng
            int result = 0; // 0 là Thua

            // 2. GỌI LỆNH LƯU TỰ ĐỘNG
            if (historyManager != null)
            {
                historyManager.SaveMatch(CurrentUserID, CurrentMapLevel, result, finalGold);
                Debug.Log($"✅ Đã tự động lưu trận thua cho User {CurrentUserID}!");
            }

            // 3. Hiện bảng thua (Code cũ của bạn)
            IsGameEnded = true;
            if (defeatPanel) defeatPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        void AutoUpdateLevelFromScene()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName.StartsWith("Level_"))
            {
                string numberPart = sceneName.Substring(6);
                if (int.TryParse(numberPart, out int level)) CurrentMapLevel = level;
            }
        }
    }
}