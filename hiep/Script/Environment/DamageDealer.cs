using UnityEngine;
using Gameplay.Entities;

public class DamageDealer : MonoBehaviour
{
    [Header("Cấu hình")]
    public int damage = 10;          // Sát thương mỗi lần
    public float damageInterval = 1f; // Bao lâu thì trừ máu 1 lần? (1 giây)
    public bool pushBack = true;

    private float nextDamageTime = 0f; // Biến đếm thời gian

    // 1. Khi vừa bước vào -> Trừ ngay lập tức
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ProcessDamage(collision);
        }
    }

    // 2. Khi vẫn còn đứng bên trong -> Kiểm tra thời gian để trừ tiếp
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Nếu thời gian hiện tại (Time.time) đã vượt qua mốc cho phép
            if (Time.time >= nextDamageTime)
            {
                ProcessDamage(collision);
            }
        }
    }

    // Hàm xử lý chung (để đỡ phải viết lại code 2 lần)
    void ProcessDamage(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Debug.Log($"🌵 Á á! Đang dính độc... Mất {damage} máu");

            // Cập nhật mốc thời gian cho lần trừ máu tiếp theo
            // Ví dụ: Bây giờ là giây thứ 5, interval là 1s -> Lần sau trừ ở giây thứ 6
            nextDamageTime = Time.time + damageInterval;

            // Logic đẩy lùi (nếu cần)
            if (pushBack)
            {
                // Gọi hàm Knockback bên Player (nếu có)
            }
        }
    }
}