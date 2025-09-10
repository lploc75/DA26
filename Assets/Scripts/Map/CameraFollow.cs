//using UnityEngine;

//public class CameraFollow : MonoBehaviour
//{
//    public Transform target;     // nhân vật để camera theo dõi
//    public float smoothSpeed = 5f;  // độ mượt khi theo dõi
//    public Vector3 offset;       // khoảng cách giữa camera và player

//    void LateUpdate()
//    {
//        if (target == null) return;

//        // vị trí mong muốn
//        Vector3 desiredPosition = target.position + offset;

//        // di chuyển mượt
//        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

//        // gán cho camera
//        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
//    }
//}
