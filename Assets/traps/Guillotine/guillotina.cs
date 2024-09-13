using System.Collections;
using UnityEngine;

public class guillotina : MonoBehaviour
{
    [Header("Guillotine Variables")]
    [SerializeField] float dropSpeed = 10f; // Caida
    [SerializeField] float riseSpeed = 2f;  // subida
    [SerializeField] float waitTime = 2f;   // tiempo que espera en el suelo
    [SerializeField] float cycleTime = 5f;  // tiempo de espera
    //[SerializeField] float warningTime = 1f;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isDropping = true;

    private Color warningColor = Color.red;
    private Color normalColor = Color.white;
    private SpriteRenderer renderer;

    public Transform runnerTransform;
    public Transform chaserTransform;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        initialPosition = transform.position;
        targetPosition = new Vector3(transform.position.x, transform.position.y - 4f, transform.position.z);
        StartCoroutine(GuillotineCycle());
    }

    IEnumerator GuillotineCycle()
    {
        while (true)
        {
            //warning
            renderer.color = warningColor;
            //yield return new WaitForSeconds(warningTime);

            //caida
            while (isDropping && transform.position.y > targetPosition.y)
            {
                renderer.color = normalColor;
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
