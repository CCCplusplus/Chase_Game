using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.VisualScripting;

public class CardItem : MonoBehaviour
{
    [SerializeField]
    public float shootForce = 40f;
    public Transform holdPosition;
    private GameObject player;
    private bool isHeld = false;
    private Transform playerTransform;

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

        if(collision.CompareTag("Runner"))
        {
            InvertPlayerControls(collision.gameObject);
            Destroy(gameObject);
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
        isHeld = false;
        transform.SetParent(null);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.AddForce(holdPosition.right * shootForce, ForceMode2D.Impulse);
    }

    void InvertPlayerControls(GameObject hitPlayer)
    {
        var playerInput = hitPlayer.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            // Invertir los controles usando input mappings
            InputActionMap actionMap = playerInput.actions.FindActionMap("Player");

            // Invertir las entradas de "Move"
            var moveAction = actionMap.FindAction("Move");
            if (moveAction != null)
            {
                moveAction.ApplyBindingOverride("<Gamepad>/leftStick" /*new InputBinding { processors = "invertVector2(invertX=true)" }*/);
            }
        }
    }
}
