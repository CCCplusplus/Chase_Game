using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;

public class ShotgunItem : MonoBehaviour
{
    [SerializeField] private AudioClip shootSound; // Sonido del disparo

    private bool isPickedUp = false;
    private bool shoot = false;
    private GameObject player;
    private GameObject hitboxGameObject; // Hitbox para el disparo
    private BoxCollider2D hitbox;
    private ParticleSystem muzzleFlash; // Efecto de partículas 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Runner"))
        {
            isPickedUp = true;
            player = other.gameObject;

            // Buscar el GameObject con el tag "Hitbox" dentro del jugador
            hitboxGameObject = FindHitbox(player);

            hitbox = hitboxGameObject.GetComponent<BoxCollider2D>();

            muzzleFlash = hitboxGameObject.GetComponent<ParticleSystem>();

            if (hitbox == null)
                Debug.LogError("No se encontró un GameObject con el tag 'Hitbox' en los hijos del jugador.");
            

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
                Fire();
            
        }

        if (hitbox != null)
        {
            if (shoot)
                hitbox.enabled = true;
            else
                hitbox.enabled = false;
        }

    }

    private void Fire()
    {
        // Reproducir el sonido del disparo
        if (shootSound != null)
            AudioSource.PlayClipAtPoint(shootSound, player.transform.position);
        

        // Activar el efecto de partículas
        if (muzzleFlash != null)
            muzzleFlash.Play();

        isPickedUp = false;


        // Activar la hitbox temporalmente para detectar colisiones
        shoot = true;
        StartCoroutine(ActivateHitbox());
    }

    private IEnumerator ActivateHitbox()
    {

        yield return new WaitForSeconds(0.3f); // Duración del hitbox

        shoot = false;
    }

    private GameObject FindHitbox(GameObject player)
    {
        // Busca el GameObject con el tag "Hitbox" solo entre los hijos del jugador
        foreach (Transform child in player.transform)
        {
            if (child.CompareTag("Hitbox"))
                return child.gameObject;
            
        }

        return null;
    }
}
