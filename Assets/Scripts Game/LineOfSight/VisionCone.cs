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

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = transform.InverseTransformPoint(origin);

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -angle / 2f + (angle * i / segments);
            Quaternion rotation = Quaternion.Euler(0f, currentAngle, 0f);
            Vector3 direction = rotation * forward;
            Vector3 point = origin + direction.normalized * radius;

            vertices[i + 1] = transform.InverseTransformPoint(point);
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3 + 0] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        transform.position = origin;
        transform.rotation = lineOfSight.SecurityCamera.rotation;
    }
}
