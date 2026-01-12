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
        private bool canMove = true;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (!canMove) return;

            // 1. DI CHUYỂN (CHỈ DÙNG MŨI TÊN TRÁI/PHẢI)
            float moveX = 0f;

            // Nếu giữ mũi tên phải -> moveX = 1
            if (Input.GetKey(KeyCode.RightArrow))
            {
                moveX = 1f;
            }
            // Nếu giữ mũi tên trái -> moveX = -1
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveX = -1f;
            }
            // Nếu không bấm gì (hoặc bấm A/D) -> moveX vẫn bằng 0 (đứng im)

            rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);

            // 2. QUAY MẶT
            if (moveX > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (moveX < 0) transform.localScale = new Vector3(-1, 1, 1);

            // 3. GỬI LỆNH CHO ANIMATOR
            if (anim != null)
            {
                anim.SetFloat("Speed", Mathf.Abs(moveX));
            }

            // 4. NHẢY (CHỈ DÙNG MŨI TÊN LÊN)
            // Đã xóa KeyCode.Space, chỉ để lại UpArrow
            if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                isGrounded = false;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
        }

        public void SetMobility(bool state)
        {
            canMove = state;
            if (!state && rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}