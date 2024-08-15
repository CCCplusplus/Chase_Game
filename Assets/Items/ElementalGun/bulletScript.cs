using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    private Collider2D selfCollider;
    private SpriteRenderer selfSprite;
    public bool hit = false;

    private void Awake()
    {
        selfCollider = GetComponent<Collider2D>();
        selfSprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Chaser") || collision.CompareTag("Runner"))
        {
            selfCollider.enabled = false;
            selfSprite.enabled = false;
            hit = true;
        }else
        {
            hit = false;
        }
    }
}
