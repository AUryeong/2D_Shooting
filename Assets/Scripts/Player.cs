using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    SpriteRenderer spriteRenderer;

    public float moveSpeed;

    public int level = 1;
    public int exp = 0;

    public int defaultMaxExp = 100;
    public int maxExp;

    public int defaulthp = 3;
    private int hp;

    bool hitable;

    [Header("ÃÑ¾Ë °ü·Ã")]
    [SerializeField] Bullet playerBullet;

    [Space(20f)]
    public float defaultPower = 1;
    public float power;

    public float defaultAttackSpeed = 0.1f;
    public float attackSpeed;

    public int defaultMultiplier = 1;
    public int multiplier;

    private float attackDuration;

    public int GetMaxNextExp(int nowMaxExp)
    {
        return (int)Mathf.Pow(nowMaxExp, 1.1f);
    }

    protected override void OnCreated()
    {
        base.OnCreated();
        spriteRenderer = GetComponent<SpriteRenderer>();

        level = 1;
        exp = 0;

        hp = defaulthp;
        power = defaultPower;
        multiplier = defaultMultiplier;
        attackSpeed = defaultAttackSpeed;
        hitable = true;
    }

    private void Update()
    {
        ShootUpdate();
        Move();
    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        transform.position = IngameManager.Instance.OutlineCheck(transform.position + (new Vector3(x, y) * moveSpeed * Time.deltaTime));
    }

    private void ShootUpdate()
    {
        attackDuration += Time.deltaTime;
        if (attackDuration >= attackSpeed)
        {
            attackDuration -= attackSpeed;

            Vector3 left = transform.position + new Vector3(0.2f * (multiplier - 1), 0, 0);

            for (int i = 0; i < multiplier; i++)
            {
                GameObject obj = PoolManager.Instance.Init(playerBullet.gameObject);
                obj.transform.position = left - new Vector3(0.4f * i, 0, 0);
            }

        }
    }
    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag(nameof(Enemy)))
        {
            if (hitable)
                Hit();
        }
    }

    public void Hit()
    {
        StartCoroutine(HitInv());
    }

    IEnumerator HitInv()
    {
        gameObject.layer = LayerMask.NameToLayer("PlayerInv");
        spriteRenderer.color = new Color(1, 1, 1, 0.3f);
        hitable = false;
        IngameManager.Instance.CameraShake(0.3f, 0.2f);

        float recoverTime = 1;
        while (recoverTime > 0)
        {
            recoverTime -= Time.deltaTime;
            spriteRenderer.color = new Color(1, 1, 1, 0.3f + (1 - recoverTime) * 0.7f);
            yield return null;
        }

        gameObject.layer = LayerMask.NameToLayer("Player");
        hitable = true;
    }
}
