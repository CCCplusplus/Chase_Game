using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disappearingPlatform2 : MonoBehaviour
{
    [Header("Platform Variables")]
    [SerializeField] float disappearTime = 2f; //invisible
    [SerializeField] float reappearTime = 2f;  //visible

    private Renderer platformRenderer;
    private Collider2D platformCollider;

    void Start()
    {
        platformRenderer = GetComponent<Renderer>();
        platformCollider = GetComponent<Collider2D>();

        StartCoroutine(TogglePlatform());
    }

    IEnumerator TogglePlatform()
    {
        while (true)
        {
            platformRenderer.enabled = false;
            platformCollider.enabled = false;

            yield return new WaitForSeconds(disappearTime);

            platformRenderer.enabled = true;
            platformCollider.enabled = true;

            yield return new WaitForSeconds(reappearTime);
        }
    }
}
