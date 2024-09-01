using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;

public class CardItem : MonoBehaviour
{
    private Collider2D selfCollider;
    private SpriteRenderer selfSprite;

    public AudioClip cardSound;
    private AudioSource audioSource;

    public bool hit = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        selfCollider = GetComponent<Collider2D>();
        selfSprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Runner"))
        {
            InvertPlayerControls(collision.gameObject);
            selfCollider.enabled = false;
            selfSprite.enabled = false;
        }
    }

    void InvertPlayerControls(GameObject hitPlayer)
    {
        hit = true;
        StartCoroutine(TurnBack());
    }

    private IEnumerator TurnBack()
    {
        yield return new WaitForSeconds(3f);
        hit = false;
    }
}