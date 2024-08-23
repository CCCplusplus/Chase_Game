using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBG : MonoBehaviour
{
    [SerializeField]
    private float parallaxMultiplier;

    private Transform cameraTransform;
    private Vector3 previousCameraPos;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        previousCameraPos = cameraTransform.position;
    }
    void Update()
    {
        float deltaX = (cameraTransform.position.x - previousCameraPos.x) * parallaxMultiplier;
        transform.Translate(new Vector3 (deltaX, 0, 0));
        previousCameraPos = cameraTransform.position;
    }
}
