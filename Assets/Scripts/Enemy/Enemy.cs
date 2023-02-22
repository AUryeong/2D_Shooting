using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed;

    public float hp;
    public float defaultHp;

    protected virtual void OnEnable()
    {
        hp = defaultHp;
    }

    protected virtual void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(Vector2.down * Time.deltaTime * moveSpeed);
        if (IngameManager.Instance.OutlineBound(transform.position))
            gameObject.SetActive(false);
    }
    public void Hit()
    {
        hp -= Player.Instance.power * Random.Range(0.8f,1.2f);
        if (hp <= 0)
            gameObject.SetActive(false);
    }
}
