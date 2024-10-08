using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCannon : MonoBehaviour
{
    public Transform exitPoint;

    private bool playerInside = false;
    private GameObject player;

    [SerializeField]
    public float launchForce = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Runner") ^ other.CompareTag("Chaser"))
        {
            player = other.gameObject;
            PlayerController playerController = player.GetComponent<PlayerController>();
            //playerController.interactwCanon = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Runner") ^ other.CompareTag("Chaser"))
        {
            player = other.gameObject;
            PlayerController playerController = player.GetComponent<PlayerController>();
            //playerController.interactwCanon = false;
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if(playerInside && context.performed)
        {
            FirePlayer();
        }
    }

    void FirePlayer()
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        player.transform.position = exitPoint.position;
        Vector2 launchDir = exitPoint.position;
        //launchForce = launchDir.magnitude;
        rb.AddForce(launchDir * launchForce, ForceMode2D.Impulse);
        playerInside = false;
    }
}
