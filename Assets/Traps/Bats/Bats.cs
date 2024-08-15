using System.Collections;
using UnityEngine;

public class Bats : MonoBehaviour
{
    [SerializeField] private Transform initialPosition; // Posición inicial para reaparecer
    [SerializeField] private bool moveLeft = true; // Determina si se mueven hacia la izquierda o derecha
    [SerializeField] private float horizontalSpeed = 2f; // Velocidad horizontal de los círculos
    [SerializeField] private float arcHeight = 2f; // Altura del arco del movimiento
    [SerializeField] private float arcSpeed = 2f; // Velocidad del movimiento en arco
    [SerializeField] private float appearDelay = 1f;
    [SerializeField] private float disappearTime = 4f; // Tiempo antes de desaparecer
    [SerializeField] private float respawnDelay = 0.5f; // Tiempo antes de reaparecer

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    private Vector2 startPosition;
    private float arcTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        startPosition = initialPosition.position;
        rb.isKinematic = true; // Asegurarse de que los círculos no sean empujados por el jugador

        spriteRenderer.enabled = false;
        circleCollider.enabled = false;

        StartCoroutine(ShowTime());
    }

    void Update()
    {
        MoveInArc();
    }

    private void MoveInArc()
    {
        arcTimer += Time.deltaTime * arcSpeed;

        float verticalOffset = Mathf.Sin(arcTimer) * arcHeight; // Movimiento en arco
        float horizontalMovement = moveLeft ? -horizontalSpeed : horizontalSpeed;

        Vector2 newPosition = new Vector2(transform.position.x + horizontalMovement * Time.deltaTime, startPosition.y + verticalOffset);

        rb.MovePosition(newPosition);
    }

    private IEnumerator ShowTime()
    {
        yield return new WaitForSeconds(appearDelay);
        
        transform.position = initialPosition.position;
        arcTimer = 0f;

        spriteRenderer.enabled = true;
        circleCollider.enabled = true;

        StartCoroutine(MovementRoutine());
    }

    private IEnumerator MovementRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(disappearTime);

            
            spriteRenderer.enabled = false;
            circleCollider.enabled = false;

            yield return new WaitForSeconds(respawnDelay);

            // Mover a la posición inicial
            transform.position = initialPosition.position;
            arcTimer = 0f; // Reinicia el movimiento en arco

            // Reactivar el SpriteRenderer y el CircleCollider2D
            spriteRenderer.enabled = true;
            circleCollider.enabled = true;
        }
    }
}
