using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class DraggableItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject objectPrefab; // Prefab 3D khi thả vào Scene
    private GameObject spawnedObject; // Object 3D đang được kéo
    private bool isDragging3DObject = false;

    public Vector3 defaultScale = new Vector3(1, 1, 1); // Kích thước cơ bản khi thả vào Player
    [SerializeField]private RectTransform itemBoard; // Vùng Item Board
    [SerializeField] private RectTransform canvasRectTransform; // Toàn bộ canvas
    [SerializeField] private GameObject UI3DModel;
    public Canvas canvas; // Canvas chính
    public float size;

    void Awake()
    {
       
        // Lấy `RectTransform` của `Item Board`
        itemBoard = GameObject.Find("Board").GetComponent<RectTransform>(); // Đặt đúng tên Object của bạn
        canvasRectTransform = canvas.GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Spawn object prefab tại vị trí con chuột
        if (objectPrefab != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            Plane groundPlane = new Plane(Vector3.forward, Vector3.zero); // Mặt phẳng XY (Z cố định)

            if (groundPlane.Raycast(ray, out float distance))
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

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging3DObject && spawnedObject != null)
        {
            // Di chuyển object theo chuột, chỉ di chuyển trên XY
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            Plane groundPlane = new Plane(Vector3.forward, Vector3.zero); // Mặt phẳng XY (Z cố định)

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
