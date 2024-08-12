using UnityEngine;

public class ShotgunHitbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Chaser"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Calcula la dirección del knockback
                Vector2 knockbackDirection = other.transform.position - transform.position;
                knockbackDirection.Normalize();

                // Aplica la fuerza de retroceso
                rb.AddForce(knockbackDirection * 10f, ForceMode2D.Impulse);
            }
        }
    }
}
