using System.Net.Sockets;
using UnityEngine;

public class ShotgunItem : MonoBehaviour
{
    public bool shotgunPicked = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Runner"))
        {
            shotgunPicked = true;
            // Get the ShotgunHitbox component from the player
            ShotgunHitbox shotgunHitbox = other.GetComponentInChildren<ShotgunHitbox>();

            PlayerController activate = other.GetComponentInChildren<PlayerController>();

            activate.shotgun = this;

            shotgunHitbox.shotgun = this;

            // Enable the shotgun on the player
            if (shotgunHitbox != null)
                shotgunHitbox.hasShotgun = true;

            // Disable the shotgun item in the scene after it's picked up
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
