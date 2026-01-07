using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Class Cha quản lý logic chung
public class BasePopup : MonoBehaviour
{
    [Header("Base UI Elements")]
    [SerializeField] protected TMP_Text titleText; // protected để con kế thừa dùng được
    [SerializeField] protected Button closeBtn;
    [SerializeField] protected GameObject contentPanel;

    // Hàm ảo (Virtual) cho phép lớp con ghi đè logic
    public virtual void Open()
    {
        gameObject.SetActive(true);
        Debug.Log("Đang mở cửa sổ: " + gameObject.name);
        // Có thể thêm hiệu ứng âm thanh mở cửa sổ ở đây (Tính tái sử dụng code)
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    protected virtual void Start()
    {
        // Tự động gắn sự kiện tắt cho nút Close
        if (closeBtn != null)
            closeBtn.onClick.AddListener(Close);
    }
}