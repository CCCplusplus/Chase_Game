using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool triggered = false;

    public Transform runnerTransform;
    public Transform chaserTransform;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.gameObject.tag == "Runner")
        {
            runnerTransform = this.transform;
            triggered = true;
        }
        else if (collision.gameObject.tag == "Chaser")
        {
            chaserTransform = this.transform;
            triggered = true;
        }
            
    }
}
