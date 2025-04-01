using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReboteGizmo : MonoBehaviour
{
    [SerializeField] float gravity = 9.8f;

    [SerializeField] float lineHeight;
    float velocity = 0;


    void Update()
    {
        velocity += gravity * Time.deltaTime;

        Vector3 pos = transform.position;
        pos.y -= velocity * Time.deltaTime;
        transform.position = pos;

        if (transform.position.y <= lineHeight)
        {
            velocity *= -1.05f;
            pos = transform.position;
            pos.y = lineHeight;
            transform.position = pos;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-10, lineHeight, 0), new Vector3(10, lineHeight, 0));
    }
}
