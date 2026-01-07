using UnityEngine;

namespace Gameplay.Entities
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Settings")]
        public float moveSpeed = 5f;
        public float jumpForce = 12f;

        private Rigidbody2D rb;
        private Animator anim;
        private bool isGrounded = true;
        private bool canMove = true; // Biến kiểm soát việc cho phép di chuyển

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            // Nếu bị khóa di chuyển (do chết hoặc choáng) thì dừng lại
            if (!canMove) return;

            // 1. DI CHUYỂN
            float moveX = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);

            // 2. QUAY MẶT
            if (moveX > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (moveX < 0) transform.localScale = new Vector3(-1, 1, 1);

            // 3. GỬI LỆNH CHO ANIMATOR
            if (anim != null)
            {
                anim.SetFloat("Speed", Mathf.Abs(moveX));
            }

            // 4. NHẢY
            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                isGrounded = false;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
        }

        // --- HÀM MÀ FILE PLAYER ĐANG TÌM KIẾM (ĐÂY RỒI!) ---
        public void SetMobility(bool state)
        {
            canMove = state;
            if (!state && rb != null)
            {
                rb.linearVelocity = Vector2.zero; // Dừng hẳn quán tính
            }
        }
    }
}