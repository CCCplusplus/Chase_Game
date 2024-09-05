using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalChaser : MonoBehaviour
{
    private HashSet<GameObject> portalObjects = new HashSet<GameObject>();
    private Dictionary<GameObject, bool> teleportCooldowns = new Dictionary<GameObject, bool>();


    [SerializeField] private Transform destination;
    [SerializeField] private AudioSource portalSound;
    [SerializeField] private float cooldownTime = 4.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el objeto está en cooldown, no hacer nada
        if (teleportCooldowns.ContainsKey(collision.gameObject) && teleportCooldowns[collision.gameObject])
        {
            return;
        }

        if (collision.CompareTag("Chaser"))
        {
            portalObjects.Add(collision.gameObject);

            //Reproducir sonido del portal
            if(portalSound != null)
            {
                portalSound.Play();
            }

            // Si el portal de destino tiene un script de PortalChaser, agregar el objeto a su lista y comenzar el cooldown
            if (destination.TryGetComponent(out PortalChaser destinationPortal))
            {
                destinationPortal.portalObjects.Add(collision.gameObject);
                destinationPortal.StartCoroutine(destinationPortal.TeleportCooldown(collision.gameObject));
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

    private IEnumerator TeleportCooldown(GameObject teleportedObject)
    {
        //Iniciar el cooldown
        if (!teleportCooldowns.ContainsKey(teleportedObject))
        {
            teleportCooldowns.Add(teleportedObject, true);
        }
        else
        {
            teleportCooldowns[teleportedObject] = true;
        }

        //Esperar el tiempo de cooldown
        yield return new WaitForSeconds(cooldownTime);

        //Finalizar el cooldown
        teleportCooldowns[teleportedObject] = false;
    }

}
