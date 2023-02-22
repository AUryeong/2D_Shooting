using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Enemy,
    Player
}
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
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if(team == Team.Enemy)
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
                collider2D.GetComponent<Enemy>().Hit();
                gameObject.SetActive(false);
            }
        }
    }
}