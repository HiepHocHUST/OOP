using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;

public class HistoryManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject rowPrefab;
    public Transform contentHolder;

    private string dbPath;

    void Awake()
    {
        // Xác lập đường dẫn Database
        if (Application.platform == RuntimePlatform.Android)
            dbPath = "URI=file:" + Application.persistentDataPath + "/demo.db";
        else
            dbPath = "URI=file:" + Application.streamingAssetsPath + "/demo.db";
    }

    // --- 👇 ĐOẠN QUAN TRỌNG NHẤT ĐỂ HIỆN BẢNG ---
    private void OnEnable()
    {
        // Mỗi lần bạn bật bảng Lịch sử (SetActive = true), nó sẽ tự Load lại dữ liệu mới nhất
        // Lấy đúng CurrentUserID từ GameManager để tránh hiện bảng trống
        LoadHistory(Core.GameManager.CurrentUserID);
    }

    public void LoadHistory(int userID)
    {
        if (contentHolder == null || rowPrefab == null) return;

        // Xóa sạch các dòng cũ trên giao diện trước khi nạp mới
        foreach (Transform child in contentHolder) Destroy(child.gameObject);

        Debug.Log($"📊 Đang tải lịch sử cho User ID: {userID}");

        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                // Truy vấn lấy dữ liệu sắp xếp theo ngày mới nhất lên đầu
                cmd.CommandText = "SELECT StageID, IsWin, GoldEarned, PlayDate FROM MatchHistory WHERE UserID = @uid ORDER BY PlayDate DESC";
                cmd.Parameters.Add(new SqliteParameter("@uid", userID));

                using (var reader = cmd.ExecuteReader())
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        int sID = reader.GetInt32(0);
                        int res = reader.GetInt32(1);
                        int gold = reader.GetInt32(2);
                        string date = reader.GetString(3);

                        SpawnRow(sID, res, gold, date);
                        count++;
                    }
                    Debug.Log($"✅ Đã hiển thị {count} trận đấu gần nhất.");
                }
            }
        }
    }

    void SpawnRow(int stage, int result, int gold, string date)
    {
        GameObject newRow = Instantiate(rowPrefab, contentHolder);
        BattleRow rowScript = newRow.GetComponent<BattleRow>();
        if (rowScript != null) rowScript.SetupRow(date, stage, result, gold);
    }

    // --- HÀM LẤY DATA QUÁI (GIỮ NGUYÊN) ---
    public EnemyDBData GetEnemyStats(int id)
    {
        EnemyDBData data = null;
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                // Đảm bảo tên bảng trong Database là 'Enemies' hoặc 'enemy' cho khớp
                cmd.CommandText = "SELECT Name, BaseHP, BaseDamage, ExpReward, MinGoldDrop, MaxGoldDrop, IsBoss, Scale FROM enemy WHERE EnemyID = @id";
                cmd.Parameters.Add(new SqliteParameter("@id", id));

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        data = new EnemyDBData();
                        data.name = reader.GetString(0); data.hp = reader.GetInt32(1);
                        data.dmg = reader.GetInt32(2); data.exp = reader.GetInt32(3);
                        data.minG = reader.GetInt32(4); data.maxG = reader.GetInt32(5);
                        data.isBoss = reader.GetInt32(6) == 1; data.scale = (float)reader.GetDouble(7);
                    }
                }
            }
        }
        return data;
    }

    // --- HÀM LƯU KẾT QUẢ TRẬN ĐẤU (GIỮ NGUYÊN) ---
    public void SaveMatch(int userID, int stageID, int result, int gold)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                string currentDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                cmd.CommandText = "INSERT INTO MatchHistory (UserID, StageID, IsWin, GoldEarned, PlayDate) VALUES (@u, @s, @w, @g, @d)";
                cmd.Parameters.Add(new SqliteParameter("@u", userID));
                cmd.Parameters.Add(new SqliteParameter("@s", stageID));
                cmd.Parameters.Add(new SqliteParameter("@w", result));
                cmd.Parameters.Add(new SqliteParameter("@g", gold));
                cmd.Parameters.Add(new SqliteParameter("@d", currentDate));
                cmd.ExecuteNonQuery();
            }
        }
        Debug.Log("✅ Đã lưu kết quả trận đấu!");
    }
}