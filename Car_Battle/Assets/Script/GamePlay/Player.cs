using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
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
    public float currentHP;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHP = hp;
    }

    // Update is called once per frame
    void Update()
    {
        if (isKnockedBack)
        {
            return;
        }
        //transform.position.z = 0.5f;
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

    public GameObject Exploresion;
    private void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra va chạm giữa Player và Enemy
        if (this.gameObject.CompareTag("Player") && collision.gameObject.CompareTag("Enemy"))
        {
            // Lấy vận tốc tại thời điểm va chạm
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                float collisionSpeed = rb.velocity.magnitude;

                // Áp dụng sát thương
                int baseDamage = 9; // Sát thương cơ bản
                int additionalDamage = Mathf.FloorToInt(collisionSpeed * 2); // Tăng sát thương dựa trên vận tốc
                int totalDamage = baseDamage + additionalDamage;

                ApplyDamage(collision, totalDamage);
                // Nếu vận tốc lớn hơn 5, áp dụng knockback
                if (collisionSpeed > 1.5f)
                {
                  
                    if (collision.contacts.Length > 0) // Kiểm tra xem có contact point không
                    {
                        Vector3 contactPoint = collision.contacts[0].point;

                        // Tạo hiệu ứng SparkEffect tại vị trí va chạm
                        if (SparkEffect != null)
                        {
                            Instantiate(Exploresion, contactPoint, Quaternion.identity);
                            SoundManager.Instance.PlayVFXSound(6);
                        }
                    }
                    Vector3 knockbackDirection = (transform.position - collision.transform.position).normalized;
                    StartCoroutine(ApplyKnockback(knockbackDirection, 5)); // Lực knockback = 10
                }
            }
        }
        else if (this.gameObject.CompareTag("PlayerWeapon") && collision.gameObject.CompareTag("EnemyWeapon"))
        {
            // Lấy vận tốc tại thời điểm va chạm
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                float collisionSpeed = rb.velocity.magnitude;

                // Áp dụng sát thương
                int baseDamage = 8; // Sát thương cơ bản
                int additionalDamage = Mathf.FloorToInt(collisionSpeed * 2); // Tăng sát thương dựa trên vận tốc
                int totalDamage = baseDamage + additionalDamage;

                ApplyDamage(collision, totalDamage);

                // Nếu vận tốc lớn hơn 5, áp dụng knockback
                if (collisionSpeed > 1.5f)
                {
                    
                    if (collision.contacts.Length > 0) // Kiểm tra xem có contact point không
                    {
                        Vector3 contactPoint = collision.contacts[0].point;

                        // Tạo hiệu ứng SparkEffect tại vị trí va chạm
                        if (SparkEffect != null)
                        {
                            Instantiate(Exploresion, contactPoint, Quaternion.identity);
                            SoundManager.Instance.PlayVFXSound(6);
                        }
                    }
                    Vector3 knockbackDirection = (transform.position - collision.transform.position).normalized;
                    StartCoroutine(ApplyKnockback(knockbackDirection, 5)); // Lực knockback = 10
                }
            }
        }
        else if (this.gameObject.CompareTag("PlayerWeapon") && collision.gameObject.CompareTag("Enemy"))
        {
            // Lấy vận tốc tại thời điểm va chạm
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                float collisionSpeed = rb.velocity.magnitude;

                // Áp dụng sát thương
                int baseDamage = 17; // Sát thương cơ bản
                int additionalDamage = Mathf.FloorToInt(collisionSpeed * 2); // Tăng sát thương dựa trên vận tốc
                int totalDamage = baseDamage + additionalDamage;

                ApplyDamage(collision, totalDamage);

                // Nếu vận tốc lớn hơn 5, áp dụng knockback
                if (collisionSpeed > 1.5f)
                {
                    // Lấy vị trí va chạm từ collision
                    if (collision.contacts.Length > 0) // Kiểm tra xem có contact point không
                    {
                        Vector3 contactPoint = collision.contacts[0].point;

                        // Tạo hiệu ứng SparkEffect tại vị trí va chạm
                        if (SparkEffect != null)
                        {
                            Instantiate(Exploresion, contactPoint, Quaternion.identity);
                            SoundManager.Instance.PlayVFXSound(6);
                        }
                    }
                    Vector3 knockbackDirection = (transform.position - collision.transform.position).normalized;
                    StartCoroutine(ApplyKnockback(knockbackDirection, 5)); // Lực knockback = 10
                }
            }
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        // Kiểm tra va chạm giữa Weapon và đối tượng cha
        if (collision.CompareTag("Enemy"))
        {
            ApplyDamageTrigger(collision, 5);

        }
        // Kiểm tra va chạm giữa Weapon và đối tượng cha

    }


    private void ApplyDamageTrigger(Collider collision, int damage)
    {

        // Tìm object gốc cấp cao nhất chứa script Player hoặc Enemy
        Transform currentTransform = collision.transform;

        while (currentTransform != null)
        {
            // Tìm component Player
            var player = currentTransform.GetComponent<Player>();
            if (player != null)
            {
                player.currentHP -= damage; // Gây sát thương cho Player
                return;
            }

            // Di chuyển lên cấp cha
            currentTransform = currentTransform.parent;
        }
    }
    public bool isKnockedBack = false; // Biến cờ để kiểm tra trạng thái knockback
    private float knockbackDuration = 0.5f; // Thời gian knockback
    private IEnumerator ApplyKnockback(Vector3 knockbackDirection, float force)
    {
        isKnockedBack = true; // Kích hoạt trạng thái knockback

        // Lấy Rigidbody của đối tượng để áp dụng lực knockback
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Chỉ áp dụng lực trên trục X
            knockbackDirection = new Vector3(knockbackDirection.x, 0f, 0f).normalized;

            // Áp dụng lực knockback
            rb.AddForce(knockbackDirection * force, ForceMode.Impulse);
        }

        // Dừng di chuyển trong khoảng thời gian knockback
        yield return new WaitForSeconds(knockbackDuration);

        isKnockedBack = false; // Cho phép di chuyển lại
    }
    // Hàm áp dụng sát thương
    private void ApplyDamage(Collision collision, float damage)
    {
        // Tìm object gốc cấp cao nhất chứa script Player hoặc Enemy
        Transform currentTransform = collision.transform;
        while (currentTransform != null)
        {
            // Tìm component Enemy
            var enemy = currentTransform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.currentHP -= damage; // Gây sát thương cho Enemy
                return;
            }

            // Di chuyển lên cấp cha
            currentTransform = currentTransform.parent;
        }
    }


    private bool isDealingDamage = false; // Biến kiểm tra xem có đang gây sát thương hay không
    private Coroutine damageCoroutine; // Để theo dõi Coroutine hiện tại
    private void OnCollisionStay(Collision collision)
    {
        // Kiểm tra các điều kiện tương tự OnCollisionEnter
        if ((collision.gameObject.CompareTag("EnemyWeapon") && this.gameObject.CompareTag("PlayerWeapon")) ||      
            (collision.gameObject.CompareTag("Enemy") && this.gameObject.CompareTag("Player")))
        {
            if (!isDealingDamage) // Chỉ bắt đầu gây sát thương nếu chưa có Coroutine nào đang chạy
            {
                damageCoroutine = StartCoroutine(DealDamageOverTime(collision, 0.5f, 7.5f)); // Gây sát thương 5 mỗi 0.5 giây
                
            }
        }
        else if(collision.gameObject.CompareTag("Enemy") && this.gameObject.CompareTag("PlayerWeapon"))
        {
            if (!isDealingDamage) // Chỉ bắt đầu gây sát thương nếu chưa có Coroutine nào đang chạy
            {
                damageCoroutine = StartCoroutine(DealDamageOverTime(collision, 0.5f, 21));

            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Khi va chạm kết thúc, dừng Coroutine và đánh dấu trạng thái
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
            isDealingDamage = false;

        }
    }
    public GameObject SparkEffect;
    private IEnumerator DealDamageOverTime(Collision collision, float interval, float damage)
    {
        isDealingDamage = true; // Đánh dấu bắt đầu gây sát thương

        while (true) // Vòng lặp gây sát thương liên tục
        {
            // Gây sát thương cho cả hai đối tượng
            if (collision != null)
            {
                ApplyDamage(collision, damage);

            }
            // Lấy vị trí va chạm từ collision
            if (collision.contacts.Length > 0) // Kiểm tra xem có contact point không
            {
                Vector3 contactPoint = collision.contacts[0].point;

                // Tạo hiệu ứng SparkEffect tại vị trí va chạm
                if (SparkEffect != null)
                {
                    Instantiate(SparkEffect, contactPoint, Quaternion.identity);
                    SoundManager.Instance.PlayVFXSound(5);
                }
            }

            yield return new WaitForSeconds(interval); // Đợi 0.5 giây
        }
    }
}
