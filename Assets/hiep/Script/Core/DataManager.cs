using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System.Collections.Generic; // 1. Thêm dòng này để dùng List<> cho gọn
using Gameplay.Core; // 2. Thêm dòng này để gọi HeroData, SkillData không cần gõ dài

namespace Core
{
    public class DataManager : MonoBehaviour
    {
        public static DataManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            OpenConnection();
        }

        // --- DATABASE CONFIG ---
        private IDbConnection dbConnection;

        // 3. SỬA TÊN FILE CHO KHỚP VỚI SQL CỦA BẠN
        private string dbName = "demo.db";

        public void OpenConnection()
        {
            string dbPath = "";
            if (Application.platform == RuntimePlatform.Android)
            {
                // Trên Android, cần copy từ StreamingAssets ra persistentDataPath trước
                // (Tạm thời giữ logic đơn giản này, sau này build Android tôi sẽ đưa code copy file sau)
                dbPath = "URI=file:" + Application.persistentDataPath + "/" + dbName;
            }
            else
            {
                // PC Editor
                dbPath = "URI=file:" + Application.streamingAssetsPath + "/" + dbName;
            }

            Debug.Log("🔗 Đang kết nối DB tại: " + dbPath);

            try
            {
                dbConnection = new SqliteConnection(dbPath);
                dbConnection.Open();
                Debug.Log("✅ Kết nối Database thành công!");
            }
            catch (System.Exception e)
            {
                Debug.LogError("❌ Lỗi kết nối DB: " + e.Message);
            }
        }

        // --- CORE SQL METHODS ---

        public IDataReader ExecuteQuery(string sqlQuery)
        {
            IDbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = sqlQuery;
            return dbCommand.ExecuteReader();
        }

        private void OnApplicationQuit()
        {
            if (dbConnection != null)
            {
                dbConnection.Close();
                dbConnection = null;
            }
        }

        // --- CÁC HÀM LẤY DỮ LIỆU GAME (DATA FETCHING) ---

        // Hàm 1: Lấy danh sách tướng (Cho Menu Chọn Tướng)
        public List<HeroData> GetAllHeroesList()
        {
            var list = new List<HeroData>();

            // Lấy ID, Tên, HP, Dame, Tốc độ, Mô tả
            string query = "SELECT HeroID, Name, BaseHP, BaseAtk, BaseSpeed, Description, BaseMana FROM Heroes";

            try
            {
                using (IDataReader reader = ExecuteQuery(query))
                {
                    while (reader.Read())
                    {
                        var data = new HeroData();
                        data.HeroID = reader.GetInt32(0);
                        data.Name = reader.GetString(1);
                        data.BaseHP = reader.GetInt32(2);
                        data.BaseAtk = reader.GetInt32(3);
                        data.BaseSpeed = (float)reader.GetDouble(4);

                        // Kiểm tra null an toàn cho cột Description
                        if (!reader.IsDBNull(5))
                            data.Description = reader.GetString(5);
                        else
                            data.Description = "Không có mô tả";
                        data.BaseMana = reader.GetInt32(6);
                        list.Add(data);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Lỗi đọc bảng Heroes: " + e.Message);
            }

            return list;
        }

        // Hàm 2: Lấy Skill của tướng (Cho Player khi vào game)
        public List<SkillData> GetSkillsOfHero(int heroID)
        {
            var list = new List<SkillData>();

            // Lấy các skill thuộc class này
            string query = $"SELECT SkillID, Name, Cooldown, ManaCost, SkillSlot FROM HeroSkills WHERE RequiredClassID = {heroID}";

            try
            {
                using (IDataReader reader = ExecuteQuery(query))
                {
                    while (reader.Read())
                    {
                        var skill = new SkillData();
                        skill.SkillID = reader.GetInt32(0);
                        skill.Name = reader.GetString(1);
                        skill.Cooldown = (float)reader.GetDouble(2);
                        skill.ManaCost = reader.GetInt32(3);
                        skill.SkillSlot = reader.GetInt32(4);

                        list.Add(skill);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Lỗi đọc bảng HeroSkills: " + e.Message);
            }

            return list;
        }
    }
}