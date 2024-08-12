using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;

public class ShotgunItem : MonoBehaviour
{
    [SerializeField] private AudioClip shootSound; // Sonido del disparo
    [SerializeField] private ParticleSystem muzzleFlash; // Efecto de partículas 
    [SerializeField] private float knockbackForce = 10f; // Fuerza del retroceso

    private bool isPickedUp = false;
    private GameObject player;
    private GameObject hitbox; // Hitbox para el disparo

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Runner"))
        {
            isPickedUp = true;
            player = other.gameObject;

            // Buscar el GameObject con el tag "Hitbox" dentro del jugador
            hitbox = FindHitbox(player);

            if (hitbox == null)
            {
                Debug.LogError("No se encontró un GameObject con el tag 'Hitbox' en los hijos del jugador.");
            }

            // Desactivar el objeto o hacerlo invisible al recogerlo
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void Update()
    {
        if (isPickedUp && player != null && hitbox != null)
        {
            var inputActions = player.GetComponent<PlayerInput>().actions; // Obtener el input action map
            if (inputActions["Use-Item"].triggered) // Verificar si se presionó el botón de Use-Item
            {
                Fire();
            }
        }
    }

    private void Fire()
    {
        // Reproducir el sonido del disparo
        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(shootSound, player.transform.position);
        }

        // Activar el efecto de partículas
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Activar la hitbox temporalmente para detectar colisiones
        StartCoroutine(ActivateHitbox());
    }

    private IEnumerator ActivateHitbox()
    {
        hitbox.SetActive(true);

        yield return new WaitForSeconds(0.2f); // Duración del hitbox

        hitbox.SetActive(false);
    }

    private GameObject FindHitbox(GameObject player)
    {
        // Busca el GameObject con el tag "Hitbox" solo entre los hijos del jugador
        foreach (Transform child in player.transform)
        {
            if (child.CompareTag("Hitbox"))
            {
                return child.gameObject;
            }
        }

        return null;
    }
}
