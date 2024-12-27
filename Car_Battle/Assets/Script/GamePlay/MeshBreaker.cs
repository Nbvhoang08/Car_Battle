using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBreaker : MonoBehaviour
{
    public float breakForce = 10f; // Lực tác động
    [SerializeField] private float breakRadius = 0.5f; // Bán kính vùng vỡ
    [SerializeField] private int maxDebrisPieces = 10; // Số mảnh tối đa được tạo

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void BreakAtPoint(Vector3 hitPoint)
    {
        // Chuyển điểm va chạm sang local space
        Vector3 localHitPoint = transform.InverseTransformPoint(hitPoint);

        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector2[] uvs = mesh.uv;

        // Tìm các tam giác trong vùng va chạm
        List<int> affectedTriangles = new List<int>();
        HashSet<int> affectedVertices = new HashSet<int>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 triCenter = (vertices[triangles[i]] + vertices[triangles[i + 1]] + vertices[triangles[i + 2]]) / 3f;
            float distance = Vector3.Distance(triCenter, localHitPoint);

            if (distance <= breakRadius)
            {
                affectedTriangles.Add(i);
                affectedVertices.Add(triangles[i]);
                affectedVertices.Add(triangles[i + 1]);
                affectedVertices.Add(triangles[i+ 2]);
            }
        }

        if (affectedTriangles.Count == 0) return;

        // Tạo mảnh vỡ từ các tam giác bị ảnh hưởng
        CreateDebrisPieces(affectedTriangles, vertices, uvs, triangles, hitPoint);
        RemoveAffectedTriangles(mesh, affectedTriangles);
      
    }
    private void RemoveAffectedTriangles(Mesh mesh, List<int> affectedTriangles)
    {
        // Tạo danh sách tam giác mới
        List<int> newTriangles = new List<int>();
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUVs = new List<Vector2>();

        // Lấy mảng các tam giác, đỉnh và UV cũ
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector2[] uvs = mesh.uv;

        // Map từ các đỉnh đã bị loại bỏ sang các đỉnh mới
        Dictionary<int, int> vertexMap = new Dictionary<int, int>();

        // Duyệt qua tất cả các tam giác và chỉ thêm các tam giác không bị loại bỏ
        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (!affectedTriangles.Contains(i))  // Nếu tam giác này không bị loại bỏ
            {
                // Thêm đỉnh của tam giác vào danh sách mới
                for (int j = 0; j < 3; j++)
                {
                    int originalVertexIndex = triangles[i + j];
                    if (!vertexMap.ContainsKey(originalVertexIndex))
                    {
                        // Nếu đỉnh chưa được thêm vào danh sách, thêm nó
                        vertexMap[originalVertexIndex] = newVertices.Count;
                        newVertices.Add(vertices[originalVertexIndex]);
                        newUVs.Add(uvs[originalVertexIndex]);
                    }

                    // Thêm tam giác mới vào danh sách
                    newTriangles.Add(vertexMap[originalVertexIndex]);
                }
            }
        }

        // Tạo mesh mới với các đỉnh và tam giác đã được thay đổi
        Mesh newMesh = new Mesh
        {
            vertices = newVertices.ToArray(),
            triangles = newTriangles.ToArray(),
            uv = newUVs.ToArray()
        };

        // Tính toán lại các thông tin của mesh
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        // Cập nhật lại mesh và collider
        meshFilter.mesh = newMesh;
        UpdateMeshCollider();
    }

    private void UpdateMeshCollider()
    {
        if (meshCollider != null)
        {
            Mesh sourceMesh = meshFilter.mesh;

            Mesh cleanedMesh = new Mesh
            {
                vertices = sourceMesh.vertices,
                triangles = sourceMesh.triangles
            };

            cleanedMesh.RecalculateNormals();
            cleanedMesh.RecalculateBounds();

            // Cập nhật MeshCollider với mesh mới
            meshCollider.sharedMesh = null;  // Xóa mesh cũ
            meshCollider.sharedMesh = cleanedMesh;  // Cập nhật mesh mới

            // Đặt collider làm convex
            meshCollider.convex = true;  // Dùng convex collider
        }
    }


    private void CreateDebrisPieces(List<int> affectedTriangles, Vector3[] sourceVertices, Vector2[] sourceUVs, int[] triangles, Vector3 hitPoint)
    {
        int trianglesPerPiece = Mathf.Max(3, affectedTriangles.Count / maxDebrisPieces);

        for (int i = 0; i < affectedTriangles.Count; i += trianglesPerPiece)
        {
            int count = Mathf.Min(trianglesPerPiece, affectedTriangles.Count - i);
            if (count < 3) continue; // Đảm bảo đủ 3 đỉnh để tạo tam giác

            List<int> pieceTriangles = new List<int>();
            for (int j = 0; j < count; j++)
            {
                int triIndex = affectedTriangles[i + j];
                pieceTriangles.Add(triangles[triIndex]);
                pieceTriangles.Add(triangles[triIndex + 1]);
                pieceTriangles.Add(triangles[triIndex + 2]);
            }

            CreateDebrisPiece(pieceTriangles, sourceVertices, sourceUVs, hitPoint);
        }
    }
   

    
    private void CreateDebrisPiece(List<int> pieceTriangles, Vector3[] sourceVertices, Vector2[] sourceUVs, Vector3 hitPoint)
    {
        if (pieceTriangles.Count < 3) return;

        GameObject debris = new GameObject("Debris");
        debris.transform.position = transform.position;
        debris.transform.rotation = transform.rotation;
        debris.transform.localScale = new Vector3(20,20,20);

        MeshFilter debrisFilter = debris.AddComponent<MeshFilter>();
        MeshRenderer debrisRenderer = debris.AddComponent<MeshRenderer>();
        Rigidbody debrisRb = debris.AddComponent<Rigidbody>();
        MeshCollider debrisCollider = debris.AddComponent<MeshCollider>();

        // Tạo mesh cho mảnh vỡ
        List<Vector3> debrisVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector2> debrisUVs = new List<Vector2>();
        Dictionary<int, int> vertexMapping = new Dictionary<int, int>();

        for (int i = 0; i < pieceTriangles.Count; i++)
        {
            int originalIndex = pieceTriangles[i];
            if (!vertexMapping.ContainsKey(originalIndex))
            {
                vertexMapping[originalIndex] = debrisVertices.Count;
                debrisVertices.Add(sourceVertices[originalIndex]);
                debrisUVs.Add(sourceUVs[originalIndex]);
            }
            newTriangles.Add(vertexMapping[originalIndex]);
        }

        Mesh debrisMesh = new Mesh();
        debrisMesh.vertices = debrisVertices.ToArray();
        debrisMesh.triangles = newTriangles.ToArray();
        debrisMesh.uv = debrisUVs.ToArray();
        debrisMesh.RecalculateNormals();
        debrisMesh.RecalculateBounds();

        debrisFilter.mesh = debrisMesh;
        debrisRenderer.material = meshRenderer.material;

        // Gán collider và thêm lực
        debrisCollider.sharedMesh = debrisMesh;
        debrisCollider.convex = true;

        debrisRb.mass = 0.1f;
        debrisRb.AddExplosionForce(breakForce, hitPoint, breakRadius);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, breakRadius);
    }
}