using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện quản lý Scene

public class SceneController : MonoBehaviour
{
    // --- PHẦN 1: ĐI TỪ HUB RA CÁC CHỨC NĂNG ---

    public void OnClick_GoToBattle()
    {
        SceneManager.LoadScene("BattleScene");
    }

    public void OnClick_GoToInventory()
    {
        SceneManager.LoadScene("InventoryScene");
    }

    public void OnClick_GoToShop()
    {
        SceneManager.LoadScene("ShopScene");
    }

    public void OnClick_GoToSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    public void OnClick_GoToRanking()
    {
        SceneManager.LoadScene("RankingScene");
    }

    public void OnClick_GoToHeroes()
    {
        SceneManager.LoadScene("HeroesScene");
    }

    // --- PHẦN 2: NÚT QUAY VỀ (Dùng cho các Scene con) ---

    // Gắn hàm này vào nút "Back" hoặc "Home" ở các Scene con
    public void OnClick_BackToHub()
    {
        SceneManager.LoadScene("Gameplay");
        // Nhớ đổi "PlayerHubScene" đúng tên Scene chính của bạn
    }

    // --- PHẦN 3: ĐĂNG XUẤT ---
    public void OnClick_Logout()
    {
        SceneManager.LoadScene("LoginScene");
    }
}