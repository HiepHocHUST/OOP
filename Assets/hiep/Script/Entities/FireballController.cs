using UnityEngine;
using Gameplay.Entities;

public class FireballController : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 20;
    public float lifeTime = 10f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true;
    }

    void Start()
    {
        // Debug.Log("Viên đạn đã sinh ra, bắt đầu đếm ngược 3s...");
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 direction)
    {
        if (rb != null) rb.linearVelocity = direction.normalized * speed;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // --- LOG QUAN TRỌNG ĐỂ TÌM THỦ PHẠM ---
        string hitName = hitInfo.name;
        string hitTag = hitInfo.tag;
        string hitLayer = LayerMask.LayerToName(hitInfo.gameObject.layer);

        // 1. GẶP PHE MÌNH (Enemy/Boss) -> BỎ QUA
        if (hitInfo.CompareTag("Enemy") || hitInfo.CompareTag("Boss"))
        {
            // Debug.Log($"🛡️ Đạn xuyên qua đồng đội: {hitName}");
            return;
        }

        // 2. TRÚNG PLAYER
        if (hitInfo.CompareTag("Player"))
        {
            Debug.Log($"🔥 Bắn trúng Player ({hitName})!");
            Player playerScript = hitInfo.GetComponent<Player>();
            if (playerScript != null) playerScript.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // 3. TRÚNG TƯỜNG (Layer Ground)
        if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log($"🧱 Đạn đâm vào Tường/Đất: {hitName} (Layer: {hitLayer}) -> HỦY!");
            Destroy(gameObject);
            return;
        }

        // 4. TRƯỜNG HỢP LẠ: Va vào cái gì đó không phải Enemy, không phải Player, không phải Ground
        // Đây chính là chỗ đạn biến mất vô lý
        Debug.LogWarning($"❓ Đạn chạm vật lạ: '{hitName}' (Tag: {hitTag} | Layer: {hitLayer}). Code hiện tại đang BỎ QUA nó.");
    }
}