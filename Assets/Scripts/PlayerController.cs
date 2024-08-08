using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using Unity.VisualScripting;

public class PlayerController : NetworkBehaviour
{
    public enum PlayerType { Runner, Chaser }

    [SyncVar]
    public PlayerType playerType;

    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float jumpForce = 5f;
    [SerializeField]
    private float dashDuration = 1.5f;
    [SerializeField]
    private float dashCooldown = 5f;
    [SerializeField]
    private Color[] colores;
    [SerializeField]
    private GameObject pausa;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundCheckRadius = 0.1f;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private float coyoteTimeDuration = 0.2f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isDashing;
    private float dashEndTime;
    private bool canDoubleJump;
    private float nextDashTime;
    private float lastGroundedTime;
    public bool isPaused = false;
    public Pausa pausita;


#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private SpriteRenderer renderer;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        pausa.SetActive(false);
    }

    public override void OnStartLocalPlayer()
    {
        Camera.main.GetComponent<CameraFollow>().target = transform;
        CmdSetPlayerType(PlayerPrefs.GetString("PlayerType"));
    }

    [Command]
    private void CmdSetPlayerType(string type)
    {
        playerType = (PlayerType)System.Enum.Parse(typeof(PlayerType), type);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        moveInput = context.ReadValue<Vector2>();
        CmdMove(moveInput);
    }

    [Command]
    private void CmdMove(Vector2 moveInput)
    {
        RpcMove(moveInput);
    }

    [ClientRpc]
    private void RpcMove(Vector2 moveInput)
    {
        if (!isLocalPlayer)
        {
            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isPaused) return;

        if (!isLocalPlayer) return;

        if (context.performed)
        {
            if (IsGrounded() || (Time.time - lastGroundedTime <= coyoteTimeDuration))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                CmdJump(jumpForce);
                if (playerType == PlayerType.Chaser)
                {
                    canDoubleJump = true;
                }
            }
            else if (playerType == PlayerType.Chaser && canDoubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                CmdJump(jumpForce);
                canDoubleJump = false;
            }
        }
    }

    [Command]
    private void CmdJump(float jumpForce)
    {
        RpcJump(jumpForce);
    }

    [ClientRpc]
    private void RpcJump(float jumpForce)
    {
        if (!isLocalPlayer)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (isPaused) return;

        if (!isLocalPlayer || playerType != PlayerType.Runner) return;

        if (context.performed && !isDashing && Time.time >= nextDashTime)
        {
            isDashing = true;
            dashEndTime = Time.time + dashDuration;
            nextDashTime = dashEndTime + dashCooldown;
            CmdDash(dashDuration);
            StartCoroutine(Invencible());
            StartCoroutine(ColorChange());
        }
    }

    [Command]
    private void CmdDash(float dashDuration)
    {
        RpcDash(dashDuration);
    }

    [ClientRpc]
    private void RpcDash(float dashDuration)
    {
        if (isLocalPlayer)
        {
            StartCoroutine(Dash(dashDuration));
        }
    }

    private IEnumerator Dash(float duration)
    {
        isDashing = true;
        yield return new WaitForSeconds(duration);
        isDashing = false;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (context.performed)
        {
            if (isPaused)
            {
                isPaused = false;
                pausita.Turnoff();
            }
            else
                isPaused = !isPaused;
            

            if (!isPaused)
                pausa.SetActive(false);
            else
                pausa.SetActive(true);
        }
    }

    public void MenuPause()
    {
        isPaused = !isPaused;

        if (!isPaused)
            pausa.SetActive(false);
        else
            pausa.SetActive(true);
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        

        if (isDashing)
        {
            if (Time.time >= dashEndTime)
            {
                isDashing = false;
            }
        }
        if (IsGrounded())
        {
            lastGroundedTime = Time.time;
        }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        
        if (!isDashing)
            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        else
            rb.velocity = new Vector2(moveInput.x * moveSpeed * 2, rb.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private IEnumerator Invencible()
    {
        this.tag = "Invencible";
        yield return new WaitForSeconds(dashDuration);
        this.tag = "Runner";
    }

    private IEnumerator ColorChange()
    {
        int lastColorIndex = -1;

        for (int i = 0; i < 15; i++)
        {
            int newColorIndex;

            // Genera un nuevo color que sea diferente al último
            do
            {
                newColorIndex = Random.Range(0, colores.Length);
            } while (newColorIndex == lastColorIndex);

            // Asigna el nuevo color y actualiza el el último color
            this.renderer.color = colores[newColorIndex];
            lastColorIndex = newColorIndex;

            yield return new WaitForSeconds(0.1f);
        }

        //esto sera el color white en la version final.
        this.renderer.color = new Color(0.3143499f, 1.0f, 0.0f, 1.0f);
    }

}
