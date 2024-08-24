using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class ShotgunHitbox : NetworkBehaviour
{
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private BoxCollider2D shotgunHitbox;
    [SerializeField] private ParticleSystem muzzleFlash;

    [SyncVar] public bool hasShotgun = false;
    private bool isShooting = false;

    [SerializeField] private Transform hitboxOrigin;
    [SerializeField] private float hitboxSpeed = 20f;
    [SerializeField] private float hitboxDistance = 5f; 

    private Vector2 originalPosition;

    private void Start()
    {
        if (shotgunHitbox != null)
        {
            shotgunHitbox.enabled = false;
            originalPosition = shotgunHitbox.transform.localPosition;
        }
    }

    private void Update()
    {
        if (hasShotgun && isLocalPlayer)
        {
            var inputActions = GetComponentInParent<PlayerInput>().actions;

            if (inputActions["Use-Item"].triggered && !isShooting)
            {
                CmdFire();
            }
        }

        if (shotgunHitbox != null)
        {
            shotgunHitbox.enabled = isShooting;
        }
    }

    [Command]
    private void CmdFire()
    {
        RpcFire();
    }

    [ClientRpc]
    private void RpcFire()
    {
        if (shootSound != null)
            AudioSource.PlayClipAtPoint(shootSound, transform.position);

        if (muzzleFlash != null)
            muzzleFlash.Play();

        StartCoroutine(MoveHitbox());
    }

    private IEnumerator MoveHitbox()
    {
        isShooting = true;

        
        Vector2 targetPosition = originalPosition + Vector2.right * hitboxDistance;
        float elapsedTime = 0f;
        while (elapsedTime < hitboxDistance / hitboxSpeed)
        {
            shotgunHitbox.transform.localPosition = Vector2.Lerp(originalPosition, targetPosition, (elapsedTime * hitboxSpeed) / hitboxDistance);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        shotgunHitbox.transform.localPosition = targetPosition;

        // Wait for a short duration to simulate impact
        yield return new WaitForSeconds(0.2f);

        // Move the hitbox back to its original position
        elapsedTime = 0f;
        while (elapsedTime < hitboxDistance / hitboxSpeed)
        {
            shotgunHitbox.transform.localPosition = Vector2.Lerp(targetPosition, originalPosition, (elapsedTime * hitboxSpeed) / hitboxDistance);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        shotgunHitbox.transform.localPosition = originalPosition;

        isShooting = false;
    }
}
