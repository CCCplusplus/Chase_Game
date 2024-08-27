using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGeneral : MonoBehaviour
{
    private HashSet<GameObject> portalObjects = new HashSet<GameObject>();

    [SerializeField] private Transform destination; // El destino al que se teletransportan los objetos
    [SerializeField] private AudioSource portalSound; //Referencia al componente de Audio

    private void OnTriggerEnter2D(Collider2D collision) {
        //Verifica si el objeto tiene la etiqueta "runner" o "chaser"
        if (collision.CompareTag("Runner") || collision.CompareTag("Chaser")) {
            //Si el objeto ya ha sido teletransportado, no hacer nada
            if (portalObjects.Contains(collision.gameObject)) {
                return;
            }

            //Agregar el objeto a la lista de objetos teletransportados
            portalObjects.Add(collision.gameObject);

            //Si el portal de destino tiene un script de Portal, agregar el objeto su lista
            if(destination.TryGetComponent(out PortalGeneral destinationPortal)) {
                destinationPortal.portalObjects.Add(collision.gameObject);
            }

            //Teletransportar el objeto al destino
            collision.transform.position = destination.position;

            //Reproducir sonido del portal
            if(portalSound != null) {
                portalSound.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Cuando el objeto sale del portal, eliminarlo de la lista
        portalObjects.Remove(collision.gameObject);
    }
}
