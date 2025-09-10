using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;       // Player hoặc object muốn follow

    [Header("Offset & Smooth")]
    public Vector3 offset = new Vector3(0, 0, -10f); // Camera giữ -10f để nhìn thấy
    public float smoothSpeed = 5f; // Tốc độ lerp

    void LateUpdate()
    {
        if (target == null) return;

        // Vị trí mong muốn
        Vector3 desiredPosition = target.position + offset;

        // Lerp để mượt
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}
