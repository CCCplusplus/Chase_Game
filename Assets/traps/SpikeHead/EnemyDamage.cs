using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] protected float damage;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Chaser" || collision.tag == "Runner")
        {
            Debug.Log("Morido");
            collision.gameObject.SetActive(false);
        }
    }
}
