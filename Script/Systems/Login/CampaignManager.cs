using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CampaignManager : MonoBehaviour
{
    [Header("--- UI TRINH SÁT (Kéo vào đây) ---")]
    public GameObject scoutingPanel;
    public Image bossImageUI;
    public Text bossNameUI;
    public Text bossStatsUI; // Gộp chung HP, Faction, Damage vào 1 text cho gọn
    public Text skillDescUI;

    // Biến lưu level đang chọn để chuẩn bị vào game
    private string currentSceneToLoad;

    // --- HÀM 1: MỞ BẢNG TRINH SÁT (Được gọi khi bấm nút Level) ---
    public void ShowScoutingInfo(LevelDataSO data)
    {
        // 1. Lưu lại tên scene cần load
        currentSceneToLoad = data.sceneToLoad;

        // 2. Đổ dữ liệu từ ScriptableObject lên UI
        if (bossImageUI) bossImageUI.sprite = data.bossImage;
        if (bossNameUI) bossNameUI.text = data.bossName;
        if (skillDescUI) skillDescUI.text = data.skillDescription;

        if (bossStatsUI)
        {
            bossStatsUI.text = $"Hệ: {data.faction}\n" +
                               $"HP: {data.bossHP:N0}\n" +
                               $"Sát thương: {data.bossDamage:N0}";
        }

        // 3. Hiện bảng lên
        scoutingPanel.SetActive(true);
    }

    // --- HÀM 2: BẮT ĐẦU TRẬN ĐẤU (Gắn vào nút ở bảng Trinh sát) ---
    public void OnClick_StartBattle()
    {
        if (!string.IsNullOrEmpty(currentSceneToLoad))
        {
            SceneManager.LoadScene(currentSceneToLoad);
        }
        else
        {
            Debug.LogError("Chưa nhập tên Scene trong Level Data!");
        }
    }

    // --- HÀM 3: ĐÓNG BẢNG ---
    public void OnClick_CloseScouting()
    {
        scoutingPanel.SetActive(false);
    }
}