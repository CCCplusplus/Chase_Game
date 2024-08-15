using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class FallPlatform : MonoBehaviour
{
    [SerializeField] private float tiempoEspera = 1f;
    [SerializeField] private float tiempoReaparicion = 2f;
    private Collider2D col2D;
    private Renderer renderer;

    private void Start()
    {
        col2D = GetComponent<Collider2D>();
        renderer = GetComponent<Renderer>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Runner"))
        {
            StartCoroutine(Desaparece());
        }
    }

    private IEnumerator Desaparece()
    {
        yield return new WaitForSeconds(tiempoEspera);

        // Desactiva la plataforma (desaparece)
        col2D.enabled = false;
        renderer.enabled = false;

        // Espera un tiempo antes de reaparecer la plataforma
        yield return new WaitForSeconds(tiempoReaparicion);

        // Reactiva la plataforma (reaparece)
        col2D.enabled = true;
        renderer.enabled = true;
    }
}
