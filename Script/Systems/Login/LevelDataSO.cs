using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "GameData/Level Data")]
public class LevelDataSO : ScriptableObject
{
    [Header("--- THÔNG TIN LEVEL ---")]
    public int levelID;              // Ví dụ: 1, 2, 3
    public string sceneToLoad;       // Tên Scene game thật (ví dụ: "Map_Forest_1")

    [Header("--- THÔNG TIN TRINH SÁT BOSS ---")]
    public string bossName;          // Tên Boss (Rồng Hắc Ám)
    public Sprite bossImage;         // Ảnh Boss
    public string faction;           // Hệ (Bóng tối)
    public int bossHP;               // Máu
    public int bossDamage;           // Sát thương
    [TextArea] public string skillDescription; // Mô tả kỹ năng

    [Header("--- TRANG BỊ GỢI Ý (Ảnh) ---")]
    public Sprite item1;
    public Sprite item2;
    public Sprite item3;
}