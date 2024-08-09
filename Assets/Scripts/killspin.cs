using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killspin : MonoBehaviour
{
    [SerializeField][Header("Killspin Variables")]
    float rotationSpeed = 100f; // Velocidad de rotación en grados por segundo

    void Update()
    {
        // Rotar el objeto alrededor de su eje Z
        transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Runner") || other.CompareTag("Chaser"))
        {
            //Cosas para matar al jugador
            Debug.Log("Morido");
            other.gameObject.SetActive(false);
        }
    }
}
