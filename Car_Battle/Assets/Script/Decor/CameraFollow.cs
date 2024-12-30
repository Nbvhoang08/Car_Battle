using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Đối tượng mà camera sẽ theo dõi

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 5, -10); // Khoảng cách giữa camera và target
    public float smoothSpeed = 0.125f; // Tốc độ mượt khi di chuyển camera

    private void Start()
    {
        target = Player.Instance.transform;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Target not assigned for CameraFollow script!");
            return;
        }

        // Vị trí mong muốn của camera
        Vector3 desiredPosition = target.position + offset;

        // Lerp để di chuyển camera một cách mượt mà
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        //// Đặt vị trí của camera
        transform.position = smoothedPosition;

        // Để camera luôn nhìn về phía target (nếu cần)
        //transform.LookAt(target);
    }
}
