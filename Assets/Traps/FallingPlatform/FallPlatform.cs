using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FallPlatform : NetworkBehaviour
{
    [SerializeField] private float tiempoEspera = 1f;
    [SerializeField] private float tiempoReaparicion = 2f;
    [SerializeField] private int numeroParpadeos = 3;
    [SerializeField] private float duracionParpadeo = 0.2f;

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
            yield return new WaitForSeconds(duracionParpadeo);
            platformRd.enabled = true;
            yield return new WaitForSeconds(duracionParpadeo);
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
