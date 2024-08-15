using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalRunner : MonoBehaviour
{
    private HashSet<GameObject> portalObjects = new HashSet<GameObject>();

    [SerializeField] private Transform destination;
    [SerializeField] private AudioSource portalSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (portalObjects.Contains(collision.gameObject))
        {
            return;
        }

        if (collision.CompareTag("Runner"))
        {
            portalObjects.Add(collision.gameObject);

            //Reproducir sonido del portal
            if(portalSound != null)
            {
                portalSound.Play();
            }

            if (destination.TryGetComponent(out PortalRunner destinationPortal))
            {
                destinationPortal.portalObjects.Add(collision.gameObject);
            }
            collision.transform.position = destination.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        portalObjects.Remove(collision.gameObject);
    }
}
