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
    [SerializeField]
    private GameObject bulletHitG;
    [SerializeField] 
    private bulletScript bulletHit;
    [SerializeField]
    private GunScript gunScript;

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
    private float proximityThreshold = 5f; // Umbral de proximidad para oír sonidos de otros jugadores

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip dashSound;

    [SerializeField]
    private GameObject jumpvfxHolder;
    private ParticleSystem jumpvfx;

    [SerializeField]
    private PhysicsMaterial2D materialWithFriction;  // Material con fricción
    [SerializeField]
    private PhysicsMaterial2D materialNoFriction;    // Material sin fricción

    //Animacion
    [SerializeField]
    public Animator animator;

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

        //------------------------------------------------(Marco Antonio)
        //Apague el Awake de el Particle System  esta linea no es necesaría (Carlos)
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
        carditemG = GameObject.FindGameObjectWithTag("Card");
        if (carditemG != null) 
            carditem = carditemG.GetComponent<CardItem>();

        //bulletHitG = GameObject.FindGameObjectWithTag("Bullet");
        bulletHit = null;
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

        //if (moveInput.x != 0)
        //{
        //    CmdSetMoving(true);
        //}
        //else
        //{
        //    CmdSetMoving(false);
        //}

        CmdMove(moveInput);
    }

    //[Command]
    //private void CmdSetMoving(bool isMoving)
    //{
    //    RpcSetMoving(isMoving);
    //}
    //
    //[ClientRpc]
    //private void RpcSetMoving(bool isMoving)
    //{
    //    animator.SetBool("Moving", isMoving);
    //}

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
            //animator.SetBool("Moving", moveInput.x != 0);
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
                //CmdSetJumpingAnimation(true);
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
                //CmdSetJumpingAnimation(true);
            }
        }
        else if (context.canceled && isJumping)
        {
            isJumping = false;
            jumpButtonHeld = false;
            StartFalling();
            //CmdSetJumpingAnimation(false);
        }
    }

    //[Command]
    //private void CmdSetJumpingAnimation(bool isJumping)
    //{
    //    RpcSetJumpingAnimation(isJumping);
    //}
    //
    //[ClientRpc]
    //private void RpcSetJumpingAnimation(bool isJumping)
    //{
    //    animator.SetBool("Jumping", isJumping);
    //}

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
        AudioSource.PlayClipAtPoint(dashSound, transform.position);
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


        //------------------------------------------------(Marco Antonio)
        ////Flip del sprite basado en la direccion de movimiento
        //float moveDirection = moveInput.x;
        //if (moveDirection < 0)
        //{
        //    CmdFlipSprite(true); //Llama al comando para flip en red
        //}
        //else if (moveDirection > 0)
        //{
        //    CmdFlipSprite(false);
        //}

        float moveDirection = moveInput.x;
        if (moveDirection < 0 && isFacingRight)
        {
            animator.SetBool("Moving", true);
            Flip(); // Voltea a la izquierda.
            //CmdFlipSprite(true); //Llama al comando para flip en red
        }
        else if (moveDirection > 0 && !isFacingRight)
        {
            animator.SetBool("Moving", true);
            Flip(); // Voltea a la derecha.
            //CmdFlipSprite(false);
        }else if (moveDirection == 0)
        {
            animator.SetBool("Moving", false);
        }

        if (isJumping)
        {
            animator.SetBool("Jumping", true);
        }
        else if (isFalling || !IsGrounded())
        {
            animator.SetBool("Jumping", false);
        }
        else
        {
            animator.SetBool("Jumping", false);
        }

        //------------------------------------------------

        //UpdateAnimation();

        if (isJumping && jumpButtonHeld)
        {
            animator.SetBool("Jumping", true);

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

        if(gunScript.isShooting == false)
        {
            return;
        }
        else
        {
            bulletHitG = GameObject.FindGameObjectWithTag("Bullet");
            bulletHit = bulletHitG.GetComponent<bulletScript>();
        }
    }

    //private void UpdateAnimation()
    //{
    //    if (IsGrounded())
    //    {
    //        CmdSetJumpingAnimation(false);
    //    }
    //    else if (isJumping)
    //    {
    //        CmdSetJumpingAnimation(true);
    //    }
    //}
 
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
            boxCollider.sharedMaterial = materialWithFriction;  // Activar fricción cuando está en el suelo
            //CmdSetJumpingAnimation(false);
        }
        else
        {
            boxCollider.sharedMaterial = materialNoFriction;  // Desactivar fricción cuando está en el aire
        }

        UpdateMovement();

        if (bulletHit == false)
        {
            return;
        }
        else
        {
            if (playerType == PlayerType.Runner)
            {
                if (bulletHit.ammotype.currentAmmoType == GunScript.AmmoType.Ice)
                {
                    moveSpeed = 2f;
                    rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
                }
                else if (bulletHit.ammotype.currentAmmoType == GunScript.AmmoType.Fire)
                {
                    moveSpeed = 7f;
                    rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
                }
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
        CmdSetTag("Invencible"); // Comando para cambiar la etiqueta en todos los clientes
        yield return new WaitForSeconds(dashDuration);
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

            CmdChangeColor(newColorIndex);  // Comando para cambiar el color en todos los clientes
            lastColorIndex = newColorIndex;

            yield return new WaitForSeconds(0.1f);
        }

        CmdChangeColorFinal(new Color(255f, 255f, 255f, 255f)); // Comando para establecer el color final
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
        else
        {
            if (!isDashing)
                rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
            else
                rb.velocity = new Vector2(moveInput.x * moveSpeed * 2, rb.velocity.y);
        }
    }

}