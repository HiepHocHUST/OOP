using UnityEngine;

namespace Gameplay.Combat
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [Header("Settings")]
        public float speed = 10f;
        public float lifetime = 3f;
        public GameObject hitVFX;

        private int damage;
        private Vector2 direction;
        private Rigidbody2D rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.useFullKinematicContacts = true;
        }

        // 👇 ĐÃ CẬP NHẬT: Thêm tham số newSpeed
        public void Setup(Vector2 _dir, int _dmg, float _newSpeed)
        {
            direction = _dir.normalized;
            damage = _dmg;
            speed = _newSpeed; // Gán tốc độ từ Skill vào đạn

            // Xoay mũi viên đạn
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            Destroy(gameObject, lifetime);
        }

        void FixedUpdate()
        {
            // Đạn tự bay dựa trên speed đã được Setup
            rb.linearVelocity = direction * speed;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) return;

            Gameplay.Entities.Enemy enemy = collision.GetComponent<Gameplay.Entities.Enemy>();
            if (enemy != null)
            {
                // Debug.Log($"Đạn trúng {collision.name}, gây {damage} sát thương!");
                enemy.TakeDamage(damage);
                HitSomething();
            }
            else if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
            {
                HitSomething();
            }
        }

        void HitSomething()
        {
            if (hitVFX != null)
            {
                Instantiate(hitVFX, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}