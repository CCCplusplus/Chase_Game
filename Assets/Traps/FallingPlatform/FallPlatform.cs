using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class FallPlatform : MonoBehaviour
{
    [SerializeField] private float tiempoEspera = 1f;
    [SerializeField] private float tiempoReaparicion = 2f;
    private Collider2D col2D;
    private Renderer platformRd;
    //private bool runnerEncima = false;

    private void Start()
    {
        col2D = GetComponent<Collider2D>();
        platformRd = GetComponent<Renderer>();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Runner") || other.gameObject.CompareTag("Chaser"))
        {
            StartCoroutine(DesapareceYReaparece());
        }
    }

    private IEnumerator DesapareceYReaparece()
    {
        //Espera antes de que la plataforma desaparezca
        yield return new WaitForSeconds(tiempoEspera);
        //Desactiva la plataforma
        col2D.enabled = false;
        platformRd.enabled = false;
        //Espera un tiempo antes de reaparecer la plataforma
        yield return new WaitForSeconds(tiempoReaparicion);
        //Reactiva la plataforma
        col2D.enabled = true;
        platformRd.enabled = true;
    }
}
