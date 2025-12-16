using System;
using System.Data;
using MySql.Data.MySqlClient; // Thư viện vừa cài
using System.Windows.Forms;

namespace WindowsForm
{
    public class DatabaseHelper
    {
        // 1. CẤU HÌNH KẾT NỐI (SỬA LẠI CHO ĐÚNG MÁY BẠN)
        private string server = "localhost";
        private string database = "gamerpg_db"; // <--- Tên Database bạn vừa chạy SQL
        private string uid = "root";            // Tên đăng nhập MySQL (mặc định là root)
        private string password = "Hiep2005@HAIHAU";        // <--- Mật khẩu MySQL Workbench của bạn

        public MySqlConnection GetConnection()
        {
            string connString = $"Server={server};Database={database};Uid={uid};Pwd={password};";
            return new MySqlConnection(connString);
        }

        // 2. HÀM KIỂM TRA ĐĂNG NHẬP
        public string CheckLogin(string username, string password)
        {
            string role = null;

            using (MySqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();

                    // 1. Kiểm tra tài khoản
                    string query = "SELECT Role FROM users WHERE Username = @u AND Password = @p";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", password);

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            role = result.ToString();

                            // 2. NẾU ĐÚNG: Cập nhật thời gian LastLogin
                            // (Lệnh này chạy ngầm, không cần chờ kết quả)
                            string updateQuery = "UPDATE users SET LastLogin = NOW() WHERE Username = @u";
                            using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@u", username);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi Database: " + ex.Message);
                }
            }
            return role;
        }

    }
}