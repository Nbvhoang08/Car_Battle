using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Player : Singleton<Player>
{
    public List<GameObject> attachedItem = new List<GameObject>();
    public Rigidbody rb;
    // Start is called before the first frame update
    [SerializeField]private List<Wheel> wheels = new List<Wheel>(); // Danh sách các bánh xe
    public GameObject chassis;
    public float wheelForce = 10f;    // Lực tác động của bánh xe
    public float turnSpeed = 50f;     // Tốc độ quay bánh xe
    public float horizontalInput;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Gửi lệnh di chuyển tới các bánh xe
        if (wheels.Count > 0)
        {
            foreach (var wheel in wheels)
            {
                if (wheel != null)
                {
                    wheel.HandleMovement(horizontalInput);
                }
            }
        }
    
    }

    // Làm mới danh sách bánh xe từ attachedItem
    public void RefreshWheels()
    {
        wheels.Clear();

        foreach (var item in attachedItem)
        {
            if (item != null)
            {
                var wheel = item.GetComponent<Wheel>();
                if (wheel != null)
                {
                    wheels.Add(wheel);

                    // Khởi tạo bánh xe với các thông số cần thiết
                    wheel.Initialize(GetComponent<Rigidbody>(), wheelForce, turnSpeed);
                }
            }
        }
    }

    // Hàm xóa item và cập nhật danh sách bánh xe


    public void RemoveItem()
    {
        if (attachedItem == null || attachedItem.Count == 0)
        {
            Debug.LogWarning("No items to remove.");
            return;
        }

        // Destroy từng phần tử trong danh sách
        foreach (var item in attachedItem)
        {
            if (item != null) // Kiểm tra null trước khi Destroy
            {
                Destroy(item);
            }
        }

        // Xóa sạch danh sách sau khi Destroy
        attachedItem.Clear();
        wheels.Clear();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra xem có va chạm với Obstacle hay không
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Lấy vị trí va chạm
            Vector3 collisionPoint = collision.contacts[0].point;
            // Xử lý logic phá vỡ trên các object con
            MeshBreaker breaker = chassis.GetComponent<MeshBreaker>();
            if (breaker != null)
            {
                if (collision.relativeVelocity.magnitude > breaker.breakForce)
                {
                    foreach (ContactPoint contact in collision.contacts)
                    {
                        Vector3 hitPoint = collision.contacts[0].point;
                  
                        breaker.BreakAtPoint(hitPoint);
                    }
                }
                 
            }
        }
    }
}
