using UnityEngine;
using UnityEngine.UI; // Bắt buộc để dùng Image

public class GameUI : MonoBehaviour
{
    public static GameUI Instance; // Singleton để Player gọi từ bất cứ đâu

    [Header("--- THANH TRẠNG THÁI ---")]
    public Image hpFill;   // Kéo HP_Fill vào đây
    public Image manaFill; // Kéo Mana_Fill vào đây

    [Header("--- ICON KỸ NĂNG (Kéo Image Icon vào đây) ---")]
    public Image iconQ;
    public Image iconW;
    public Image iconE;

    [Header("--- VÒNG HỒI CHIÊU (Kéo Image Cooldown vào đây) ---")]
    public Image cooldownQ;
    public Image cooldownW;
    public Image cooldownE;

    private void Awake()
    {
        // Tạo Singleton: Đảm bảo chỉ có 1 GameUI tồn tại
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- HÀM 1: CẬP NHẬT MÁU ---
    public void UpdateHPBar(int currentHp, int maxHp)
    {
        if (hpFill != null)
        {
            // Tính tỷ lệ phần trăm (0.0 -> 1.0)
            hpFill.fillAmount = (float)currentHp / maxHp;
        }
    }

    // --- HÀM 2: CẬP NHẬT MANA ---
    public void UpdateManaBar(int currentMana, int maxMana)
    {
        if (manaFill != null)
        {
            manaFill.fillAmount = (float)currentMana / maxMana;
        }
    }

    // --- HÀM 3: ĐỔI ICON SKILL (Tướng gọi hàm này khi vào game) ---
    public void SetupSkillIcons(Sprite spriteQ, Sprite spriteW, Sprite spriteE)
    {
        // Đổi hình ảnh cho các ô skill
        if (iconQ != null && spriteQ != null) { iconQ.sprite = spriteQ; iconQ.enabled = true; }
        if (iconW != null && spriteW != null) { iconW.sprite = spriteW; iconW.enabled = true; }
        if (iconE != null && spriteE != null) { iconE.sprite = spriteE; iconE.enabled = true; }

        // Reset vòng hồi chiêu về 0 (trong suốt)
        if (cooldownQ != null) cooldownQ.fillAmount = 0;
        if (cooldownW != null) cooldownW.fillAmount = 0;
        if (cooldownE != null) cooldownE.fillAmount = 0;
    }

    // --- HÀM 4: CẬP NHẬT VÒNG QUAY HỒI CHIÊU ---
    public void UpdateCooldown(string skillKey, float currentTime, float maxTime)
    {
        // Tính tỷ lệ: Nếu maxTime = 0 thì trả về 0 để tránh lỗi chia cho 0
        float ratio = (maxTime > 0) ? currentTime / maxTime : 0;

        switch (skillKey)
        {
            case "Q":
                if (cooldownQ != null) cooldownQ.fillAmount = ratio;
                break;
            case "W":
                if (cooldownW != null) cooldownW.fillAmount = ratio;
                break;
            case "E":
                if (cooldownE != null) cooldownE.fillAmount = ratio;
                break;
        }
    }
}