using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Chaser" || collision.tag == "Runner")
        {
            Debug.Log("Morido");
            collision.gameObject.SetActive(false);
        }
    }
}
