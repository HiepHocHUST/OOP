using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("--- BẮT BUỘC PHẢI CÓ ---")]
    public LevelDataSO levelData; // Cái này bắt buộc phải kéo file Level_1 vào
    public CampaignManager manager; // Cái này bắt buộc phải kéo SystemCampaign vào

    [Header("--- CÓ THÌ KÉO, KHÔNG CÓ CŨNG KHÔNG SAO ---")]
    public GameObject lockIcon;      // Ổ khóa (Có thể để trống)
    public GameObject starGroup;     // Nhóm sao (Có thể để trống)
    public Text actionText;          // Chữ "Chiến đấu" (Có thể để trống)
    public Button myButton;          // Nút bấm

    void Start()
    {
        // Kiểm tra an toàn: Nếu quên kéo Level Data thì báo lỗi nhẹ rồi dừng, không làm sập game
        if (levelData == null)
        {
            Debug.LogError($"Nút {gameObject.name} chưa kéo file Level Data vào!");
            return;
        }

        UpdateLevelStatus();
    }

    void UpdateLevelStatus()
    {
        int levelID = levelData.levelID;

        // 1. Logic mở khóa
        bool isUnlocked = (levelID == 1) || (PlayerPrefs.GetInt("Level_" + (levelID - 1) + "_Completed", 0) == 1);

        if (!isUnlocked)
        {
            // --- TRẠNG THÁI KHÓA ---
            if (lockIcon != null) lockIcon.SetActive(true); // Có ổ khóa thì hiện

            if (myButton != null) myButton.interactable = false; // Có nút thì tắt bấm

            if (actionText != null) actionText.text = "Đã khóa"; // Có chữ thì đổi chữ

            if (starGroup != null) starGroup.SetActive(false); // Có sao thì tắt sao
        }
        else
        {
            // --- TRẠNG THÁI MỞ ---
            if (lockIcon != null) lockIcon.SetActive(false);

            if (myButton != null) myButton.interactable = true;

            // Kiểm tra đã thắng chưa
            bool isCompleted = PlayerPrefs.GetInt("Level_" + levelID + "_Completed", 0) == 1;

            if (isCompleted)
            {
                if (actionText != null) actionText.text = "Chơi lại";
                if (starGroup != null) starGroup.SetActive(true);
            }
            else
            {
                if (actionText != null) actionText.text = "Chiến đấu";
                if (starGroup != null) starGroup.SetActive(false);
            }
        }
    }

    public void OnClick_Level()
    {
        if (manager != null && levelData != null)
        {
            manager.ShowScoutingInfo(levelData);
        }
    }
}