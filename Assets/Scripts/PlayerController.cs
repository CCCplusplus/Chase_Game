using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public enum PlayerType { Runner, Chaser }

    [SyncVar]
    public PlayerType playerType;

    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float minJumpHeight = 4f;
    [SerializeField]
    private float maxJumpHeight = 8f;
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
    private Transform ceilingCheck; // Para detección de colisiones con el techo
    [SerializeField]
    private float groundCheckRadius = 0.1f;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private float coyoteTimeDuration = 0.2f;
    [SerializeField]
    private float maxJumpTime = 0.15f;
    [SerializeField]
    private float gravityMultiplierAscend = 2f;
    [SerializeField]
    private float gravityMultiplierDescend = 3.5f;
    [SerializeField]
    private float gravityMultiplierFall = 3.5f;
    [SerializeField]
    private float jumpForceMultiplier = 3f;
    [SerializeField]
    private GameObject carditemG;
    [SerializeField]
    private CardItem carditem;
    //[SerializeField]
    //private GameObject bulletHitG;
    //[SerializeField] 
    //private bulletScript bulletHit;

    [SerializeField]
    private AudioClip runnerJumpSound;
    [SerializeField]
    private AudioClip chaserJumpSound;
    [SerializeField]
    private AudioClip chaserDoubleJumpSound;
    [SerializeField]
    private float proximityThreshold = 5f; // Umbral de proximidad para oír sonidos de otros jugadores

    private AudioSource audioSource;


    [SerializeField]
    private PhysicsMaterial2D materialWithFriction;  // Material con fricción
    [SerializeField]
    private PhysicsMaterial2D materialNoFriction;    // Material sin fricción

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
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

    private float jumpStartTime;
    private bool isJumping;
    private bool jumpButtonHeld;
    private bool isFalling;
    private bool isFallingFree; // Para detectar caída libre
    private float initialYPosition;
    private float currentHeight;
    private float originalGravityScale;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();  // Obtener el BoxCollider2D
        audioSource = GetComponent<AudioSource>();
        originalGravityScale = rb.gravityScale;
        pausa.SetActive(false);
        //bulletHitG = GameObject.FindGameObjectWithTag("Bullet");
        //bulletHit = bulletHitG.GetComponent<bulletScript>();
    }

    private void Start()
    {
        carditemG = GameObject.FindGameObjectWithTag("Card");
        carditem = carditemG.GetComponent<CardItem>();
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

        if (context.started)
        {
            if (IsGrounded() || (Time.time - lastGroundedTime <= coyoteTimeDuration))
            {
                isJumping = true;
                jumpButtonHeld = true;
                isFalling = false;
                isFallingFree = false;
                rb.gravityScale = gravityMultiplierAscend;
                jumpStartTime = Time.time;
                initialYPosition = transform.position.y;
                PlayJumpSound();
                ApplyJumpForce(minJumpHeight * jumpForceMultiplier);
                CmdJump(rb.velocity.y);
                if (playerType == PlayerType.Chaser)
                {
                    canDoubleJump = true;
                }
            }
            else if (playerType == PlayerType.Chaser && canDoubleJump)
            {
                isJumping = true;
                jumpButtonHeld = true;
                isFalling = false;
                isFallingFree = false;
                rb.gravityScale = gravityMultiplierAscend;
                jumpStartTime = Time.time;
                initialYPosition = transform.position.y;
                PlayDoubleJumpSound();
                ApplyJumpForce(minJumpHeight * jumpForceMultiplier);
                CmdJump(rb.velocity.y);
                canDoubleJump = false;
            }
        }
        else if (context.canceled && isJumping)
        {
            isJumping = false;
            jumpButtonHeld = false;
            StartFalling();
        }
    }

    private void ApplyJumpForce(float height)
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Sqrt(2 * height * Mathf.Abs(Physics2D.gravity.y)));
    }

    private void StartFalling()
    {
        isFalling = true;
        isJumping = false;
        isFallingFree = false;
        rb.gravityScale = gravityMultiplierDescend;
    }

    [Command]
    private void CmdJump(float currentJumpVelocity)
    {
        RpcJump(currentJumpVelocity);
    }

    [ClientRpc]
    private void RpcJump(float currentJumpVelocity)
    {
        if (!isLocalPlayer)
        {
            rb.velocity = new Vector2(rb.velocity.x, currentJumpVelocity);
            if (IsPlayerNearby())
                PlayJumpSound(); 
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
            isPaused = !isPaused;

            if (isPaused)
                pausa.SetActive(true);
            else
                pausa.SetActive(false);
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

    private void PlayJumpSound()
    {
        if (playerType == PlayerType.Runner)
        {
            audioSource.PlayOneShot(runnerJumpSound);
        }
        else if (playerType == PlayerType.Chaser)
        {
            audioSource.PlayOneShot(chaserJumpSound);
        }
    }

    private void PlayDoubleJumpSound()
    {
        audioSource.PlayOneShot(chaserDoubleJumpSound);
    }


    private void Update()
    {
        if (!isLocalPlayer) return;

        if (isJumping && jumpButtonHeld)
        {
            float currentJumpTime = Time.time - jumpStartTime;
            float jumpProgress = currentJumpTime / maxJumpTime;

            float desiredJumpHeight = Mathf.Lerp(minJumpHeight * jumpForceMultiplier, maxJumpHeight * jumpForceMultiplier, jumpProgress);
            currentHeight = transform.position.y - initialYPosition;

            if (currentHeight < maxJumpHeight * 0.8f && !IsHittingCeiling())
            {
                ApplyJumpForce(desiredJumpHeight);
            }
            else
            {
                StartFalling();
            }
        }

        if (isFalling && rb.velocity.y > 0)
        {
            rb.gravityScale = gravityMultiplierDescend;
        }

        if (!IsGrounded() && !isJumping && !isFalling && rb.velocity.y < 0)
        {
            isFallingFree = true;
            rb.gravityScale = gravityMultiplierFall;
        }

        if (isDashing && Time.time >= dashEndTime)
        {
            isDashing = false;
        }

        if (IsGrounded())
        {
            lastGroundedTime = Time.time;
            isFalling = false;
            isFallingFree = false;
            rb.gravityScale = originalGravityScale;
            boxCollider.sharedMaterial = materialWithFriction;  // Activar fricción cuando está en el suelo
        }
        else
        {
            boxCollider.sharedMaterial = materialNoFriction;  // Desactivar fricción cuando está en el aire
        }
    }

    private bool IsHittingCeiling()
    {
        return Physics2D.OverlapCircle(ceilingCheck.position, groundCheckRadius, groundLayer);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        if (carditem.hit == false)
        {
            if (!isDashing)
                rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
            else
                rb.velocity = new Vector2(moveInput.x * moveSpeed * 2, rb.velocity.y);
        }
        else
        {
            if (playerType == PlayerType.Runner)
            {
                if (!isDashing)
                    rb.velocity = new Vector2(moveInput.x * -1 * moveSpeed, rb.velocity.y);
                else
                    rb.velocity = new Vector2(moveInput.x * -1 * moveSpeed * 2, rb.velocity.y);
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private bool IsPlayerNearby()
    {
        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            if (player != this && Vector2.Distance(transform.position, player.transform.position) <= proximityThreshold)
            {
                return true;
            }
        }
        return false;
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

            do
            {
                newColorIndex = Random.Range(0, colores.Length);
            } while (newColorIndex == lastColorIndex);

            this.renderer.color = colores[newColorIndex];
            lastColorIndex = newColorIndex;

            yield return new WaitForSeconds(0.1f);
        }

        this.renderer.color = new Color(0.3143499f, 1.0f, 0.0f, 1.0f);
    }
}
