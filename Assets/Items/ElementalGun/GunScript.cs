using UnityEngine.InputSystem;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public enum AmmoType { Ice, Fire }
    public AmmoType currentAmmoType;

    public SpriteRenderer weaponSpriteRenderer;
    public Color iceColor = Color.blue;
    public Color fireColor = Color.red;

    public GameObject iceBulletPrefab;
    public GameObject fireBulletPrefab;

    public Transform bulletSpawnPoint;
    public float bulletSpeed = 10f;

    public AudioClip iceSound;
    public AudioClip fireSound;
    private AudioSource audioSource;

    private bool isHeld = false;
    private GameObject player;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        RandomizeAmmoType();
    }

    void Update()
    {
        if (isHeld && player != null)
        {
            var inputActions = player.GetComponent<PlayerInput>().actions;
            if (inputActions["Use-Item"].triggered)
                Shoot();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Chaser") || collision.CompareTag("Runner") && !isHeld)
        {
            PickUpItem(collision.gameObject);
        }
    }

    void PickUpItem(GameObject playerObject)
    {
        isHeld = true;
        player = playerObject;
        transform.SetParent(player.transform);

        Transform holdPos = player.transform.Find("HoldPosition");
        if (holdPos != null)
        {
            transform.position = holdPos.position;
        }

        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    void Shoot()
    {
        GameObject bullet = null;

        if (currentAmmoType == AmmoType.Ice)
        {
            bullet = Instantiate(iceBulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            audioSource.PlayOneShot(iceSound);
        }
        else if (currentAmmoType == AmmoType.Fire)
        {
            bullet = Instantiate(fireBulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            audioSource.PlayOneShot(fireSound);
        }

        if (bullet != null)
        {
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = bulletSpawnPoint.right * bulletSpeed;
        }
    }

    void RandomizeAmmoType()
    {
        currentAmmoType = (AmmoType)Random.Range(0, 2); // Aleatorio entre 0 y 1

        switch (currentAmmoType)
        {
            case AmmoType.Ice:
                weaponSpriteRenderer.color = iceColor;
                break;
            case AmmoType.Fire:
                weaponSpriteRenderer.color = fireColor;
                break;
        }
    }
}
