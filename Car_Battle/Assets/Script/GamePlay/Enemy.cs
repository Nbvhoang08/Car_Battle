using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 5f; // Tốc độ di chuyển của Enemy
    public float chargeSpeed = 10f; // Tốc độ khi lao vào húc
    public float idleTime = 2f; // Thời gian nghỉ giữa các hành động
    public float roamDistance = 5f; // Khoảng cách di chuyển tự do

    public float acceleration;
    private Rigidbody rb;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 roamTarget;

    public List<Wheel> movementWheels = new List<Wheel>(); // Danh sách các Wheel để xử lý di chuyển
    public List<GameObject> Weapon = new List<GameObject>();
    public float hp;
    public float currentHP;
    private enum EnemyState
    {
        Idle,
        Roaming,
        Charging,
        Retreating
    }

    [SerializeField] private EnemyState currentState;
    [SerializeField] private float stateTimer;
    [SerializeField] private float inputDirection; // Đầu vào cho hàm HandleMovement

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        currentState = EnemyState.Idle;
        stateTimer = idleTime;
        foreach (Wheel wheel in movementWheels)
        {
            wheel.Initialize(rb, 10, 720);
        }
        currentHP = hp;
        // Kích hoạt ngẫu nhiên từ 1 đến 3 weapon
        //int activeWeapons = Random.Range(1, 5);
        int activeWeapons = 0;
        List<int> selectedIndices = new List<int>();

        while (selectedIndices.Count < activeWeapons)
        {
            int randomIndex = Random.Range(0, Weapon.Count);
            if (!selectedIndices.Contains(randomIndex))
            {
                selectedIndices.Add(randomIndex);
            }
        }

        for (int i = 0; i < Weapon.Count; i++)
        {
            Weapon[i].SetActive(selectedIndices.Contains(i));
        }
    }

    void Update()
    {


        // Xử lý logic dựa trên trạng thái hiện tại
        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;

            case EnemyState.Roaming:
                HandleRoamingState();
                break;

            case EnemyState.Charging:
                HandleChargingState();
                break;

            case EnemyState.Retreating:
                HandleRetreatingState();
                break;
        }
        if (isKnockedBack) return;
        // Sử dụng các Wheel để xử lý di chuyển
        foreach (var wheel in movementWheels)
        {
            if (wheel != null)
            {
                wheel.HandleMovement(inputDirection, acceleration);
            }
        }
    }

    private void HandleIdleState()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            // Chọn trạng thái tiếp theo ngẫu nhiên
            int nextState = Random.Range(0, 3); // 0: Roaming, 1: Charging, 2: Retreating

            if (nextState == 0)
            {
                SetRoamingState();
            }
            else if (nextState == 1)
            {
                SetChargingState();
            }
            else if (nextState == 2 && Vector3.Distance(transform.position, startPosition) > 1f)
            {
                SetRetreatingState();
            }
        }
    }

    private void SetRoamingState()
    {
        float randomX = Random.Range(0, 2) == 0 ? Random.Range(-roamDistance, -2f) : Random.Range(2f, roamDistance);
        roamTarget = startPosition + new Vector3(randomX, 0, 0);
        currentState = EnemyState.Roaming;

        // Thiết lập hướng di chuyển đúng mục tiêu roamTarget
        Vector3 directionToTarget = (roamTarget - new Vector3(transform.position.x, 0, 0)).normalized;
        inputDirection = directionToTarget.x > 0 ? 1f : -1f; // Đặt hướng theo X
    }

    private void HandleRoamingState()
    {
     

        if (Mathf.Abs(transform.position.x - roamTarget.x) <= 1f)
        {
            SetIdleState();
        }
    }

    private void SetChargingState()
    {
        currentState = EnemyState.Charging;
        acceleration = Random.Range(5, 10);
        stateTimer = Random.Range(1f, 3f); // Thời gian húc
        inputDirection = -1f; // Hướng di chuyển về phía trước
    }

    private void HandleChargingState()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            SetIdleState();
        }
    }

    private void SetRetreatingState()
    {
        currentState = EnemyState.Retreating;
        roamTarget = startPosition; // Quay về vị trí ban đầu
        stateTimer = 3;
        // Thiết lập hướng di chuyển đúng mục tiêu startPosition
        Vector3 directionToTarget = (roamTarget - new Vector3(transform.position.x, 0, 0)).normalized;
        inputDirection = directionToTarget.x > 0 ? 1f : -1f; // Đặt hướng theo X
    }

    private void HandleRetreatingState()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            SetIdleState();
        }
    }

    private void SetIdleState()
    {
        currentState = EnemyState.Idle;
        acceleration = 0;
        stateTimer = idleTime;
        inputDirection = 0f; // Dừng di chuyển
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (currentState == EnemyState.Roaming)
            {
                // Random chọn 1 trong 2: đổi hướng hoặc chuyển sang trạng thái Idle
                if (Random.value > 0.5f)
                {
                    inputDirection = -inputDirection; // Đổi hướng nhưng không reset target
                }
                else
                {
                    SetIdleState(); // Chuyển sang trạng thái Idle
                }
            }
            else
            {
                // Chuyển sang trạng thái Idle nếu không phải Roaming
                SetIdleState();
            }
        }
        // Kiểm tra va chạm giữa Weapon và Weapon
        if (collision.gameObject.CompareTag("PlayerWeapon") && this.gameObject.CompareTag("EnemyWeapon"))
        {
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
                    Vector3 knockbackDirection = (transform.position - collision.transform.position).normalized;
                    StartCoroutine(ApplyKnockback(knockbackDirection, 5)); // Lực knockback = 10
                }
            }
        }
        // Kiểm tra va chạm giữa Weapon và đối tượng cha
        else if (collision.gameObject.CompareTag("Player") && this.gameObject.CompareTag("EnemyWeapon"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                float collisionSpeed = rb.velocity.magnitude;

                // Áp dụng sát thương
                int baseDamage = 30; // Sát thương cơ bản
                int additionalDamage = Mathf.FloorToInt(collisionSpeed * 2); // Tăng sát thương dựa trên vận tốc
                int totalDamage = baseDamage + additionalDamage;

                ApplyDamage(collision, totalDamage);

                // Nếu vận tốc lớn hơn 5, áp dụng knockback
                if (collisionSpeed > 1.5f)
                {
                    Vector3 knockbackDirection = (transform.position - collision.transform.position).normalized;
                    StartCoroutine(ApplyKnockback(knockbackDirection, 5)); // Lực knockback = 10
                }
            }
        }
        // Kiểm tra va chạm giữa Player và Enemy
        else if (collision.gameObject.CompareTag("Player") && this.gameObject.CompareTag("Enemy"))
        {
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
                    Vector3 knockbackDirection = (transform.position - collision.transform.position).normalized;
                    StartCoroutine(ApplyKnockback(knockbackDirection, 5)); // Lực knockback = 10
                }
            }
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
    // H

    private void OnTriggerEnter(Collider collision)
    {
        // Kiểm tra va chạm giữa Weapon và đối tượng cha
        if (collision.CompareTag("Player"))
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

    // Hàm áp dụng sát thương cho cả hai đối tượng

    public bool isDealingDamage = false; // Đánh dấu trạng thái gây sát thương
    private Coroutine damageCoroutine; // Để theo dõi Coroutine hiện tại

    private void OnCollisionStay(Collision collision)
    {
        if ((collision.gameObject.CompareTag("PlayerWeapon") && this.gameObject.CompareTag("EnemyWeapon")) ||         
            (collision.gameObject.CompareTag("Player") && this.gameObject.CompareTag("Enemy")))
        {
            if (!isDealingDamage) // Chỉ bắt đầu gây sát thương nếu chưa có Coroutine nào đang chạy
            {
                damageCoroutine = StartCoroutine(DealDamageOverTime(collision, 0.5f, 7)); // Gây sát thương 5 mỗi 0.5 giây
             
            }
        }
        else if (collision.gameObject.CompareTag("Player") && this.gameObject.CompareTag("EnemyWeapon"))
        {
            if (!isDealingDamage) // Chỉ bắt đầu gây sát thương nếu chưa có Coroutine nào đang chạy
            {
                damageCoroutine = StartCoroutine(DealDamageOverTime(collision, 0.5f, 20)); // Gây sát thương 5 mỗi 0.5 giây
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

    private IEnumerator DealDamageOverTime(Collision collision, float interval, int damage)
    {
        isDealingDamage = true;

        // Lưu trữ object root
        Transform rootTransform = collision.transform.root;

        while (true)
        {
        
            // Gây sát thương cho root object
            ApplyDamageToRoot(rootTransform, damage);

            yield return new WaitForSeconds(interval); // Đợi 0.5 giây
        }
    }

    private void ApplyDamageToRoot(Transform rootTransform, int damage)
    {
        var player = rootTransform.GetComponent<Player>();
        if (player != null)
        {
            player.currentHP -= damage;
        }
       
    }
    private void ApplyDamage(Collision collision, int damage)
    {
        // Tìm object gốc cấp cao nhất
        Transform rootTransform = collision.transform.root;
        // Kiểm tra xem rootTransform có chứa script Player hoặc Enemy không
        var player = rootTransform.GetComponent<Player>();
        if (player != null)
        {
            player.currentHP -= damage; // Gây sát thương cho Player
            return;
        }


    }

}

