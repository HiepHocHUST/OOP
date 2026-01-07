using UnityEngine;
using UnityEngine.UI; // Bắt buộc có dòng này

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton để gọi từ xa

    [Header("Gắn ảnh vào đây")]
    public Image healthBarImg;
    public Image manaBarImg;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Hàm 1: Cập nhật Máu
    public void UpdateHP(int current, int max)
    {
        // 1. Kiểm tra xem ảnh đã được gắn chưa
        if (healthBarImg == null)
        {
            Debug.LogError("❌ LỖI TO: Chưa kéo ảnh Thanh Máu vào GameManager/UIManager bạn ơi!");
            return;
        }

        // 2. Tính toán và in ra xem nó tính ra số mấy
        float ratio = (float)current / max;
        Debug.Log($"🩸 UI Máu: {current}/{max} = {ratio}"); // Nó phải in ra 0.9, 0.8...

        // 3. Thực hiện thay đổi
        healthBarImg.fillAmount = ratio;
    }

    public void UpdateMana(int current, int max)
    {
        if (manaBarImg == null)
        {
            Debug.LogError("❌ LỖI TO: Chưa kéo ảnh Thanh Mana vào GameManager/UIManager!");
            return;
        }

        float ratio = (float)current / max;
        Debug.Log($"💧 UI Mana: {current}/{max} = {ratio}");

        manaBarImg.fillAmount = ratio;
    }
}