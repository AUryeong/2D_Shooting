using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameManager : Singleton<IngameManager>
{
    public IngameUIManager uiManager;

    private int score;
    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            if (score > maxScore)
                MaxScore = score;
            uiManager.ScoreChange(score);
        }
    }
    private int maxScore;
    public int MaxScore
    {
        get
        {
            return maxScore;
        }
        set
        {
            maxScore = value;
            GameManager.Instance.maxScore = maxScore;
            uiManager.MaxScoreChange(maxScore);
        }
    }

    public ParticleSystem dieEffect;
    public ParticleSystem expEffect;
    public ParticleSystem bossDeathEffect;
    public Exp exp;
    public SkillItem skillItem;

    public bool isBoss;

    public bool clearSideBoss;
    public bool clearSecondBoss;
    public bool clearSideSideBoss;
    float spawnDuration = 0;
    float spawnCooltime = 7f;

    [SerializeField] Vector2 minPos;
    [SerializeField] Vector2 maxPos;
    [SerializeField] Text nextStageText;

    [SerializeField] Enemy[] mobs;
    [SerializeField] SideBoss sideBoss;
    [SerializeField] SideSideBoss sideSideBoss;
    [SerializeField] TwinBoss secondBoss;

    private float scoreAddDuration = 0;

    public void NextStage()
    {
        StartCoroutine(NextStageCoroutine());
        IEnumerator NextStageCoroutine()
        {
            nextStageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2);
            nextStageText.gameObject.SetActive(false);
        }
    }
    public void CameraShake(float power, float duration)
    {
        StartCoroutine(CameraShakeCoroutine(power, duration));
    }

    IEnumerator CameraShakeCoroutine(float power, float duration)
    {
        Vector3 center = new Vector3(0, 0, -10);
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            Camera.main.transform.position = center + Random.onUnitSphere * power;
            yield return null;
        }
        Camera.main.transform.position = center;
    }

    public Vector3 OutlineCheck(Vector3 vector)
    {
        return new Vector3(Mathf.Clamp(vector.x, minPos.x, maxPos.x), Mathf.Clamp(vector.y, minPos.y, maxPos.y), vector.z);
    }

    public bool OutlineBound(Vector3 vector)
    {
        return vector.x < minPos.x || vector.x > maxPos.x || vector.y < minPos.y || vector.y > maxPos.y;
    }

    public bool OutlineBoundForEnemy(Vector3 vector)
    {
        return vector.x < minPos.x || vector.x > maxPos.x || vector.y < minPos.y || vector.y > maxPos.y+2;
    }

    protected override void OnCreated()
    {
        base.OnCreated();
        Score = 0;
        MaxScore = GameManager.Instance.maxScore;
        uiManager.SkillSetting(null);
        spawnDuration = 3;
    }

    private void Update()
    {
        if (uiManager.intro) return;
        ScoreUpdate();
        if (!isBoss)
            MobCreate();
    }
    void FirstSpawnMob()
    {
        float spawnY = 6;
        float spawnRandomMinX = -1f;
        float spawnRandomMaxX = 1;

        int pattern = Random.Range(0, (clearSideBoss) ? 5 : 3);
        switch (pattern)
        {
            case 0:
                for (int i = 0; i < 6; i++)
                {
                    PoolManager.Instance.Init(mobs[0].gameObject).transform.position =
                        new Vector2(-7.4f + 1.8f * i + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                }
                break;
            case 1:
                for (int i = 0; i < 3; i++)
                {
                    PoolManager.Instance.Init(mobs[3].gameObject).transform.position =
                        new Vector2(-6.1f + 4 * i + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                }
                break;
            case 4:
                for (int i = 0; i < 3; i++)
                {
                    PoolManager.Instance.Init(mobs[6].gameObject).transform.position =
                        new Vector2(-6.1f + 4 * i + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                }
                break;
            case 5:
                for (int i = 0; i < 3; i++)
                {
                    PoolManager.Instance.Init(mobs[5].gameObject).transform.position =
                        new Vector2(-6.1f + 4 * i + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                }
                break;
        }
        SpawnMob();
    }

    void SpawnMob()
    {
        float spawnY = 6;
        float spawnRandomMinX = -1f;
        float spawnRandomMaxX = 1;

        int pattern = Random.Range(0, (clearSideBoss) ? 7 : 4);
        switch (pattern)
        {
            case 0:
                for (int i = 0; i < 3; i++)
                {
                    PoolManager.Instance.Init(mobs[1].gameObject).transform.position = 
                        new Vector2(-5.1f + 3 * i + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                }
                break;
            case 1:
                PoolManager.Instance.Init(mobs[2].gameObject).transform.position = 
                    new Vector2(-6.1f + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                PoolManager.Instance.Init(mobs[2].gameObject).transform.position = 
                    new Vector2(1.9f + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                break;
            case 2:
                PoolManager.Instance.Init(mobs[4].gameObject).transform.position = new 
                    Vector2(-6.1f + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                PoolManager.Instance.Init(mobs[4].gameObject).transform.position = new 
                    Vector2(1.9f + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                break;
            case 3:
                PoolManager.Instance.Init(mobs[0].gameObject).transform.position = 
                    new Vector2(-5.1f + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                PoolManager.Instance.Init(mobs[1].gameObject).transform.position = 
                    new Vector2(-2.1f + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                PoolManager.Instance.Init(mobs[0].gameObject).transform.position = 
                    new Vector2(0.9f + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                PoolManager.Instance.Init(mobs[2].gameObject).transform.position = 
                    new Vector2(-6.1f + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                PoolManager.Instance.Init(mobs[4].gameObject).transform.position = 
                    new Vector2(1.9f + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                break;
            case 4:
                PoolManager.Instance.Init(mobs[6].gameObject).transform.position =
                    new Vector2(-5.1f + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                PoolManager.Instance.Init(mobs[7].gameObject).transform.position =
                    new Vector2(-2.1f + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                PoolManager.Instance.Init(mobs[6].gameObject).transform.position =
                    new Vector2(0.9f + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                PoolManager.Instance.Init(mobs[5].gameObject).transform.position =
                    new Vector2(-6.1f + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                PoolManager.Instance.Init(mobs[5].gameObject).transform.position =
                    new Vector2(1.9f + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                break;
            case 5:
                for (int i = 0; i < 3; i++)
                {
                    PoolManager.Instance.Init(mobs[7].gameObject).transform.position =
                        new Vector2(-5.1f + 3f * i + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                }
                break;
            case 6:
                for (int i = 0; i < 4; i++)
                {
                    PoolManager.Instance.Init(mobs[5].gameObject).transform.position =
                        new Vector2(-5.1f + 2f * i + Random.Range(spawnRandomMinX, -spawnRandomMaxX), spawnY);
                }
                break;
        }
    }

    private void MobCreate()
    {
        spawnDuration -= Time.deltaTime;
        if (spawnDuration <= 0)
        {
            if (clearSecondBoss)
            {
                Win();
                return;
            }
            spawnDuration += Mathf.Max(1, Random.Range(0.9f, 1.1f) * spawnCooltime - Player.Instance.Level);
            FirstSpawnMob();
        }
    }

    private void ScoreUpdate()
    {
        scoreAddDuration += Time.deltaTime;
        if (scoreAddDuration >= 1)
        {
            scoreAddDuration--;
            Score += Random.Range(2, 8);

            if (!isBoss)
            {
                if (!clearSideSideBoss && score >= 7000)
                {
                    CreateSideSideBoss();
                }
                if (!clearSideBoss && score >= 25000)
                {
                    CreateSideBoss();
                }
                if (!clearSecondBoss && score >= 70000)
                {
                    CreateSecondBoss();
                }
            }
        }
    }

    public void CreateSideSideBoss()
    {
        clearSideSideBoss = true;
        PoolManager.Instance.Init(sideSideBoss.gameObject).transform.position = new Vector3(-2.1f, 7, 0);

        CameraShake(1, 2);
    }

    public void CreateSecondBoss()
    {
        clearSideBoss = true;
        clearSideSideBoss = true;
        clearSecondBoss = true;
        PoolManager.Instance.Init(secondBoss.gameObject).transform.position = new Vector3(-2.1f, 7, 0);

        CameraShake(1, 2);
    }

    public void CreateSideBoss()
    {
        clearSideBoss = true;
        clearSideSideBoss = true;
        PoolManager.Instance.Init(sideBoss.gameObject).transform.position = new Vector3(-2.1f, 7, 0);

        CameraShake(1, 2);
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        uiManager.GameOver();
    }

    public void Win()
    {
        Time.timeScale = 0;
        uiManager.Win();
    }
}
