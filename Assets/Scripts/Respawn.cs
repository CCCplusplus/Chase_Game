using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Transform runnerTransform;
    public Transform chaserTransform;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
            player.died = true;
        
        if (collision.gameObject.tag == "Runner" || collision.gameObject.tag == "Invencible")
            collision.gameObject.transform.position = runnerTransform.position;
        else if (collision.gameObject.tag == "Chaser")
            collision.gameObject.transform.position = chaserTransform.position;
    }
}