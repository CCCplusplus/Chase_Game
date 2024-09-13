using UnityEngine;
using System.Collections;

public class FireTrap : MonoBehaviour
{
    [SerializeField] private float damage;

    [Header("Firetrap Timers")]
    [SerializeField] private float activationDelay;
    [SerializeField] private float activeTime;
    private Animator anim;
    private SpriteRenderer spriteRend;

    private bool triggered; //when the trap gets triggered
    private bool active; //when the trap is active and can hurt the player

    public Transform runnerTransform;
    public Transform chaserTransform;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Chaser" || collision.tag == "Runner")
        {
            if (!triggered)
                StartCoroutine(ActivateFiretrap());

            if (active)
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                player.died = true;
                if (collision.gameObject.tag == "Runner" || collision.gameObject.tag == "Invencible")
                    collision.gameObject.transform.position = runnerTransform.position;
                else if (collision.gameObject.tag == "Chaser")
                    collision.gameObject.transform.position = chaserTransform.position;
            }

        }
    }
    private IEnumerator ActivateFiretrap()
    {
        //turn the sprite red to notify the player and trigger the trap
        triggered = true;
        spriteRend.color = Color.red;

        //Wait for delay, activate trap, turn on animation, return color back to normal
        yield return new WaitForSeconds(activationDelay);
        spriteRend.color = Color.white; //turn the sprite back to its initial color
        active = true;
        anim.SetBool("Activate", true);

        //Wait until X seconds, deactivate trap and reset all variables and animator
        yield return new WaitForSeconds(activeTime);
        active = false;
        triggered = false;
        anim.SetBool("Activate", false);
    }
}
