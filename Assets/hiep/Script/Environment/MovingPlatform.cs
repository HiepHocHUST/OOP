using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform diemDen; // Kéo 1 cái Gameobject rỗng làm đích đến vào đây
    public float tocDo = 3f;
    private Vector3 diemA;
    private Vector3 diemB;
    private Vector3 mucTieu;

    void Start()
    {
        diemA = transform.position; // Điểm bắt đầu là chỗ đặt đám mây
        diemB = diemDen.position;   // Điểm đến
        mucTieu = diemB;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, mucTieu, tocDo * Time.deltaTime);
        if (Vector3.Distance(transform.position, mucTieu) < 0.1f)
        {
            mucTieu = (mucTieu == diemA) ? diemB : diemA; // Đổi chiều
        }
    }

    // Giữ chân nhân vật khi đứng lên mây
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) collision.transform.SetParent(transform);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!gameObject.activeInHierarchy) return;
        if (collision.gameObject.CompareTag("Player")) collision.transform.SetParent(null);
    }
}