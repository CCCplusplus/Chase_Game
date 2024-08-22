using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;

public class GunScript : MonoBehaviour
{
    public enum AmmoType { Ice, Fire }
    public AmmoType currentAmmoType;

    public SpriteRenderer weaponSpriteRenderer;
    public Color iceColor = Color.blue;
    public Color fireColor = Color.red;

    public GameObject iceBulletPrefab;
    public GameObject fireBulletPrefab;

    public List<GameObject> bulletPool; // pool de balas
    public int poolSize = 3; // tamaño del pool
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 10f;

    public AudioClip iceSound;
    public AudioClip fireSound;
    private AudioSource audioSource;

    private bool isHeld = false;
    private GameObject player;
    public bool isShooting = false;

    private int shotsLeft = 3; // limite de disparos

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        RandomizeAmmoType();
        FillBulletPool();
    }

    void Update()
    {
        if (isHeld && player != null)
        {
            var inputActions = player.GetComponent<PlayerInput>().actions;
            if (inputActions["Use-Item"].triggered)
            {
                isShooting = false;
                Shoot();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && !isHeld)
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
        isShooting = true;
        if (shotsLeft == 0)
        {
            DestroyGun();
            return;
        }

        GameObject bullet = GetBulletFromPool();

        if (bullet != null)
        {
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = bulletSpawnPoint.rotation;
            bullet.SetActive(true);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = bulletSpawnPoint.right * bulletSpeed;

            PlayShootSound();
            shotsLeft--;
        }
    }

    GameObject GetBulletFromPool()
    {
        foreach (var bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }

        return null;
    }

    void PlayShootSound()
    {
        if (currentAmmoType == AmmoType.Ice)
        {
            audioSource.PlayOneShot(iceSound);
        }
        else if (currentAmmoType == AmmoType.Fire)
        {
            audioSource.PlayOneShot(fireSound);
        }
    }

    void DestroyGun()
    {
        Destroy(gameObject);
    }

    void RandomizeAmmoType()
    {
        currentAmmoType = (AmmoType)Random.Range(0, 2);

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

    void FillBulletPool()
    {
        GameObject bulletPrefab = currentAmmoType == AmmoType.Ice ? iceBulletPrefab : fireBulletPrefab;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }
    }
}

