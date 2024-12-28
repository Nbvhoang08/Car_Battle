using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Player : Singleton<Player>
{
    public List<GameObject> attachedItem = new List<GameObject>();
    public Rigidbody rb;
    // Start is called before the first frame update
    [SerializeField] private List<Wheel> wheels = new List<Wheel>(); // Danh sách các bánh xe
    public GameObject chassis;
    public float wheelForce = 10f;    // Lực tác động của bánh xe
    public float turnSpeed = 50f;     // Tốc độ quay bánh xe
    public float horizontalInput;
    public float hp;
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
                    wheel.HandleMovement(horizontalInput, 0);
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


    private void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra va chạm giữa Weapon và Weapon
        if ((collision.gameObject.CompareTag("PlayerWeapon") && this.gameObject.CompareTag("EnemyWeapon")) ||
            (collision.gameObject.CompareTag("EnemyWeapon") && this.gameObject.CompareTag("PlayerWeapon")))
        {
            // Gây ít damage hơn khi vũ khí va chạm với nhau
            Debug.Log("Weapon collided with another weapon! Less damage applied.");
            ApplyDamage(collision, 5); // Ví dụ: giảm sát thương còn 5
        }
        // Kiểm tra va chạm giữa Weapon và đối tượng cha
        else if ((collision.gameObject.CompareTag("Player") && this.gameObject.CompareTag("EnemyWeapon")) ||
                 (collision.gameObject.CompareTag("Enemy") && this.gameObject.CompareTag("PlayerWeapon")))
        {
            // Gây damage lớn hơn khi vũ khí va chạm với đối tượng cha
            Debug.Log("Weapon collided with a player or enemy! Full damage applied.");
            ApplyDamage(collision, 20); // Ví dụ: sát thương đầy đủ là 20
        }
        // Kiểm tra va chạm giữa Player và Enemy
        else if (collision.gameObject.CompareTag("Player") && this.gameObject.CompareTag("Enemy") ||
                 collision.gameObject.CompareTag("Enemy") && this.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collided with Enemy!");
            ApplyMutualDamage(collision, 15); // Gây sát thương cho cả hai
        }
    }

    // Hàm áp dụng sát thương
    private void ApplyDamage(Collision collision, int damage)
    {
        hp -= 1;
    }

    // Hàm áp dụng sát thương cho cả hai đối tượng
    private void ApplyMutualDamage(Collision collision, int damage)
    {
        // Gây sát thương cho Enemy
        var enemyHP = GetComponent<Enemy>();
        if (enemyHP != null)
        {
            enemyHP.hp -= damage;
        }

        // Gây sát thương cho Player
        hp -= damage;
    }
}
