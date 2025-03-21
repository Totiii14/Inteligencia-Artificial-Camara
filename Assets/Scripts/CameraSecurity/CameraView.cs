using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    public GameObject alertImage;
    private CameraModel model;
    private void Awake()
    {
        model = GetComponent<CameraModel>();
    }

    private void Update()
    {
        alertImage.SetActive(model.IsDetecting);
    }
}
