using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform runnerTransform;
    public Transform chaserTransform;
    private Animator animator;

    private Transform own;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        own = this.transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Runner" ^ collision.gameObject.tag == "Invencible")
        {
            Debug.Log("passed");
            runnerTransform.position = own.position;
            runnerTransform = own;
            animator.SetTrigger("Activar");
        }
        else if (collision.gameObject.tag == "Chaser")
        {
            chaserTransform.position = own.position;
            chaserTransform = own;
            animator.SetTrigger("Activar");
        }

    }
}
