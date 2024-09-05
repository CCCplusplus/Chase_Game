using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGeneral : MonoBehaviour
{
    private HashSet<GameObject> portalObjects = new HashSet<GameObject>();
    private Dictionary<GameObject, bool> teleportCooldowns = new Dictionary<GameObject, bool>();

    [SerializeField] private Transform destination; // El destino al que se teletransportan los objetos
    [SerializeField] private AudioSource portalSound; //Referencia al componente de Audio
    [SerializeField] private float cooldownTime = 1.5f; 

    private void OnTriggerEnter2D(Collider2D collision) {
        //Verifica si el objeto tiene la etiqueta "runner" o "chaser"
        if (collision.CompareTag("Runner") || collision.CompareTag("Chaser")) {
            // Si el objeto está en cooldown, no hacer nada
            if (teleportCooldowns.ContainsKey(collision.gameObject) && teleportCooldowns[collision.gameObject]) {
                return;
            }

            //Agregar el objeto a la lista de objetos teletransportados
            portalObjects.Add(collision.gameObject);

            //Si el portal de destino tiene un script de Portal, agregar el objeto su lista
            if (destination.TryGetComponent(out PortalGeneral destinationPortal)) {
                destinationPortal.StartCoroutine(destinationPortal.TeleportCooldown(collision.gameObject));
            }

            //Teletransportar el objeto al destino
            collision.transform.position = destination.position;

            //Reproducir sonido del portal
            if(portalSound != null) {
                portalSound.Play();
            }

            //Iniciar el cooldown para este objeto
            StartCoroutine(TeleportCooldown(collision.gameObject));

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Cuando el objeto sale del portal, eliminarlo de la lista
        portalObjects.Remove(collision.gameObject);
    }

    private IEnumerator TeleportCooldown(GameObject teleportedObject)
    {
        //Iniciar el cooldon
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
