using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerEquipment : MonoBehaviour
{
      public Transform equipmentParent;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    // Phương thức này được gọi từ DraggableItem khi item được thả
    public bool TryEquipItem(GameObject uiItem, Vector3 dropPosition)
    {
        // Chuyển đổi vị trí drop từ screen space sang world space
        Ray ray = mainCamera.ScreenPointToRay(dropPosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject) // Kiểm tra nếu hit đúng player
            {
                GameObject sceneItem = CreateSceneItem(uiItem);
                AttachItemToPlayer(sceneItem, hit.point);
                return true;
            }
        }
        
        return false;
    }

    private GameObject CreateSceneItem(GameObject uiItem)
    {
        string prefabName = uiItem.GetComponent<ItemData>().prefabName;
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");
        return Instantiate(prefab, equipmentParent);
    }

    private void AttachItemToPlayer(GameObject item, Vector3 hitPoint)
    {
        // Tính toán vị trí gắn dựa trên điểm hit
        Vector3 localHitPoint = transform.InverseTransformPoint(hitPoint);
        item.transform.localPosition = localHitPoint;
        
        // Có thể thêm logic để snap vào các socket point
        // SnapToNearestSocket(item, localHitPoint);
        
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale = Vector3.one;
    }
    
    // Optional: Thêm phương thức để snap item vào các điểm định sẵn
    private void SnapToNearestSocket(GameObject item, Vector3 hitPoint)
    {
        // Implement logic để tìm socket gần nhất
        // Và snap item vào đó
    }
}

