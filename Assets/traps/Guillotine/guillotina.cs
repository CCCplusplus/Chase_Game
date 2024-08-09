using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class guillotina : MonoBehaviour
{
    [Header("Guillotine Variables")]
    [SerializeField] float dropSpeed = 10f; // Caida
    [SerializeField] float riseSpeed = 2f;  // subida
    [SerializeField] float waitTime = 2f;   // tiempo que espera en el suelo
    [SerializeField] float cycleTime = 5f;  // tiempo de espera

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isDropping = true;

    void Start()
    { 
        initialPosition = transform.position;
        targetPosition = new Vector3(transform.position.x, transform.position.y - 4f, transform.position.z);
        StartCoroutine(GuillotineCycle());
    }

    IEnumerator GuillotineCycle()
    {
        while (true)
        {
            //caida
            while (isDropping && transform.position.y > targetPosition.y)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, dropSpeed * Time.deltaTime);
                yield return null;
            }

            //espera
            yield return new WaitForSeconds(waitTime);

            //subida
            isDropping = false;
            while (!isDropping && transform.position.y < initialPosition.y)
            {
                transform.position = Vector3.MoveTowards(transform.position, initialPosition, riseSpeed * Time.deltaTime);
                yield return null;
            }

            //espera el tiempo restante
            yield return new WaitForSeconds(cycleTime - waitTime);

            isDropping = true;
        }
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
