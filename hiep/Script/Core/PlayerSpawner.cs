using UnityEngine;
// Nếu bạn dùng Cinemachine Camera thì bỏ comment dòng dưới
// using Cinemachine; 

namespace Gameplay.Core
{
    public class PlayerSpawner : MonoBehaviour
    {
        [Header("Danh sách tướng (Kéo Prefab vào đây)")]
        // Element 1 = Warrior, Element 4 = Assassin...
        public GameObject[] heroPrefabs;

        [Header("Vị trí xuất hiện")]
        public Transform spawnPoint;

        [Header("Camera theo dõi")]
        // Kéo Main Camera hoặc Cinemachine Virtual Camera vào đây
        public GameObject mainCamera;

        void Start()
        {
            SpawnPlayer();
        }

        void SpawnPlayer()
        {
            // 1. Lấy ID tướng đã chọn
            int selectedID = PlayerPrefs.GetInt("SelectedHeroID", 1);
            Debug.Log("Đang sinh ra nhân vật ID: " + selectedID);

            // 2. Kiểm tra xem có Prefab cho ID này không
            if (selectedID < heroPrefabs.Length && heroPrefabs[selectedID] != null)
            {
                // 3. TẠO NHÂN VẬT (Instantiate)
                GameObject newPlayer = Instantiate(heroPrefabs[selectedID], spawnPoint.position, Quaternion.identity);

                // Đặt tên lại cho đỡ bị (Clone)
                newPlayer.name = "Player";

                // 4. GẮN CAMERA VÀO NHÂN VẬT MỚI
                // Nếu dùng Camera thường (Code follow đơn giản):
                // mainCamera.GetComponent<CameraFollow>().target = newPlayer.transform;

                // Nếu dùng Cinemachine (Khuyên dùng):
                // var vcam = mainCamera.GetComponent<CinemachineVirtualCamera>();
                // if (vcam != null) vcam.Follow = newPlayer.transform;
            }
            else
            {
                Debug.LogError("Chưa gắn Prefab cho ID: " + selectedID);
            }
        }
    }
}