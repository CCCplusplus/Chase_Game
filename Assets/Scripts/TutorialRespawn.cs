using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRespawn : MonoBehaviour
{
    public Transform runnerTransform;
    public Transform chaserTransform;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
            player.died = true;

        if (collision.gameObject.tag == "Runner")
            collision.gameObject.transform.position = runnerTransform.position;
        else if (collision.gameObject.tag == "Chaser")
            collision.gameObject.transform.position = chaserTransform.position;
    }
}
