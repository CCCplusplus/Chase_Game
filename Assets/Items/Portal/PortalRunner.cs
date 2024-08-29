using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalRunner : MonoBehaviour
{
    private HashSet<GameObject> portalObjects = new HashSet<GameObject>();
    private Dictionary<GameObject, bool> teleportCooldowns = new Dictionary<GameObject, bool>();

    [SerializeField] private Transform destination;
    [SerializeField] private AudioSource portalSound;
    [SerializeField] private float cooldownTime = 4.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (teleportCooldowns.ContainsKey(collision.gameObject)&&teleportCooldowns[collision.gameObject])
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
            //Teletransportar el objeto al destino
            collision.transform.position = destination.position;

            //Iniciar el cooldown para este objeto
            StartCoroutine(TeleportCooldown(collision.gameObject));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        portalObjects.Remove(collision.gameObject);
    }

    private IEnumerator TeleportCooldown(GameObject teleportObject)
    {
        //Iniciar el cooldown
        if (!teleportCooldowns.ContainsKey(teleportObject))
        {
            teleportCooldowns.Add(teleportObject, true);
        }
        else
        {
            teleportCooldowns[teleportObject] = true;
        }

        //Esperar el tiempo de cooldown
        yield return new WaitForSeconds(cooldownTime);

        //Finalizar el cooldown
        teleportCooldowns[teleportObject] = false;
    }
}
