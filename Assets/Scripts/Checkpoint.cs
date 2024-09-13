using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform runnerTransform;
    public Transform chaserTransform;

    private Transform own;

    private void Awake()
    {
        own = this.transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Runner")
        {
            Debug.Log("passed");
            runnerTransform.position = own.position;
            runnerTransform = own;
        }
        else if (collision.gameObject.tag == "Chaser")
        {
            chaserTransform.position = own.position;
            chaserTransform = own;
        }
            
    }
}
