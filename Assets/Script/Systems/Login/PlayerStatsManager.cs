using UnityEngine;
using UnityEngine.UI; // Bắt buộc để chỉnh sửa Text và Slider

public class PlayerStatsManager : MonoBehaviour
{
    [Header("--- KẾT NỐI GIAO DIỆN (Kéo thả vào đây) ---")]
    public Text textGold;      // Số vàng
    public Text textGem;       // Số kim cương
    public Text textLevel;     // Số Level
    public Slider sliderXP;    // Thanh trượt XP
    public Text textXP_Ratio;  // Chữ hiển thị tỉ lệ (ví dụ: 150/1000)

    [Header("--- DỮ LIỆU NGƯỜI CHƠI (Để test) ---")]
    public int currentLevel = 1;
    public int currentGold = 1000;
    public int currentGem = 50;

    public float currentXP = 0;
    public float maxXP = 500; // Cần 500 XP để lên cấp

    void Start()
    {
        // Khi game chạy, cập nhật giao diện lần đầu tiên
        UpdateUI();
    }

    // --- HÀM 1: CẬP NHẬT GIAO DIỆN ---
    void UpdateUI()
    {
        // 1. Cập nhật Tiền tệ
        if (textGold != null) textGold.text = currentGold.ToString("N0"); // N0 để có dấu phẩy (1,000)
        if (textGem != null) textGem.text = currentGem.ToString("N0");
        if (textLevel != null) textLevel.text = "Level: " + currentLevel;

        // 2. Cập nhật Thanh XP
        if (sliderXP != null)
        {
            sliderXP.maxValue = maxXP; // Đặt giá trị tối đa cho thanh trượt
            sliderXP.value = currentXP; // Đặt giá trị hiện tại
        }

        // 3. Cập nhật chữ trên thanh XP
        if (textXP_Ratio != null)
        {
            textXP_Ratio.text = currentXP + " / " + maxXP;
        }
    }

    // --- HÀM 2: TĂNG VÀNG (Gắn vào nút để test) ---
    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateUI(); // Thay đổi số liệu xong phải cập nhật lại màn hình
    }

    // --- HÀM 3: TĂNG XP (Quan trọng) ---
    public void AddXP(float amount)
    {
        currentXP += amount;

        // Kiểm tra Lên cấp: Nếu XP hiện tại lớn hơn Max XP
        if (currentXP >= maxXP)
        {
            currentXP = currentXP - maxXP; // Giữ lại phần XP dư
            currentLevel++;                // Tăng cấp
            maxXP = maxXP * 1.2f;          // Cấp sau khó hơn cấp trước (tăng 20% yêu cầu)

            Debug.Log("CHÚC MỪNG! BẠN ĐÃ LÊN LEVEL " + currentLevel);
        }

        UpdateUI();
    }
}