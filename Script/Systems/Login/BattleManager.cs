using UnityEngine;
using UnityEngine.SceneManagement; // Để chuyển cảnh
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [Header("--- CÀI ĐẶT TÊN SCENE GAME ---")]
    // Điền tên scene gameplay của bạn vào đây (ví dụ: "GameCampaign", "GameEndless")
    public string campaignSceneName = "Game_Campaign";
    public string endlessSceneName = "Game_Endless";
    public string trainingSceneName = "Game_Training";
    public string hubSceneName = "Gameplay"; // Tên scene quay về

    [Header("--- CÀI ĐẶT POPUP (Kéo Panel vào) ---")]
    public GameObject historyPanel; // Bảng Lịch sử đấu
    public GameObject guidePanel;   // Bảng Cẩm nang

    // --- NHÓM 1: VÀO GAME (Chuyển Scene) ---

    public void OnClick_Campaign()
    {
        Debug.Log("Vào chế độ: Chiến Dịch");
        LoadGameScene(campaignSceneName);
    }

    public void OnClick_Endless()
    {
        Debug.Log("Vào chế độ: Vô Tận");
        LoadGameScene(endlessSceneName);
    }

    public void OnClick_Training()
    {
        Debug.Log("Vào chế độ: Tập Luyện");
        LoadGameScene(trainingSceneName);
    }

    // Hàm phụ để kiểm tra và load scene an toàn
    void LoadGameScene(string sceneName)
    {
        // Kiểm tra xem Scene có trong Build Settings chưa
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Chưa tạo hoặc chưa thêm Scene: " + sceneName + " vào Build Settings!");
        }
    }

    // --- NHÓM 2: HIỆN THÔNG TIN (Bật/Tắt Panel) ---

    public void OnClick_History()
    {
        if (historyPanel != null) historyPanel.SetActive(true);
    }

    public void OnClick_Guide()
    {
        if (guidePanel != null) guidePanel.SetActive(true);
    }

    public void OnClick_ClosePopup()
    {
        // Gắn vào nút X của các bảng popup
        if (historyPanel != null) historyPanel.SetActive(false);
        if (guidePanel != null) guidePanel.SetActive(false);
    }

    // --- NHÓM 3: QUAY VỀ ---
    public void OnClick_BackToHub()
    {
        SceneManager.LoadScene("Gameplay");
    }
}