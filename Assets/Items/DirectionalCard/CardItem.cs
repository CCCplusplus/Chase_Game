using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;

public class CardItem : MonoBehaviour
{
    [SerializeField] private AudioClip cardShootSound;

    private bool isPickedUp = false;
    private GameObject player;
    private Transform itemPosition;
    private GameObject hitboxGameObject;
    private GameObject projectilePrefab;
    private ParticleSystem hitDustParticles;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Runner"))
        {
            isPickedUp = true;
            player = other.gameObject;

            hitDustParticles = hitboxGameObject.GetComponent<ParticleSystem>();

            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void Update()
    {
        if (isPickedUp && player != null)
        {
            var inputActions = player.GetComponent<PlayerInput>().actions;
            if (inputActions["Use-Item"].triggered)
                Fire();
        }
    }

    private void Fire()
    {
        if (cardShootSound != null)
            AudioSource.PlayClipAtPoint(cardShootSound, player.transform.position);

        if (hitDustParticles != null)
            hitDustParticles.Play();

        isPickedUp = false;


    }
}
