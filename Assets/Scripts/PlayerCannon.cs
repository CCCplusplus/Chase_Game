using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCannon : MonoBehaviour
{
    public Transform exitPoint;

    public bool playerInside = false;
    private GameObject player;

    [SerializeField]
    public float launchForce = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            player = other.gameObject;
            playerInside = true;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    void Update()
    {
        if(playerInside && Input.GetKeyDown(KeyCode.E))
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
