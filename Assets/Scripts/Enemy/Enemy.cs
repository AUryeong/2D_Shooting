using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected Color defaultColor;
    protected WaitForSeconds wait = new WaitForSeconds(0.12f);
    public float moveSpeed;
    protected float speed;

    public float hp;
    public float defaultHp;
    protected float time;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }

    protected virtual void OnEnable()
    {
        hp = defaultHp;
        spriteRenderer.color = defaultColor;
        speed = moveSpeed * Random.Range(0.7f, 1.3f);
        time = 0;
    }

    protected virtual void Update()
    {
        Move();
    }


    protected void Move()
    {
        time += Time.deltaTime;
        transform.Translate(new Vector3(Mathf.Sin(time * 3) * Time.deltaTime * 0.1f, -Time.deltaTime * speed, 0), Space.World);
        if (IngameManager.Instance.OutlineBoundForEnemy(transform.position))
            gameObject.SetActive(false);
    }
    public virtual void Hit(float multiplier = 1)
    {
        if (hp <= 0) return;
        hp -= Player.Instance.power * Random.Range(0.8f, 1.2f) * multiplier;
        if (hp <= 0)
            Die();
        else
            StartCoroutine(HitEffect());
    }

    public virtual void Die()
    {
        gameObject.SetActive(false);
        IngameManager.Instance.Score += (int)(Mathf.Pow(defaultHp + Random.Range(5, 10), 2) * moveSpeed) + Random.Range(30, 70);

        float random = Random.Range(0f, 1f);
        if (random <= 0.4f)
        {
            GameObject skillObj = PoolManager.Instance.Init(IngameManager.Instance.skillItem.gameObject);
            skillObj.transform.position = transform.position;
        }
        GameObject expObj = PoolManager.Instance.Init(IngameManager.Instance.exp.gameObject);
        expObj.transform.position = transform.position;
        expObj.GetComponent<Exp>().exp = (int)(Mathf.Pow(defaultHp, 2) + Mathf.Pow(moveSpeed, 2)) + Random.Range(3, 10) + Mathf.RoundToInt(Player.Instance.MaxExp / 1000f);

        PoolManager.Instance.Init(IngameManager.Instance.dieEffect.gameObject).transform.position = transform.position;
    }

    protected virtual IEnumerator HitEffect()
    {
        spriteRenderer.color = Color.white;
        yield return wait;
        spriteRenderer.color = defaultColor;
    }
}
