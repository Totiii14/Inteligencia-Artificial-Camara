using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [SerializeField] private LineOfSight lineOfSight;
    [SerializeField] private int segments = 50;
    [SerializeField] private Color visionColor = new Color(1f, 0f, 0f, 0.05f); // Rojo transparente
    [SerializeField] private Material visionConeMaterial;

    private Mesh mesh;
    private MeshFilter meshFilter;

    private void Awake()
    {
        if (lineOfSight == null)
            lineOfSight = GetComponent<LineOfSight>();

        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.name = "Vision Cone Mesh";
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = visionConeMaterial;
    }

    private void LateUpdate()
    {
        DrawVisionArea();
    }

    private void DrawVisionArea()
    {
        Vector3 origin = lineOfSight.SecurityCamera.position;
        Vector3 forward = Quaternion.Euler(lineOfSight.VerticalAngleOffset, 0f, 0f) * lineOfSight.SecurityCamera.forward;
        float angle = lineOfSight.DetectionAngle;
        float radius = lineOfSight.DetectionRange;
        float height = 2f;

        int vertCount = (segments + 1) * 2 + 2;
        Vector3[] vertices = new Vector3[vertCount];
        int[] triangles = new int[segments * 12];

        Vector3 baseCenter = origin;
        Vector3 topCenter = origin + Vector3.down * height;

        vertices[0] = transform.InverseTransformPoint(baseCenter);
        vertices[1] = transform.InverseTransformPoint(topCenter);

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -angle / 2f + (angle * i / segments);
            Quaternion rotation = Quaternion.Euler(0f, currentAngle, 0f);
            Vector3 direction = rotation * forward;
            Vector3 basePoint = baseCenter + direction.normalized * radius;
            Vector3 topPoint = basePoint + Vector3.down * height;

            vertices[2 + i] = transform.InverseTransformPoint(basePoint);
            vertices[2 + segments + 1 + i] = transform.InverseTransformPoint(topPoint);
        }

        int triIndex = 0;

        for (int i = 0; i < segments; i++)
        {
            triangles[triIndex++] = 0;
            triangles[triIndex++] = 2 + i;
            triangles[triIndex++] = 2 + i + 1;
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[triIndex++] = 1;
            triangles[triIndex++] = 2 + segments + 1 + i + 1;
            triangles[triIndex++] = 2 + segments + 1 + i;
        }

        for (int i = 0; i < segments; i++)
        {
            int baseA = 2 + i;
            int baseB = 2 + i + 1;
            int topA = 2 + segments + 1 + i;
            int topB = 2 + segments + 1 + i + 1;

            triangles[triIndex++] = baseA;
            triangles[triIndex++] = baseB;
            triangles[triIndex++] = topA;

            triangles[triIndex++] = topA;
            triangles[triIndex++] = baseB;
            triangles[triIndex++] = topB;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        transform.position = origin;
        transform.rotation = lineOfSight.SecurityCamera.rotation;
    }
}
