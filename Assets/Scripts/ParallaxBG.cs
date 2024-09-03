using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBG : MonoBehaviour
{
    [SerializeField]
    private float parallaxMultiplier;

    private Transform cameraTransform;
    private Vector3 previousCameraPos;
    private float spriteWidth, startPos;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        previousCameraPos = cameraTransform.position;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        startPos = transform.position.x;
    }
    void LateUpdate()
    {
        float deltaX = (cameraTransform.position.x - previousCameraPos.x) * parallaxMultiplier;
        float moveAmount = cameraTransform.position.x * (1 - parallaxMultiplier);
        
        transform.Translate(new Vector3 (deltaX, 0, 0));
        previousCameraPos = cameraTransform.position;

        if(moveAmount > startPos + spriteWidth)
        {
          transform.Translate(new Vector3(spriteWidth, 0, 0));
          startPos += spriteWidth;
        }
        else if(moveAmount < startPos - spriteWidth)
        {
          transform.Translate(new Vector3(-spriteWidth, 0, 0));
          startPos -= spriteWidth;
        }
    }
}
