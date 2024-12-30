using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class DraggableItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
{
    [SerializeField] private GameObject objectPrefab; // Prefab 3D khi thả vào Scene
    public GameObject spawnedObject; // Object 3D đang được kéo
    private bool isDragging3DObject = false;

    public Vector3 defaultScale = new Vector3(1, 1, 1); // Kích thước cơ bản khi thả vào Player
    [SerializeField]private RectTransform itemBoard; // Vùng Item Board
    [SerializeField] private RectTransform canvasRectTransform; // Toàn bộ canvas
    [SerializeField] private GameObject UI3DModel;
    public Canvas canvas; // Canvas chính
    public float size;
    public int price;
    public Text PriceText;
    public Text Log;
    void Awake()
    {
       
        // Lấy `RectTransform` của `Item Board`
        itemBoard = GameObject.Find("Board").GetComponent<RectTransform>(); // Đặt đúng tên Object của bạn
        canvasRectTransform = canvas.GetComponent<RectTransform>();
    }
    void Start()
    {
        Log.text = " ";
    }
    void Update()
    {
        if (PriceText != null)
        {
            PriceText.text = price.ToString();
        }
    }
    private Vector2 pointerDownPosition; // Vị trí chuột khi nhấn
    private const float dragThreshold = 5f; // Ngưỡng để xác định drag (pixel)
    public void OnPointerDown(PointerEventData eventData)
    {

        pointerDownPosition = eventData.position; // Lưu vị trí chuột

        // Các logic spawn object như trước
        if (objectPrefab != null)
        {
            if (price > CoinManager.Instance.GetCoins())
            {
                Log.text = "Not enough coins";
                SoundManager.Instance.PlayVFXSound(7);
                StartCoroutine(LogError());
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            Plane groundPlane = new Plane(Vector3.forward, Vector3.zero); // Mặt phẳng XY (Z cố định)

            if (groundPlane.Raycast(ray, out float distance) && spawnedObject == null)
            {
                Vector3 spawnPosition = ray.GetPoint(distance);
                spawnedObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
                isDragging3DObject = true;

                // Đặt kích thước của object 3D giống với kích thước UI
                RectTransform rectTransform = GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    Vector2 uiSize = rectTransform.rect.size;
                    spawnedObject.transform.localScale = new Vector3(size, size, size); // Đặt kích thước theo UI
                }
                UI3DModel.SetActive(false);
                SoundManager.Instance.PlayVFXSound(4);
            }
        }
    }
   

    IEnumerator LogError()
    {
        yield return new WaitForSeconds(0.5f);
        Log.text = " ";
    }
    public void OnDrag(PointerEventData eventData)
    {
       
        if (isDragging3DObject && spawnedObject != null)
        {
            // Di chuyển object theo chuột, chỉ di chuyển trên XY
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            Plane groundPlane = new Plane(Vector3.forward, new Vector3(0.5f, 0.5f, 0.5f)); // Mặt phẳng XY (Z cố định)

            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 dragPosition = ray.GetPoint(distance);
                spawnedObject.transform.position = dragPosition; // Cập nhật vị trí
            }

            // Kiểm tra vùng hiện tại (Item Board hoặc Equip Zone)
            if (IsPointerOverItemBoard(eventData))
            {
                // Đặt kích thước giống UI
                RectTransform rectTransform = GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    Vector2 uiSize = rectTransform.rect.size;
                    spawnedObject.transform.localScale = new Vector3(size, size, size);
                }
            }
            else
            {
                // Đặt kích thước về default
                spawnedObject.transform.localScale = defaultScale;
            }
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        UI3DModel.SetActive(true);

        if (isDragging3DObject && spawnedObject != null)
        {
            float sphereRadius = 0.5f; // Bán kính vùng kiểm tra
            Vector3 origin = spawnedObject.transform.position;

            // Kiểm tra tất cả các collider trong phạm vi bán kính
            Collider[] colliders = Physics.OverlapSphere(origin, sphereRadius);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player")) // Kiểm tra tag Player
                {
                    AttachObjectToPlayer(origin, collider.transform);
                    spawnedObject = null;
                    isDragging3DObject = false;
                    return;
                }
            }
            Destroy(spawnedObject);
            isDragging3DObject = false;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        UI3DModel.SetActive(true);

        if (isDragging3DObject && spawnedObject != null)
        {
            float sphereRadius = 0.5f; // Bán kính vùng kiểm tra
            Vector3 origin = spawnedObject.transform.position;

            // Kiểm tra tất cả các collider trong phạm vi bán kính
            Collider[] colliders = Physics.OverlapSphere(origin, sphereRadius);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player")) // Kiểm tra tag Player
                {
                    AttachObjectToPlayer(origin, collider.transform);
                    spawnedObject = null;
                    isDragging3DObject = false;
               
                    return;
                }
            }
            Destroy(spawnedObject);
            isDragging3DObject = false;
        }
    }

    private void AttachObjectToPlayer(Vector3 hitPosition, Transform playerTransform)
    {
        // Đặt spawnedObject làm con của Player
        spawnedObject.transform.SetParent(playerTransform);

        // Đặt vị trí theo điểm va chạm (hitPosition)
        spawnedObject.transform.position = hitPosition;

        // Giữ nguyên rotation hoặc tuỳ chỉnh theo nhu cầu
        spawnedObject.transform.rotation = Quaternion.identity;

        // Đặt kích thước của object về defaultScale
        spawnedObject.transform.localScale = Vector3.one;
        SoundManager.Instance.PlayVFXSound(3);
        Player.Instance.attachedItem.Add(spawnedObject);
        CoinManager.Instance.RemoveCoins(price);
    }

    private bool IsPointerOverItemBoard(PointerEventData eventData)
    {
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            itemBoard,
            eventData.position,
            canvas.worldCamera,
            out localMousePosition
        );

        // Kiểm tra nếu con trỏ chuột nằm trong vùng `Item Board`
        return itemBoard.rect.Contains(localMousePosition);
    }
}
