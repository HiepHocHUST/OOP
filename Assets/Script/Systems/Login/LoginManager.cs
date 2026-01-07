using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // Cần thư viện này để dùng IEnumerator

public class LoginManager : MonoBehaviour
{
    [Header("UI References - Inputs")]
    public InputField usernameInput;
    public InputField passwordInput;
    public Dropdown slotDropdown;

    [Header("UI References - Feedback")]
    public Text statusText;

    [Header("Game Settings")]
    public string gameplayScene = "Gameplay";       // Tên màn chơi game thường
    public string adminScene = "AdminDashboard";    // Tên màn quản trị Admin

    private void Start()
    {
        // Reset thông báo lỗi khi bắt đầu
        if (statusText != null) statusText.text = "";

        // Load slot đã chọn lần trước
        if (slotDropdown != null)
        {
            int savedSlot = PlayerPrefs.GetInt("SelectedSlot", 0);
            slotDropdown.value = savedSlot;
        }
    }

    // --- HÀM GẮN VÀO NÚT "ENTER WORLD" ---
    public void OnClick_EnterWorld()
    {
        string u = usernameInput.text.Trim();
        string p = passwordInput.text.Trim();
        int slotIndex = slotDropdown != null ? slotDropdown.value : 0;

        // 1. Kiểm tra nhập liệu (Validate)
        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p))
        {
            ShowStatus("Username & Password cannot be empty!", Color.red);
            return;
        }

        // 2. Kiểm tra đăng nhập (Giả lập hoặc gọi Server)
        // Lưu ý: DataManager phải có hàm CheckLogin trả về true/false
        bool isSuccess = DataManager.Instance.CheckLogin(u, p);

        if (isSuccess)
        {
            // 3. Xử lý khi đăng nhập thành công

            // Lưu thông tin phiên đăng nhập
            PlayerPrefs.SetString("CurrentPlayer", u);
            PlayerPrefs.SetInt("SelectedSlot", slotIndex);
            PlayerPrefs.Save();

            // --- LOGIC PHÂN QUYỀN ADMIN/USER ---
            string role = "noob"; // Mặc định là người chơi thường

            // Kiểm tra tên đăng nhập (không phân biệt hoa thường)
            if (u.ToLower() == "admin")
            {
                role = "admin"; // Phát hiện là Admin
                ShowStatus("Xin chào Admin! Đang vào trang quản trị...", Color.yellow);
            }
            else
            {
                ShowStatus("Đăng nhập thành công! Đang vào game...", Color.green);
            }

            // Gọi hàm chuyển cảnh (đợi 1 giây để người dùng đọc thông báo)
            StartCoroutine(HandleSceneTransition(role));
        }
        else
        {
            ShowStatus("Sai tài khoản hoặc mật khẩu!", Color.red);
        }
    }

    // Coroutine xử lý độ trễ trước khi chuyển cảnh
    IEnumerator HandleSceneTransition(string role)
    {
        yield return new WaitForSeconds(1.0f); // Đợi 1 giây
        NavigateToScene(role);
    }

    // Hàm thực hiện chuyển Scene
    void NavigateToScene(string role)
    {
        if (role == "admin")
        {
            Debug.Log("Admin detected. Loading Dashboard...");
            // Load màn hình Admin
            LoadSceneSafe(adminScene);
        }
        else
        {
            Debug.Log("User detected. Entering Game...");
            // Load màn hình Game
            LoadSceneSafe(gameplayScene);
        }
    }

    // Hàm phụ trợ để load scene an toàn (tránh lỗi nếu quên thêm vào Build Settings)
    void LoadSceneSafe(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"LỖI: Scene '{sceneName}' chưa được thêm vào Build Settings! Vào File -> Build Settings để thêm.");
            ShowStatus($"Lỗi: Không tìm thấy scene '{sceneName}'", Color.red);
        }
    }

    // --- CÁC HÀM NÚT BẤM KHÁC ---
    public void OnClick_RegisterLink()
    {
        string u = usernameInput.text.Trim();
        string p = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p))
        {
            ShowStatus("Enter User/Pass to Register!", Color.yellow);
            return;
        }

        bool result = DataManager.Instance.RegisterUser(u, p);
        if (result) ShowStatus("Registered! Click Enter World now.", Color.green);
        else ShowStatus("Username already exists!", Color.red);
    }

    public void OnClick_ForgotLink()
    {
        ShowStatus("Feature coming soon!", Color.white);
    }

    // Hàm hiển thị thông báo lên màn hình
    void ShowStatus(string msg, Color color)
    {
        if (statusText != null)
        {
            statusText.text = msg;
            statusText.color = color;
        }
        Debug.Log(msg);
    }
}