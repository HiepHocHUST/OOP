using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdminManager : MonoBehaviour
{
    [Header("Cài đặt Scene")]
    public string loginSceneName = "Login"; // Tên scene đăng nhập

    [Header("UI Panels - Bảng chính")]
    public GameObject dashboardPanel;     // Bảng chứa 4 ô vuông to

    [Header("UI Panels - Các bảng chức năng con")]
    // Kéo các bảng giao diện bạn đã tạo vào đây
    public GameObject userMgmtPanel;      // 1. Panel Quản lý User
    public GameObject maintenancePanel;   // 2. Panel Bảo trì Dữ liệu (Cái bạn vừa làm)
    public GameObject masterDataPanel;    // 3. Panel Dữ liệu Chính
    public GameObject statsPanel;         // 4. Panel Thống kê

    // --- CÁC HÀM GẮN VÀO NÚT "TRUY CẬP" ---

    // 1. Nút: Quản lý Người dùng
    public void OnClick_UserManagement()
    {
        Debug.Log("Admin: Mở Quản lý Người dùng");
        OpenSubPanel(userMgmtPanel);
    }

    // 2. Nút: Bảo trì Dữ liệu (Quan trọng nhất với bạn lúc này)
    public void OnClick_Maintenance()
    {
        Debug.Log("Admin: Mở Bảo trì Dữ liệu");
        OpenSubPanel(maintenancePanel);
    }

    // 3. Nút: Quản lý Dữ liệu Chính
    public void OnClick_MasterData()
    {
        Debug.Log("Admin: Mở Dữ liệu Chính");
        OpenSubPanel(masterDataPanel);
    }

    // 4. Nút: Thống kê Tài nguyên
    public void OnClick_Statistics()
    {
        Debug.Log("Admin: Mở Thống kê");
        OpenSubPanel(statsPanel);
    }

    // --- CÁC HÀM HỆ THỐNG ---

    // Hàm chung để xử lý việc tắt Dashboard -> Mở Panel con
    private void OpenSubPanel(GameObject panelToOpen)
    {
        if (panelToOpen != null)
        {
            // Ẩn bảng menu chính đi
            if (dashboardPanel != null) dashboardPanel.SetActive(false);

            // Hiện bảng chức năng được chọn
            panelToOpen.SetActive(true);
        }
        else
        {
            Debug.LogError("Chưa kéo Panel vào Inspector kìa bạn ơi!");
        }
    }

    // Hàm gán vào nút "X" hoặc nút "Back" nằm trong các bảng con
    public void OnClick_BackToDashboard()
    {
        // Ẩn tất cả các bảng con đi (để chắc ăn)
        if (userMgmtPanel != null) userMgmtPanel.SetActive(false);
        if (maintenancePanel != null) maintenancePanel.SetActive(false);
        if (masterDataPanel != null) masterDataPanel.SetActive(false);
        if (statsPanel != null) statsPanel.SetActive(false);

        // Hiện lại bảng menu chính
        if (dashboardPanel != null) dashboardPanel.SetActive(true);
    }

    // Hàm Đăng xuất
    public void OnClick_Logout()
    {
        PlayerPrefs.DeleteKey("CurrentPlayer");
        SceneManager.LoadScene(loginSceneName);
    }
}