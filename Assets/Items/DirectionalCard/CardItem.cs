using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;

public class CardItem : MonoBehaviour
{
    [SerializeField]
    public float shootForce = 40f;
    public Transform holdPosition;
    private GameObject player;
    private bool isHeld = false;
    private Transform playerTransform;

    private Collider2D selfCollider;
    private SpriteRenderer selfSprite;

    public AudioClip cardSound;
    private AudioSource audioSource;

    public bool hit = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        selfCollider = GetComponent<Collider2D>();
        selfSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isHeld && player != null)
        {
            var inputActions = player.GetComponent<PlayerInput>().actions;
            if (inputActions["Use-Item"].triggered)
                ShootItem();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Chaser") && !isHeld)
        {
            PickUpItem(collision.gameObject);
        }

        if (collision.CompareTag("Runner"))
        {
            InvertPlayerControls(collision.gameObject);
            selfCollider.enabled = false;
            selfSprite.enabled = false;
        }
    }

    void PickUpItem(GameObject playerObject)
    {
        isHeld = true;
        player = playerObject;
        playerTransform = player.transform;
        transform.SetParent(playerTransform);

        Transform holdPos = playerTransform.Find("HoldPosition");
        if (holdPos != null)
        {
            holdPosition = holdPos;
            transform.position = holdPosition.position;
        }

        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    void ShootItem()
    {
        audioSource.PlayOneShot(cardSound);
        isHeld = false;
        transform.SetParent(null);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.AddForce(holdPosition.right * shootForce, ForceMode2D.Impulse);
    }

    void InvertPlayerControls(GameObject hitPlayer)
    {
        hit = true;
        StartCoroutine(TurnBack());
    }

    private IEnumerator TurnBack()
    {
        yield return new WaitForSeconds(3f);
        hit = false;
    }
}