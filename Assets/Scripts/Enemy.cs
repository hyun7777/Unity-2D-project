using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    [SerializeField]
    private GameManager gameManager;

    void OnHit(int dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            if (gameManager != null)
            {
                gameManager.stagepoint += 100; // 적 처치 점수 100 예시
            }

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Fire")
        {
            Fire fire = collision.gameObject.GetComponent<Fire>();
            OnHit(fire.dmg);

            Destroy(collision.gameObject);
        }
    }
}
