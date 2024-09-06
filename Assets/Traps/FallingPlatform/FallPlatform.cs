using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FallPlatform : NetworkBehaviour
{
    [SerializeField] private float tiempoEspera = 5f;
    [SerializeField] private float tiempoReaparicion = 2f;
    [SerializeField] private int numeroParpadeos = 10;
    [SerializeField] private float duracionParpadeoInicial = 5.0f;
    [SerializeField] private float duracionParpadeoFinal = 2.0f;

    private Collider2D col2D;
    private Renderer platformRd;

    private void Start()
    {
        col2D = GetComponent<Collider2D>();
        platformRd = GetComponent<Renderer>();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Runner") || other.gameObject.CompareTag("Chaser"))
        {
            if (NetworkClient.active && isServer)
            {
                RpcDesapareceYReaparece();
            }
        }
    }

    [ClientRpc]
    private void RpcDesapareceYReaparece()
    {
        StartCoroutine(DesapareceYReaparece());
    }

    private IEnumerator DesapareceYReaparece()
    {

        // Parpadeo de la plataforma
        for (int i = 0; i < numeroParpadeos; i++)
        {
            platformRd.enabled = false;
            //Usamos SmoothStep para suavizar la transicion del parpadeo
            float t = (float)i / (numeroParpadeos - 1);
            //Calcular la duracion del parpadeo en funcion de la iteracion
            float duracionActualParpadeo = Mathf.Lerp(duracionParpadeoInicial, duracionParpadeoFinal, t);

            yield return new WaitForSeconds(duracionActualParpadeo);
            platformRd.enabled = true;
            yield return new WaitForSeconds(duracionActualParpadeo);
        }

        // Espera antes de que la plataforma desaparezca
        yield return new WaitForSeconds(tiempoEspera);

        // Desactiva la plataforma (desaparece)
        col2D.enabled = false;
        platformRd.enabled = false;

        // Espera un tiempo antes de reaparecer la plataforma
        yield return new WaitForSeconds(tiempoReaparicion);

        // Reactiva la plataforma (reaparece)
        col2D.enabled = true;
        platformRd.enabled = true;
    }
}
