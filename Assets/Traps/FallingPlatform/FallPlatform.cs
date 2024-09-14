using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class FallPlatform : MonoBehaviour
{
    [SerializeField] private float tiempoEspera = 1f;
    [SerializeField] private float tiempoReaparicion = 2f;
    private Collider2D col2D;
    private Renderer platformRd;
    private ParticleSystem particle;
    private bool touched = false;

    private void Start()
    {
        col2D = GetComponent<Collider2D>();
        platformRd = GetComponent<Renderer>();
        particle = GetComponentInChildren<ParticleSystem>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Runner") || other.gameObject.CompareTag("Chaser"))
        {
            if (!touched)
            {
                particle.Play();
                touched = true;
                StartCoroutine(Desaparece());
            }
        }
    }

    private IEnumerator Desaparece()
    {
        yield return new WaitForSeconds(tiempoEspera);

        particle.Stop();
        // Desactiva la plataforma (desaparece)
        col2D.enabled = false;
        platformRd.enabled = false;
        
        // Espera un tiempo antes de reaparecer la plataforma
        yield return new WaitForSeconds(tiempoReaparicion);
        
        // Reactiva la plataforma (reaparece)
        col2D.enabled = true;
        platformRd.enabled = true;
        touched = false;

    }
}
