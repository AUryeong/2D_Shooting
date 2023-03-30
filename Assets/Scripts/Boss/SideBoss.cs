using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideBoss : Boss
{
    [SerializeField] SpriteRenderer warning;
    [SerializeField] Bullet bullet;
    [SerializeField] Bullet gunBullet;
    [SerializeField] Bullet fastGunBullet;
    [SerializeField] Enemy defaultMob;

    Vector3 leftUp = new Vector3(-6.1f, 4, 0);
    Vector3 leftDown = new Vector3(-6.1f, -4, 0);
    Vector3 rightDown = new Vector3(1.9f, -4, 0);
    Vector3 rightUp = new Vector3(1.9f, 4, 0);
    public float dashSpeed;
    protected override void OnEnable()
    {
        base.OnEnable();
        warning.gameObject.SetActive(false);
    }

    protected IEnumerator Wait()
    {
        var wait = new WaitForSeconds(2.5f);
        if (phaseTwo)
        {
            yield return wait;
            for (int i = 0; i < 18; i++)
            {
                GameObject obj = PoolManager.Instance.Init(gunBullet.gameObject);
                obj.transform.rotation = Quaternion.Euler(0, 0, i * 20);
                obj.transform.position = transform.position;
                obj.transform.localScale = gunBullet.transform.localScale * 3f;
            }
            yield return wait;
        }
        else
            yield return new WaitForSeconds(5);
        isPatterning = true;
    }
    protected override IEnumerator BossPattern()
    {
        yield return base.BossPattern();
        while (true)
        {
            yield return FirstPattern();
            yield return Return();
            yield return Wait();
            yield return SecondPattern();
            yield return Return();
            yield return Wait();
            yield return ThirdPattern();
            yield return Wait();
            yield return FourthPattern();
            yield return Wait();
        }
    }

    IEnumerator FirstPattern()
    {
        yield return MoveTo(leftUp);
        yield return MoveTo(leftDown);
        yield return MoveTo(rightDown);
        yield return MoveTo(rightUp);
    }


    public override void Die()
    {
        base.Die();
        IngameManager.Instance.NextStage();
    }
    IEnumerator SecondPattern()
    {
        int repeat = 5;
        if (phaseTwo)
            repeat = 7;
        for (int j = 0; j < repeat; j++)
        {
            warning.transform.position = transform.position;
            warning.gameObject.SetActive(true);

            Vector3 vector = Player.Instance.transform.position - transform.position;
            float deg = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, deg + 90);

            warning.transform.Translate(Vector2.down * transform.localScale.x * 10);
            var wait = new WaitForSeconds(0.05f);
            for (int i = 0; i < 5; i++)
            {
                warning.color = new Color(warning.color.r, warning.color.g, warning.color.b, 0.45f);
                yield return wait;
                warning.color = new Color(warning.color.r, warning.color.g, warning.color.b, 0.15f);
                yield return wait;
            }
            warning.gameObject.SetActive(false);
            while (!IngameManager.Instance.OutlineBound(transform.position))
            {
                transform.Translate(0, -Time.deltaTime * dashSpeed, 0);
                yield return null;
            }
            transform.position = IngameManager.Instance.OutlineCheck(transform.position);
        }
    }

    IEnumerator ThirdPattern()
    {
        var wait = new WaitForSeconds(0.05f);
        for (int i = 0; i < 16; i++)
        {
            GameObject obj = PoolManager.Instance.Init(bullet.gameObject);
            obj.transform.rotation = Quaternion.Euler(0, 0, 240 + i * 10);
            obj.transform.position = transform.position;
            if (phaseTwo)
                obj.transform.localScale = bullet.transform.localScale * 1.2f;

            obj = PoolManager.Instance.Init(bullet.gameObject);
            obj.transform.rotation = Quaternion.Euler(0, 0, 60 + i * 10);
            obj.transform.position = transform.position;
            if (phaseTwo)
                obj.transform.localScale = bullet.transform.localScale * 1.2f;
            yield return wait;
        }
        for (int i = 0; i < 16; i++)
        {
            GameObject obj = PoolManager.Instance.Init(bullet.gameObject);
            obj.transform.rotation = Quaternion.Euler(0, 0, 120 + i * -10);
            obj.transform.position = transform.position;
            if (phaseTwo)
                obj.transform.localScale = bullet.transform.localScale * 1.2f;


            obj = PoolManager.Instance.Init(bullet.gameObject);
            obj.transform.rotation = Quaternion.Euler(0, 0, 300 + i * -10);
            obj.transform.position = transform.position;
            if (phaseTwo)
                obj.transform.localScale = bullet.transform.localScale * 1.2f;
            yield return wait;
        }
        wait = new WaitForSeconds(0.03f);
        for (int i = 0; i < 16; i++)
        {
            GameObject obj = PoolManager.Instance.Init(bullet.gameObject);
            obj.transform.rotation = Quaternion.Euler(0, 0, 240 + i * 10 + 5);
            obj.transform.position = transform.position;
            if (phaseTwo)
                obj.transform.localScale = bullet.transform.localScale * 1.2f;
            yield return wait;
        }
        for (int i = 0; i < 38; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject obj = PoolManager.Instance.Init(bullet.gameObject);
                obj.transform.rotation = Quaternion.Euler(0, 0, 35 + j * 90 + i * -10);
                obj.transform.position = transform.position;
                if (phaseTwo)
                    obj.transform.localScale = bullet.transform.localScale * 1.2f;
            }
            yield return wait;
        }
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 36; i++)
        {
            GameObject obj = i%2 == 0 ? PoolManager.Instance.Init(fastGunBullet.gameObject) : PoolManager.Instance.Init(bullet.gameObject);
            obj.transform.rotation = Quaternion.Euler(0, 0, i * 10 + Random.Range(5f, -5f));
            obj.transform.position = transform.position;
            if (phaseTwo)
                obj.transform.localScale = obj.transform.localScale * 1.2f;
        }
    }

    IEnumerator FourthPattern()
    {
        var wait = new WaitForSeconds(0.1f);
        for (int i = 0; i < 6; i++)
        {
            PoolManager.Instance.Init(defaultMob.gameObject).transform.position = new Vector2(-7.4f + 1.8f * i + Random.Range(1f, -1f), 5);
            yield return wait;
        }
        for (int i = 0; i < 6; i++)
        {
            PoolManager.Instance.Init(defaultMob.gameObject).transform.position = new Vector2(-6.9f + 1.8f * i + Random.Range(1f, -1f), 5);
            yield return wait;
        }
    }

    IEnumerator MoveTo(Vector3 wantPos)
    {
        float moveTime = 0.5f;
        Vector3 pos = transform.position;
        Vector3 vector = wantPos - pos;
        float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle + 90);
        while (moveTime > 0)
        {
            moveTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(wantPos, pos, moveTime * 2);
            yield return null;
        }
        transform.position = wantPos;

        for (int i = 0; i < 24; i++)
        {
            GameObject obj = PoolManager.Instance.Init(bullet.gameObject);
            obj.transform.rotation = Quaternion.Euler(0, 0, i * 15);
            obj.transform.position = transform.position;
            if (phaseTwo)
                obj.transform.localScale = bullet.transform.localScale * 1.15f;
        }
    }

    public override void Hit(float multiplier = 1)
    {
        base.Hit(multiplier);

    }

    protected override void PhaseTwo()
    {
        StartCoroutine(Big());
    }
    IEnumerator Big()
    {
        float bigTime = 2;
        Vector3 defaultSize = transform.localScale;
        while (bigTime > 0)
        {
            bigTime -= Time.deltaTime;
            transform.localScale = defaultSize * (1 + (1 - bigTime / 2));
            yield return null;
        }
    }
}
