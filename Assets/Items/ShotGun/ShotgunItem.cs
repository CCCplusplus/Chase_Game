using UnityEngine;

public class ShotgunItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Runner"))
        {
            // Get the ShotgunHitbox component from the player
            ShotgunHitbox shotgunHitbox = other.GetComponentInChildren<ShotgunHitbox>();

            if (shotgunHitbox != null)
            {
                // Enable the shotgun on the player
                shotgunHitbox.hasShotgun = true;
            }

            // Disable the shotgun item in the scene after it's picked up
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
