using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    protected bool intro;
    protected bool isPatterning = false;
    protected Vector3 center = new Vector3(-2.1f, 2, 0);
    protected bool phaseTwo = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        IngameManager.Instance.uiManager.bossBar.gameObject.SetActive(true);
        IngameManager.Instance.uiManager.boosBarGauge.fillAmount = 1;
        IngameManager.Instance.isBoss = true;
        intro = true;
        phaseTwo = false;
        StartCoroutine(BossPattern());
    }
    protected virtual IEnumerator BossPattern()
    {
        while (intro) yield return null;
        yield return new WaitForSeconds(2.5f);
        isPatterning = true;
    }


    protected override void Update()
    {
        if (intro)
            MoveIntro();
        else
            DragonBall();
    }

    protected virtual IEnumerator Return()
    {
        float moveTime = 1;
        Vector3 pos = transform.position;
        Vector3 vector = center - pos;
        float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle + 90);
        while (moveTime > 0)
        {
            moveTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(center, pos, moveTime);
            yield return null;
        }

        transform.position = center;
        transform.rotation = Quaternion.identity;
        isPatterning = false;
    }
    protected void DragonBall()
    {
        if (isPatterning) return;
        time += Time.deltaTime;
        transform.Translate(new Vector3(0, Mathf.Sin(time * 3) * Time.deltaTime * 0.3f, 0), Space.World);
    }


    protected void MoveIntro()
    {
        transform.Translate(new Vector3(0, -Time.deltaTime * moveSpeed, 0), Space.World);
        if (transform.position.y <= 2f)
        {
            intro = false;
        }
    }
    public override void Hit(float multiplier = 1)
    {
        base.Hit(multiplier);
        IngameManager.Instance.uiManager.boosBarGauge.fillAmount = hp / defaultHp;
        if (!phaseTwo && hp <= defaultHp * 0.5f)
        {
            PhaseTwo();
            phaseTwo = true;
        }
    }

    protected virtual void PhaseTwo()
    {
    }

    public override void Die()
    {
        IngameManager.Instance.Score += 30000;
        IngameManager.Instance.uiManager.bossBar.gameObject.SetActive(false);
        IngameManager.Instance.isBoss = false;
        Player.Instance.Exp += Player.Instance.MaxExp;
        PoolManager.Instance.Init(IngameManager.Instance.bossDeathEffect.gameObject).transform.position = transform.position ;
        Destroy(gameObject);
    }
}
