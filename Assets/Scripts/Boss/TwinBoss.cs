using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBoss : Boss
{
    [SerializeField] Bullet bullet;
    [SerializeField] BoomEnemy boomMob;

    [SerializeField] SpriteRenderer first;
    [SerializeField] SpriteRenderer second;

    [SerializeField] float dashSpeed;
    [SerializeField] float rollSpeed = 1;
    List<Bullet> bulletList = new List<Bullet>();

    int bumpIdx = 0;

    public float radius
    {
        set
        {
            first.transform.localPosition = new Vector3(value, 0, 0);
            second.transform.localPosition = new Vector3(-value, 0, 0);
        }
        get
        {
            return first.transform.localPosition.x;
        }
    }

    protected override void Awake()
    {
        defaultColor = first.color;
    }

    public override void Die()
    {
        IngameManager.Instance.Score += 40000;
        IngameManager.Instance.uiManager.bossBar.gameObject.SetActive(false);
        IngameManager.Instance.isBoss = false;
        Player.Instance.Exp += Player.Instance.MaxExp;
        PoolManager.Instance.Init(IngameManager.Instance.bossDeathEffect.gameObject).transform.position = first.transform.position;
        PoolManager.Instance.Init(IngameManager.Instance.bossDeathEffect.gameObject).transform.position = second.transform.position;
        Destroy(gameObject);
        foreach (var bullet in bulletList)
            bullet.gameObject.SetActive(false);
        IngameManager.Instance.NextStage();
    }

    protected override void OnEnable()
    {
        hp = defaultHp;
        time = 0;
        IngameManager.Instance.uiManager.bossBar.gameObject.SetActive(true);
        IngameManager.Instance.uiManager.boosBarGauge.fillAmount = 1;
        IngameManager.Instance.isBoss = true;
        intro = true;
        phaseTwo = false;
        StartCoroutine(BossPattern());
    }
    protected override IEnumerator HitEffect()
    {
        first.color = Color.white;
        second.color = Color.white;
        yield return wait;
        first.color = defaultColor;
        second.color = defaultColor;
    }
    protected override IEnumerator Return()
    {
        float moveTime = 1;
        Vector3 pos = transform.position;

        while (moveTime > 0)
        {
            moveTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(center, pos, moveTime);
            yield return null;
        }

        transform.position = center;
        isPatterning = false;
    }

    IEnumerator Wait()
    {
        if (phaseTwo)
        {
            var wait = new WaitForSeconds(2.5f);
            yield return wait;
            bumpIdx = (bumpIdx + 1) % 2;
            if (bumpIdx == 0)
                FirstBoom();
            else
                SecondBoom();
            yield return wait;
        }
        else
            yield return new WaitForSeconds(5);
    }

    protected override IEnumerator BossPattern()
    {
        yield return base.BossPattern();
        var wait = new WaitForSeconds(5);
        while (true)
        {
            yield return FirstPattern();
            yield return Return();
            yield return Wait();
            yield return SecondPattern();
            yield return Wait();
            yield return ThirdPattern();
            yield return Wait();
            yield return FourthPattern();
            yield return Wait();
        }
    }

    IEnumerator FirstPattern()
    {
        rollSpeed *= 4;
        float shootCooltime = 0.1f;
        if (phaseTwo)
            shootCooltime /= 2;
        float shootDuration = 0;
        while (!IngameManager.Instance.OutlineBound(transform.position))
        {
            shootDuration += Time.deltaTime;
            if (shootDuration >= shootCooltime)
            {
                shootDuration -= shootCooltime;
                GameObject obj = PoolManager.Instance.Init(bullet.gameObject);
                obj.transform.position = first.transform.position;
                obj.transform.rotation = first.transform.rotation;

                obj = PoolManager.Instance.Init(bullet.gameObject);
                obj.transform.position = second.transform.position;
                obj.transform.rotation = second.transform.rotation;
            }
            transform.Translate(0, -Time.deltaTime * dashSpeed, 0, Space.World);
            yield return null;
        }
        transform.position = IngameManager.Instance.OutlineCheck(transform.position);
        rollSpeed /= 4;
    }

    IEnumerator SecondPattern()
    {
        var wait = new WaitForSeconds(0.05f);
        var wait2 = new WaitForSeconds(0.15f);
        float bulletSpeed = bullet.speed;
        bulletList.Clear();

        int startIdx = Random.Range(0, 36);

        Vector3 playerPos = Player.Instance.transform.position;
        for (int i = startIdx; i < 36 * 2 + startIdx; i++)
        {
            float f = (i + 1) * 10 * Mathf.Deg2Rad;
            float size = 3;
            if (i >= 36 + startIdx)
            {
                size *= 2;
            }
            if (i % 2 == 0)
                size += 0.4f;

            Vector3 pos = playerPos + new Vector3(size * Mathf.Cos(f), size * Mathf.Sin(f));
            if (IngameManager.Instance.OutlineBound(pos))
                continue;

            GameObject obj = PoolManager.Instance.Init(bullet.gameObject);
            obj.transform.position = pos;
            if (i % 2 == 0)
                obj.transform.localScale *= 2;

            Bullet waitBullet = obj.GetComponent<Bullet>();
            waitBullet.speed = 0;
            if (!bulletList.Contains(waitBullet))
                bulletList.Add(waitBullet);
            yield return wait;
        }
        yield return new WaitForSeconds(1);
        for (int i = 0; i < bulletList.Count; i++)
        {
            Vector3 vector = Player.Instance.transform.position - bulletList[i].transform.position;
            float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

            bulletList[i].transform.rotation = Quaternion.Euler(0, 0, angle + 90);
            bulletList[i].speed = bulletSpeed;
            yield return wait2;
        }
    }

    IEnumerator ThirdPattern()
    {
        var wait = new WaitForSeconds(1);
        yield return FirstBoom();
        yield return FirstBoom();
        yield return wait;
        yield return SecondBoom();
        yield return SecondBoom();
        yield return wait;
        wait = new WaitForSeconds(0.5f);
        yield return FirstBoom();
        yield return FirstBoom();
        yield return wait;
        yield return SecondBoom();
        yield return SecondBoom();
        yield return wait;
        yield return FirstBoom();
        yield return SecondBoom();
        StartCoroutine(FirstBoom());
        StartCoroutine(SecondBoom());
    }

    IEnumerator FourthPattern()
    {
        var wait = new WaitForSeconds(0.2f);
        for (int i = 0; i < 6; i++)
        {
            PoolManager.Instance.Init(boomMob.gameObject).transform.position = new Vector2(-7.4f + 1.8f * i + Random.Range(1f, -1f), 5);
            yield return wait;
        }
        yield return new WaitForSeconds(1);
    }

    IEnumerator FirstBoom()
    {
        float scaleUpDuration = 0.1f;
        Vector3 originScale = first.transform.localScale;
        Vector3 toScale = Vector3.one * 1.2f;
        while (scaleUpDuration > 0)
        {
            scaleUpDuration -= Time.deltaTime;
            first.transform.localScale = Vector3.Lerp(toScale, originScale, scaleUpDuration / 0.2f);
            yield return null;
        }
        for (int i = 0; i < 24; i++)
        {
            GameObject obj = PoolManager.Instance.Init(bullet.gameObject);
            obj.transform.rotation = Quaternion.Euler(0, 0, i * 15);
            obj.transform.position = first.transform.position;
        }
        float scaleDownDuration = 0.2f;
        while (scaleDownDuration > 0)
        {
            scaleDownDuration -= Time.deltaTime;
            first.transform.localScale = Vector3.Lerp(originScale, toScale, scaleDownDuration / 0.4f);
            yield return null;
        }
    }

    IEnumerator SecondBoom()
    {
        float scaleUpDuration = 0.1f;
        Vector3 originScale = second.transform.localScale;
        Vector3 toScale = Vector3.one * 1.2f;
        while (scaleUpDuration > 0)
        {
            scaleUpDuration -= Time.deltaTime;
            second.transform.localScale = Vector3.Lerp(originScale, toScale, scaleUpDuration / 0.2f);
            yield return null;
        }
        for (int i = 0; i < 24; i++)
        {
            GameObject obj = PoolManager.Instance.Init(bullet.gameObject);
            obj.transform.rotation = Quaternion.Euler(0, 0, i * 15);
            obj.transform.position = second.transform.position;
        }
        float scaleDownDuration = 0.2f;
        while (scaleDownDuration > 0)
        {
            scaleDownDuration -= Time.deltaTime;
            second.transform.localScale = Vector3.Lerp(originScale, toScale, scaleDownDuration / 0.4f);
            yield return null;
        }
    }
    IEnumerator RaidusChange()
    {
        float radiusFadeTime = 2f;
        while (radiusFadeTime > 0)
        {
            radiusFadeTime -= Time.deltaTime;
            radius = Mathf.Lerp(3, 1.47f, radiusFadeTime / 2);
            yield return null;
        }
        while (true)
        {
            radiusFadeTime = 2f;
            while (radiusFadeTime > 0)
            {
                radiusFadeTime -= Time.deltaTime;
                radius = Mathf.Lerp(0.5f, 3, radiusFadeTime / 2);
                yield return null;
            }
            radius = 0.5f;

            radiusFadeTime = 2f;
            while (radiusFadeTime > 0)
            {
                radiusFadeTime -= Time.deltaTime;
                radius = Mathf.Lerp(3, 0.5f, radiusFadeTime / 2);
                yield return null;
            }
            radius = 3;
        }
    }
    protected override void PhaseTwo()
    {
        base.PhaseTwo();
        StartCoroutine(RaidusChange());
    }

    protected override void Update()
    {
        base.Update();
        transform.Rotate(new Vector3(0, 0, Time.deltaTime * 100 * rollSpeed));
    }
}
