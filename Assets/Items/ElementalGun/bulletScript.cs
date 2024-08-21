using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    private Collider2D selfCollider;
    private SpriteRenderer selfSprite;
    public GunScript ammotype;
    public bool hit = false;

    private void Awake()
    {
        selfCollider = GetComponent<Collider2D>();
        selfSprite = GetComponent<SpriteRenderer>();
        ammotype = GameObject.FindGameObjectWithTag("ElementalGun").GetComponent<GunScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Chaser") || collision.CompareTag("Runner"))
        {
            hit = true;
            StartCoroutine(TurnBack());
            selfCollider.enabled = false;
            selfSprite.enabled = false;
        }
    }

    private IEnumerator TurnBack()
    {
        yield return new WaitForSeconds(3.5f);
        hit = false; ;
    }
}
