using UnityEngine;
using UnityEngine.UI; // Để dùng Text, Button
using UnityEngine.SceneManagement; // Để chuyển cảnh
using System.Collections.Generic;
using Core; // Để gọi DataManager
using Gameplay.Core; // Để gọi HeroData

namespace Gameplay.UI
{
    public class CharacterSelectUI : MonoBehaviour
    {
        [Header("UI References (Kéo thả vào đây)")]
        public Text nameText;       // Text hiển thị Tên Tướng
        public Text statsText;      // Text hiển thị Chỉ số (HP, ATK)
        public Text descText;       // Text hiển thị Mô tả
        // public Image heroAvatar; // Sau này có ảnh thì dùng cái này

        [Header("Scene Settings")]
        public string gameSceneName = "01_Level_Ground"; // Tên màn chơi game của bạn

        private List<HeroData> heroList;
        private int currentIndex = 0;

        void Start()
        {
            // 1. Lấy danh sách tướng từ Database ngay khi mở màn hình
            if (DataManager.Instance != null)
            {
                heroList = DataManager.Instance.GetAllHeroesList();

                if (heroList != null && heroList.Count > 0)
                {
                    // Hiển thị tướng đầu tiên
                    currentIndex = 0;
                    UpdateUI();
                }
                else
                {
                    Debug.LogError("Lỗi: Không tìm thấy tướng nào trong Database!");
                    nameText.text = "No Data";
                }
            }
            else
            {
                Debug.LogError("Chưa có DataManager trong Scene! Hãy tạo GameObject DataManager.");
            }
        }

        // --- CÁC HÀM GẮN VÀO NÚT BẤM (BUTTON) ---

        public void OnNextButton()
        {
            if (heroList == null || heroList.Count == 0) return;

            currentIndex++;
            // Nếu vượt quá số lượng thì quay về đầu (Vòng lặp)
            if (currentIndex >= heroList.Count) currentIndex = 0;

            UpdateUI();
        }

        public void OnPrevButton()
        {
            if (heroList == null || heroList.Count == 0) return;

            currentIndex--;
            // Nếu nhỏ hơn 0 thì quay về cuối danh sách
            if (currentIndex < 0) currentIndex = heroList.Count - 1;

            UpdateUI();
        }

        public void OnPlayButton()
        {
            // Lấy ID của tướng đang chọn hiện tại
            int selectedID = heroList[currentIndex].HeroID;

            // LƯU ID VÀO BỘ NHỚ TẠM (PlayerPrefs)
            // Đây là cách đơn giản nhất để truyền dữ liệu sang màn chơi Game
            PlayerPrefs.SetInt("SelectedHeroID", selectedID);
            PlayerPrefs.Save();

            Debug.Log($"Đã chọn tướng ID: {selectedID}. Đang vào game...");

            // Chuyển sang màn chơi
            SceneManager.LoadScene(gameSceneName);
        }

        // --- HÀM CẬP NHẬT GIAO DIỆN ---
        void UpdateUI()
        {
            HeroData data = heroList[currentIndex];

            // Gán dữ liệu vào Text
            nameText.text = data.Name.ToUpper(); // Viết hoa tên cho đẹp

            // Format hiển thị chỉ số: HP: 100 | ATK: 20 | SPD: 1.5
            statsText.text = $"HP: <color=green>{data.BaseHP}</color>  |  ATK: <color=red>{data.BaseAtk}</color>  |  SPD: <color=yellow>{data.BaseSpeed}</color>";

            descText.text = data.Description;
        }
    }
}