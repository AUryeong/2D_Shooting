using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Enemy,
    Player
}
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public Team team;
    public Vector2 direction;
    public float speed;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime);
        if (IngameManager.Instance.OutlineBound(transform.position))
        {
            gameObject.SetActive(false);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (team == Team.Enemy)
        {
            if (collider2D.CompareTag(nameof(Player)))
            {
                Player.Instance.Hit();
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (collider2D.CompareTag(nameof(Enemy)))
            {
                Enemy enemy = collider2D.GetComponent<Enemy>();
                if (enemy == null)
                    enemy = collider2D.transform.parent.GetComponent<Enemy>();
                enemy.Hit();
                gameObject.SetActive(false);
            }
        }
    }
}
