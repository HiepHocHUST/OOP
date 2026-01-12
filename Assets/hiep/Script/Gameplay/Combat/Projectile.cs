using UnityEngine;
using Gameplay.Entities; // <--- Thêm dòng này

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

        public void Setup(Vector2 _dir, int _dmg, float _newSpeed)
        {
            direction = _dir.normalized;
            damage = _dmg;
            speed = _newSpeed;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            Destroy(gameObject, lifetime);
        }

        void FixedUpdate()
        {
            rb.linearVelocity = direction * speed;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) return;

            // Code gọn hơn nhờ dòng using ở trên
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Đúng logic trừ máu
                HitSomething();
            }
            // Đảm bảo tường/đất có Tag này trong Unity
            else if (collision.CompareTag("Ground"))
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