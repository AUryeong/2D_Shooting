using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideSideBoss : Boss
{
    [SerializeField] Bullet bullet;
    protected override void OnEnable()
    {
        hp = defaultHp;
        spriteRenderer.color = defaultColor;
        speed = moveSpeed * Random.Range(0.7f, 1.3f);
        time = 0;
        intro = true;
        StartCoroutine(BossPattern());
    }

    protected override IEnumerator BossPattern()
    {
        yield return base.BossPattern();
        var wait = new WaitForSeconds(5);
        while (true)
        {
            for (int i = 0; i < 18; i++)
            {
                GameObject obj = PoolManager.Instance.Init(bullet.gameObject);
                obj.transform.rotation = Quaternion.Euler(0, 0, i * 20);
                obj.transform.position = transform.position;
            }
            yield return wait;
        }
    }

    public override void Die()
    {
        IngameManager.Instance.Score += 3000;
        IngameManager.Instance.uiManager.bossBar.gameObject.SetActive(false);
        IngameManager.Instance.isBoss = false;
        Player.Instance.Exp += Player.Instance.MaxExp;
        PoolManager.Instance.Init(IngameManager.Instance.bossDeathEffect.gameObject).transform.position = transform.position;
        Destroy(gameObject);
    }
}
