using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    public bool useLimit = true;
    public float minX, maxX;
    public float minY, maxY;

    // --- ĐOẠN MỚI THÊM VÀO ---
    void Start()
    {
        // Nếu chưa gán target bằng tay, thì tự động đi tìm thằng nào có Tag là "Player"
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }
    }
    // -------------------------

    void FixedUpdate()
    {
        if (target == null) return; // Nếu tìm không thấy thì đứng im

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        if (useLimit)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);
        }

        transform.position = smoothedPosition;
    }
}