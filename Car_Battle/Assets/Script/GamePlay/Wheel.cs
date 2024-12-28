using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    private Rigidbody playerRigidbody; // Tham chiếu tới Rigidbody của Player
    private float wheelForce = 10f;    // Lực tác động của bánh xe
    public float turnSpeed = 100f;    // Tốc độ quay bánh xe
    public GameObject WheelModel;
    // Khởi tạo bánh xe với Rigidbody của Player
    public void Initialize(Rigidbody playerRb, float force, float speed)
    {
        playerRigidbody = playerRb;
        wheelForce = force;
        turnSpeed = speed;
    }

    // Xử lý logic bánh xe
    public void HandleMovement(float input, float acceleration)
    {

        if (playerRigidbody == null) return;

        // Tính toán vận tốc theo hướng di chuyển
        Vector3 velocity = transform.right * input * (wheelForce+acceleration);

        // Giữ nguyên vận tốc trên trục Y (trọng lực)
        velocity.z = 0; // Đảm bảo không di chuyển trên trục Z
        velocity.y = playerRigidbody.velocity.y; // Giữ trọng lực

        // Cập nhật vận tốc trực tiếp cho Player
        playerRigidbody.velocity = velocity;

        // Xoay bánh xe (cho hiệu ứng hình ảnh)
        WheelModel.transform.Rotate(Vector3.forward, -input * turnSpeed * Time.deltaTime);
    }
}
