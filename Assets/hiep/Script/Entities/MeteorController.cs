using UnityEngine;
using Gameplay.Entities;

public class MeteorController : MonoBehaviour
{
    public int damage = 20;
    public GameObject explosionEffect;

    private void Start()
    {
        // Kiểm tra xem Script có thực sự đang chạy không
        Debug.Log($"✅ Thiên thạch đã sinh ra tại: {transform.position}");

        transform.Rotate(0, 0, Random.Range(0, 360));
        Destroy(gameObject, 5f);
    }

    // Dành cho trường hợp có tích ô "Is Trigger"
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"⚡ TRIGGER: Thiên thạch vừa xuyên qua [ {collision.gameObject.name} ]");
        XuLyVaCham(collision.gameObject);
    }

    // Dành cho trường hợp QUÊN tích ô "Is Trigger" (Va đập vật lý)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"💥 COLLISION: Thiên thạch vừa húc đầu vào [ {collision.gameObject.name} ]");
        XuLyVaCham(collision.gameObject);
    }

    // Hàm xử lý chung
    void XuLyVaCham(GameObject targetObj)
    {
        if (targetObj.CompareTag("Player"))
        {
            Debug.Log("🎯 Trúng Player! Đang xử lý trừ máu...");

            // Tìm script Player (trên người hoặc trên cha)
            Player player = targetObj.GetComponent<Player>();
            if (player == null) player = targetObj.GetComponentInParent<Player>();

            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log($"🩸 Đã gọi lệnh trừ {damage} máu.");
            }
            else
            {
                Debug.LogError("❌ Lỗi: Có Tag Player nhưng không tìm thấy Script Player!");
            }
            Explode();
        }
        else if (targetObj.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("tao no");
            Explode();
        }
    }

    void Explode()
    {
        if (explosionEffect != null) Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}