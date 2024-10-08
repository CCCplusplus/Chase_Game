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
    private float moveSpeed = 12f;
    [SerializeField]
    private float minJumpHeight = 4f;
    [SerializeField]
    private float maxJumpHeight = 8f;
    [SerializeField]
    private float dashDuration = 1.5f;
    [SerializeField]
    private float dashCooldown = 5f;
    [SerializeField]
    private float speedBoost = 1.5f;
    [SerializeField]
    private Color[] colores;
    [SerializeField]
    private GameObject pausa;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private Transform ceilingCheck; // Para detecci�n de colisiones con el techo
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


    public CardItem carditem;
    public ShotgunItem shotgun;

    //------------------------------------------------(Marco Antonio)
    [SerializeField] private ParticleSystem dashParticles;

    //private ParticleSystem dashParticlesRight;
    //private ParticleSystem dashParticlesRight;

    [SerializeField] private SpriteRenderer spriteRd;
    [SerializeField] private GameObject associatedObject;
    private bool isFacingRight = true;
    //------------------------------------------------

    [SerializeField]
    private AudioClip runnerJumpSound;
    [SerializeField]
    private AudioClip chaserJumpSound;
    [SerializeField]
    private AudioClip chaserDoubleJumpSound;
    [SerializeField]
    private float proximityThreshold = 5f; // Umbral de proximidad para o�r sonidos de otros jugadores

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip dashSound;

    [SerializeField]
    private GameObject jumpvfxHolder;
    private ParticleSystem jumpvfx;

    [SerializeField]
    private PhysicsMaterial2D materialWithFriction;  // Material con fricci�n
    [SerializeField]
    private PhysicsMaterial2D materialNoFriction;    // Material sin fricci�n

    //Animacion
    [SerializeField]
    public Animator animator;
    [SerializeField]
    public RuntimeAnimatorController defaultController;
    [SerializeField]
    public RuntimeAnimatorController shotgunController;

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
    public bool died = false;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private SpriteRenderer renderer;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    private float jumpStartTime;
    private bool isJumping;
    private bool jumpButtonHeld;
    private bool isFalling;
    private bool isFallingFree; // Para detectar ca�da libre
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

        //------------------------------------------------(Marco Antonio)
        //Apague el Awake de el Particle System  esta linea no es necesar�a (Carlos)
        //if(dashParticles != null)
        //{
        //    dashParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        //}

        spriteRd = GetComponent<SpriteRenderer>();
        //------------------------------------------------
        jumpvfx = jumpvfxHolder.GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

    }

    //------------------------------------------------(Marco Antonio)
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180f, 0f);
       
    }
    //------------------------------------------------
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
        PlayJumpVfx();
        if (!isLocalPlayer)
        {
            rb.velocity = new Vector2(rb.velocity.x, currentJumpVelocity);
            if (IsPlayerNearby())
            {    
                PlayJumpSound();
                PlayJumpVfx();
            }
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
        RpcActivateDashParticles();  // Activata para todos los jugadores.
    }

    [ClientRpc]
    private void RpcDash(float dashDuration)
    {
        audioSource.PlayOneShot(dashSound);
        if (isLocalPlayer)
            StartCoroutine(Dash(dashDuration));
    }

    [ClientRpc]
    private void RpcActivateDashParticles()
    {
        if (dashParticles != null && !dashParticles.isPlaying)
        {
            dashParticles.gameObject.SetActive(true);
            dashParticles.Play();
        }
    }

    [ClientRpc]
    private void RpcDeactivateDashParticles()
    {
        if (dashParticles != null && dashParticles.isPlaying)
        {
            dashParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            dashParticles.gameObject.SetActive(false);
        }
    }

    private IEnumerator Dash(float duration)
    {
        isDashing = true;

        yield return new WaitForSeconds(duration);

        isDashing = false;
        RpcDeactivateDashParticles();  // Desactiva para todos los jugadores
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
            audioSource.PlayOneShot(runnerJumpSound);
        else if (playerType == PlayerType.Chaser)
            audioSource.PlayOneShot(chaserJumpSound);
    }

    private void PlayJumpVfx()
    {
        jumpvfx.Play();
    }

    private void PlayDoubleJumpSound()
    {
        audioSource.PlayOneShot(chaserDoubleJumpSound);
    }
    private void Update()
    {
        if (!isLocalPlayer) return;

        if (died)
        {
            if (playerType == PlayerType.Runner)
            {
                died = false;
                StartCoroutine(Invencible2());
                StartCoroutine(InvincibilityColorChange());
            }
            else { died = false; }
        }

        if (shotgun != null)
        {
            if (shotgun.shotgunPicked)
            {
                if (animator.runtimeAnimatorController != shotgunController)
                {
                    animator.runtimeAnimatorController = shotgunController;
                }
            }
            else
            {
                if (animator.runtimeAnimatorController != defaultController)
                {
                    animator.runtimeAnimatorController = defaultController;
                }
            }
        }

        HandleMovementAnimations();

        if (shotgun != null)
        {
            if (shotgun.shotgunPicked)
                HandleShotgunAnimations();
        }
        //------------------------------------------------

        if (isJumping && jumpButtonHeld)
        {
            //animator.SetBool("Jumping", true);

            float currentJumpTime = Time.time - jumpStartTime;
            float jumpProgress = currentJumpTime / maxJumpTime;

            float desiredJumpHeight = Mathf.Lerp(minJumpHeight * jumpForceMultiplier, maxJumpHeight * jumpForceMultiplier, jumpProgress);
            currentHeight = transform.position.y - initialYPosition;

            if (currentHeight < maxJumpHeight * 0.8f && !IsHittingCeiling())
                ApplyJumpForce(desiredJumpHeight);
            else
                StartFalling();
        }

        if (isFalling && rb.velocity.y > 0)
            rb.gravityScale = gravityMultiplierDescend;

        if (!IsGrounded() && !isJumping && !isFalling && rb.velocity.y < 0)
        {
            isFallingFree = true;
            rb.gravityScale = gravityMultiplierFall;
        }

        if (isDashing && Time.time >= dashEndTime)
            isDashing = false;
    }

    private void HandleMovementAnimations()
    {
        float moveDirection = moveInput.x;

        if (moveDirection < 0 && isFacingRight)
        {
            Flip(); // Voltea a la izquierda.
        }
        else if (moveDirection > 0 && !isFacingRight)
        {
            Flip(); // Voltea a la derecha.
        }

        //Moving es true siempre que moveDirection sea diferente a cero
        animator.SetBool("Moving", moveDirection != 0);

        //Siempre que el jugador no este en el suelo, Jumping es true
        bool isInAir = isJumping || isFalling || !IsGrounded();
        animator.SetBool("Jumping", isInAir);
    }

    void HandleShotgunAnimations()
    {
        var inputActions = GetComponentInParent<PlayerInput>().actions;

        if (inputActions["UseItem"].triggered)
        {
            animator.SetBool("Shooting", true);
        }
        else 
        { 
            animator.SetBool("Shooting", false);
        }
    }

    private bool IsHittingCeiling()
    {
        return Physics2D.OverlapCircle(ceilingCheck.position, groundCheckRadius, groundLayer);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        if (IsGrounded())
        {
            lastGroundedTime = Time.time;
            isFalling = false;
            isFallingFree = false;
            rb.gravityScale = originalGravityScale;
            boxCollider.sharedMaterial = materialWithFriction;  // Activar fricci�n cuando est� en el suelo
        }
        else
        {
            boxCollider.sharedMaterial = materialNoFriction;  // Desactivar fricci�n cuando est� en el aire
        }

        UpdateMovement();
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
        CmdSetTag("Invencible"); // Comando para cambiar la etiqueta en todos los clientes
        yield return new WaitForSeconds(dashDuration);
        CmdSetTag("Runner"); // Comando para volver a la etiqueta original en todos los clientes
    }

    private IEnumerator Invencible2()
    {
        CmdSetTag("Invencible"); // Comando para cambiar la etiqueta en todos los clientes
        yield return new WaitForSeconds(2.0f);
        CmdSetTag("Runner"); // Comando para volver a la etiqueta original en todos los clientes
    }

    [Command]
    private void CmdSetTag(string newTag)
    {
        RpcSetTag(newTag);
    }

    [ClientRpc]
    private void RpcSetTag(string newTag)
    {
        this.tag = newTag;
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

            CmdChangeColor(newColorIndex);
            lastColorIndex = newColorIndex;

            yield return new WaitForSeconds(0.1f);
        }

        CmdChangeColorFinal(new Color(255f, 255f, 255f, 255f));
    }

    private IEnumerator InvincibilityColorChange()
    {
        Color semiTransparentColor = new Color(255f, 255f, 255f, 0.5f); 
        Color originalColor = new Color(255f, 255f, 255f, 255f); 
        float blinkInterval = 0.1f;
        float invincibilityDuration = 2.0f;
        float elapsedTime = 0f;

        while (elapsedTime < invincibilityDuration)
        {
            
            CmdChangeColorFinal(semiTransparentColor); 
            yield return new WaitForSeconds(blinkInterval);

            
            CmdChangeColorFinal(originalColor); 
            yield return new WaitForSeconds(blinkInterval);

            elapsedTime += blinkInterval * 2;
        }

        CmdChangeColorFinal(originalColor);
    }


    [Command]
    private void CmdChangeColor(int colorIndex)
    {
        RpcChangeColor(colorIndex);
    }

    [ClientRpc]
    private void RpcChangeColor(int colorIndex)
    {
        this.renderer.color = colores[colorIndex];
    }

    [Command]
    private void CmdChangeColorFinal(Color finalColor)
    {
        RpcChangeColorFinal(finalColor);
    }

    [ClientRpc]
    private void RpcChangeColorFinal(Color finalColor)
    {
        this.renderer.color = finalColor;
    }

    private void UpdateMovement()
    {
        float moveDirection = moveInput.x;

        if (carditem != null)
        {
            if (carditem.hit == false)
            {
                if (!isDashing)
                    rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
                else
                    rb.velocity = new Vector2(moveInput.x * moveSpeed * speedBoost, rb.velocity.y);
            }
            else
            {
                if (playerType == PlayerType.Runner)
                {
                    if (!isDashing)
                        rb.velocity = new Vector2(moveInput.x * -1 * moveSpeed, rb.velocity.y);
                    else
                        rb.velocity = new Vector2(moveInput.x * -1 * moveSpeed * speedBoost, rb.velocity.y);
                }
            }
        }
        else
        {
            if (!isDashing)
                rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
            else
                rb.velocity = new Vector2(moveInput.x * moveSpeed * speedBoost, rb.velocity.y);
        }
    }

}