using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FallPlatform : NetworkBehaviour
{
    [SerializeField] private float tiempoEspera = 0.1f;
    [SerializeField] private float tiempoReaparicion = 2f;
    [SerializeField] private GameObject efectoEscombros;
    //[SerializeField] private int numeroParpadeos = 10;
    //[SerializeField] private float duracionParpadeo = 0.5f;

    private Collider2D col2D;
    //private Renderer platformRd;
    private SpriteRenderer spriteRD;

    private bool esDesaparecer = false;

    private void Start()
    {
        col2D = GetComponent<Collider2D>();
        //platformRd = GetComponent<Renderer>();
        spriteRD = GetComponent<SpriteRenderer>();

        // Asegúrate de que el sistema de partículas esté desactivado al inicio
        if (efectoEscombros != null)
        {
            efectoEscombros.SetActive(false);
        }
    }

    private void Update()
    {
        //Iniciar el parpadeo si se activa la señal de desaparecer
        if (esDesaparecer)
        {
            StartCoroutine(DesapareceYReaparece());
            esDesaparecer = false;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Runner") || other.gameObject.CompareTag("Chaser"))
        {
            if (NetworkClient.active && isServer)
            {
                esDesaparecer = true;
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

        // Activa el efecto de escombros
        if (efectoEscombros != null)
        {
            efectoEscombros.SetActive(true);
        }

        // Espera antes de que la plataforma desaparezca
        yield return new WaitForSeconds(tiempoEspera);

        // Desactiva la plataforma (desaparece)
        col2D.enabled = false;
        spriteRD.enabled = false;

        // Desactiva el efecto de escombros después de un breve tiempo
        if (efectoEscombros != null)
        {
            efectoEscombros.SetActive(false);
        }

        // Espera un tiempo antes de reaparecer la plataforma
        yield return new WaitForSeconds(tiempoReaparicion);

        // Reactiva la plataforma (reaparece)
        col2D.enabled = true;
        spriteRD.enabled = true;
    }
}
