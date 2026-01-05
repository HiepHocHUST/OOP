using UnityEngine;

// Để script này ở ngoài namespace (global) để script nào cũng gọi được dễ dàng
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance; // Singleton để gọi từ xa

    [Header("Cổng Dịch Chuyển")]
    public GameObject congDichChuyen; // Kéo cái Portal vào đây

    private int totalEnemies = 0; // Tổng số quái đang sống

    void Awake()
    {
        // Đảm bảo chỉ có 1 LevelManager tồn tại
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Đầu game tắt cổng đi (nếu có gán)
        if (congDichChuyen != null)
            congDichChuyen.SetActive(false);
    }

    // 1. Quái sinh ra thì gọi cái này
    public void DangKyQuai()
    {
        totalEnemies++;
        // Debug.Log("Quái +1. Tổng: " + totalEnemies);
    }

    // 2. Quái chết thì gọi cái này
    public void QuaiChet()
    {
        totalEnemies--;
        // Debug.Log("Quái -1. Còn: " + totalEnemies);

        if (totalEnemies <= 0)
        {
            MoCong();
        }
    }

    void MoCong()
    {
        Debug.Log("🎉 ĐÃ DIỆT SẠCH! CỔNG MỞ!");
        if (congDichChuyen != null)
            congDichChuyen.SetActive(true);
    }
}