using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System.Collections.Generic;
using Gameplay.Core;
// LƯU Ý: Mình đã bỏ "namespace Core" để bạn không cần gõ "using Core" ở các file khác nữa.
public class DataManager : MonoBehaviour
{
    // --- SINGLETON PATTERN ---
    public static DataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Giữ sống qua các màn chơi
        OpenConnection(); // Mở kết nối ngay khi chạy
    }

    // --- DATABASE CONNECTION ---
    private IDbConnection dbConnection;
    private string dbName = "demo.db";

    public void OpenConnection()
    {
        string dbPath = Path.Combine(Application.streamingAssetsPath, dbName);
        string connectionString = "URI=file:" + dbPath;

        Debug.Log("Đang kết nối DB tại: " + dbPath);

        try
        {
            dbConnection = new SqliteConnection(connectionString);
            dbConnection.Open();
            Debug.Log("Kết nối Database thành công!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Lỗi kết nối DB: " + e.Message);
        }
    }

    // --- CÁC HÀM TIỆN ÍCH CƠ BẢN ---

    public IDataReader ExecuteQuery(string sqlQuery)
    {
        if (dbConnection == null) OpenConnection();
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = sqlQuery;
        return dbCommand.ExecuteReader();
    }

    public void ExecuteNonQuery(string sqlQuery)
    {
        if (dbConnection == null) OpenConnection();
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = sqlQuery;
        dbCommand.ExecuteNonQuery();
    }

    // --- CÁC HÀM XỬ LÝ NGƯỜI DÙNG (USER) ---

    // 1. Kiểm tra đăng nhập
    public bool CheckLogin(string username, string password)
    {
        if (dbConnection == null) OpenConnection();
        string sql = $"SELECT COUNT(*) FROM Users WHERE Username = '{username}' AND Password = '{password}'";

        IDbCommand cmd = dbConnection.CreateCommand();
        cmd.CommandText = sql;
        long count = (long)cmd.ExecuteScalar();

        return count > 0;
    }

    // 2. Đăng ký tài khoản
    public bool RegisterUser(string username, string password)
    {
        if (dbConnection == null) OpenConnection();

        try
        {
            // Kiểm tra trùng tên
            string checkSql = $"SELECT COUNT(*) FROM Users WHERE Username = '{username}'";
            IDbCommand checkCmd = dbConnection.CreateCommand();
            checkCmd.CommandText = checkSql;
            long count = (long)checkCmd.ExecuteScalar();

            if (count > 0) return false; // Trùng tên -> Thất bại

            // Thêm mới
            string insertSql = $"INSERT INTO Users (Username, Password) VALUES ('{username}', '{password}')";
            IDbCommand insertCmd = dbConnection.CreateCommand();
            insertCmd.CommandText = insertSql;
            insertCmd.ExecuteNonQuery();

            return true;
        }
        catch { return false; }
    }

    // 3. Đổi mật khẩu
    public bool ResetPassword(string username, string newPassword)
    {
        if (dbConnection == null) OpenConnection();

        string updateSql = $"UPDATE Users SET Password = '{newPassword}' WHERE Username = '{username}'";
        IDbCommand cmd = dbConnection.CreateCommand();
        cmd.CommandText = updateSql;

        int rowsAffected = cmd.ExecuteNonQuery();
        return rowsAffected > 0;
    }

    // --- CÁC HÀM GAMEPLAY (Ví dụ lấy chỉ số Hero) ---
    public void GetHeroStats(int heroID)
    {
        string query = "SELECT * FROM UserHeroes WHERE HeroID = " + heroID;
        IDataReader reader = ExecuteQuery(query);

        while (reader.Read())
        {
            int str = reader.GetInt32(reader.GetOrdinal("Str"));
            int agi = reader.GetInt32(reader.GetOrdinal("Agi"));
            Debug.Log($"Hero {heroID} Stats -> STR: {str}, AGI: {agi}");
        }
        reader.Close();
    }
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
    // --- ĐÓNG KẾT NỐI KHI THOÁT GAME ---
    private void OnApplicationQuit()
    {
        if (dbConnection != null)
        {
            dbConnection.Close();
            dbConnection = null;
        }
    }
}