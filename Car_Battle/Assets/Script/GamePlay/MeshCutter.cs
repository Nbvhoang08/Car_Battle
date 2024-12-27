using EzySlice;
using System.Collections.Generic;
using UnityEngine;

public class MeshCutter : MonoBehaviour
{
    [SerializeField] private Material fractureMaterial; // Vật liệu áp dụng cho các mảnh vỡ
    [SerializeField] private int fractureCount = 5; // Số lượng mảnh muốn tạo
    [SerializeField] private float explosionForce = 100f; // Lực tác động lên các mảnh
  

    public void FractureObject(Vector3 collisionPoint)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null || !meshFilter.mesh.isReadable)
        {
            Debug.LogError("Mesh is not readable! Enable Read/Write in Import Settings.");
            return;
        }

        Mesh originalMesh = meshFilter.mesh;

        // Tạo danh sách các mảnh vỡ
        List<Mesh> fracturedMeshes = FractureMesh(originalMesh, fractureCount);

        // Duyệt qua các mảnh để tạo GameObject
        foreach (Mesh fracturedMesh in fracturedMeshes)
        {
            GameObject fragment = CreateFragment(fracturedMesh, collisionPoint);
            ApplyExplosionForce(fragment, collisionPoint);
        }

        // Xóa GameObject gốc
        Destroy(gameObject);
    }

    private List<Mesh> FractureMesh(Mesh originalMesh, int fractureCount)
    {
        List<Mesh> fracturedMeshes = new List<Mesh>();

        // Chia Mesh thành các phần nhỏ (giả sử bạn tạo một số lượng `fractureCount` mảnh ngẫu nhiên)
        for (int i = 0; i < fractureCount; i++)
        {
            Mesh newMesh = Instantiate(originalMesh); // Sao chép Mesh gốc
            // Tùy chỉnh `newMesh` bằng cách xóa bớt hoặc thay đổi các mặt (triangles)

            // Dùng thuật toán để chia nhỏ mảnh
            // (Đây là bước cần logic tùy chỉnh - bạn có thể tự viết hoặc dùng thư viện hỗ trợ)

            fracturedMeshes.Add(newMesh);
        }

        return fracturedMeshes;
    }

    private GameObject CreateFragment(Mesh mesh, Vector3 position)
    {
        // Tạo mảnh vỡ
        GameObject fragment = new GameObject("Fragment");
        fragment.transform.position = position;

        // Thêm MeshFilter và MeshRenderer
        MeshFilter meshFilter = fragment.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = fragment.AddComponent<MeshRenderer>();
        meshRenderer.material = fractureMaterial;

        // Thêm Collider và Rigidbody cho mảnh vỡ
        MeshCollider collider = fragment.AddComponent<MeshCollider>();
        collider.convex = true; // Convex bắt buộc cho Rigidbody

        Rigidbody rb = fragment.AddComponent<Rigidbody>();
        rb.mass = 0.1f; // Giảm khối lượng cho mảnh

        return fragment;
    }

    private void ApplyExplosionForce(GameObject fragment, Vector3 collisionPoint)
    {
        Rigidbody rb = fragment.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 explosionDirection = (fragment.transform.position - collisionPoint).normalized;
            rb.AddForce(explosionDirection * explosionForce, ForceMode.Impulse);
        }
    }

}
