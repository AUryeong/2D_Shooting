using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Singleton<Player>
{
    SpriteRenderer spriteRenderer;
    Color defaultColor;

    public float moveSpeed;

    private int level = 1;
    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            if (value > level)
                Hp += value - level;
            level = value;
            IngameManager.Instance.uiManager.LevelChange(level);
        }
    }
    private int exp = 0;
    public int Exp
    {
        get
        {
            return exp;
        }
        set
        {
            if (level >= 10)
            {
                return;
            }
            if (value >= maxExp)
            {
                value -= maxExp;
                Level++;
                MaxExp = GetMaxNextExp(MaxExp);
            }
            exp = value;
            IngameManager.Instance.uiManager.ExpChange(exp);
        }
    }

    public int defaultMaxExp = 100;
    private int maxExp;
    public int MaxExp
    {
        get
        {
            return maxExp;
        }
        set
        {
            if (level >= 10)
            {
                return;
            }
            maxExp = value;
            IngameManager.Instance.uiManager.MaxExpChange(maxExp);
        }
    }

    public int defaulthp = 3;
    private int hp;
    public int Hp
    {
        get
        {
            return hp;
        }
        set
        {
            hp = Mathf.Clamp(value, 0, 3);
            IngameManager.Instance.uiManager.LifeChange(hp);
        }
    }

    bool hitable;

    [Header("총알 관련")]
    [SerializeField] Bullet playerBullet;
    [SerializeField] BulletMankai playerBulletMankai;

    int upgradeLevel = 0;

    [Space(20f)]
    public float defaultPower = 1;
    public float power;
    public float Fuel
    {
        set
        {
            fuel = value;
            IngameManager.Instance.uiManager.FuelChange(value / 100f);
        }
        get
        {
            return fuel;
        }
    }
    private float fuel;

    public float defaultAttackSpeed = 0.1f;
    public float attackSpeed;

    public int defaultMultiplier = 1;
    public int multiplier;

    public float Skill1Cool = 10;
    public float defaultSkill1Cool = 10;
    public float Skill2Cool = 10;
    public float defaultSkill2Cool = 10;

    public float invTime = 1;
    bool inv;

    private float attackDuration;

    [Header("특수 능력")]
    private readonly List<SkillType> skillTypes = new List<SkillType>();

    public int GetMaxNextExp(int nowMaxExp)
    {
        return (int)Mathf.Pow(nowMaxExp, 1.13f);
    }

    protected override void OnCreated()
    {
        base.OnCreated();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;

        Level = 1;
        MaxExp = defaultMaxExp;
        Exp = 0;

        Fuel = 100;
        Hp = defaulthp;
        power = defaultPower;
        multiplier = defaultMultiplier;
        attackSpeed = defaultAttackSpeed;
        hitable = true;


        Skill1Cool = 0;
        IngameManager.Instance.uiManager.CooltimeChanage(Skill1Cool);
        Skill2Cool = 0;
        IngameManager.Instance.uiManager.Cooltime2Chanage(Skill2Cool);
    }

    private void Start()
    {
        AddSkill((SkillType)Random.Range(0, 4));
    }

    private void Cheat()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (inv)
                gameObject.layer = LayerMask.NameToLayer("Player");
            else
                gameObject.layer = LayerMask.NameToLayer("PlayerInv");
            inv = !inv;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            foreach (Enemy enemy in FindObjectsOfType<Enemy>())
            {
                enemy.Die();
            }
            foreach (Bullet bullet in FindObjectsOfType<Bullet>())
            {
                bullet.gameObject.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            IngameManager.Instance.CreateSideSideBoss();
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            IngameManager.Instance.CreateSideBoss();
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            IngameManager.Instance.CreateSecondBoss();
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            IngameManager.Instance.GameOver();
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            SceneManager.LoadScene("Title");
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            AddSkill(SkillType.Red);
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            AddSkill(SkillType.Green);
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            AddSkill(SkillType.Blue);
        }
        if (Input.GetKeyDown(KeyCode.F11))
        {
            AddSkill(SkillType.Yellow);
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            Level++;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (Skill1Cool <= 0)
            {
                Skill1Cool = defaultSkill1Cool;
                Hp++;
            }
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (Skill2Cool <= 0)
            {
                Skill2Cool = defaultSkill2Cool;
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    if (enemy is Boss) continue;
                    enemy.Die();
                }
                foreach (Bullet bullet in FindObjectsOfType<Bullet>())
                {
                    bullet.gameObject.SetActive(false);
                }
            }
        }
        if (Skill1Cool > 0)
        {
            Skill1Cool -= Time.deltaTime;
            IngameManager.Instance.uiManager.CooltimeChanage(Skill1Cool / defaultSkill1Cool);
        }
        if (Skill2Cool > 0)
        {
            Skill2Cool -= Time.deltaTime;
            IngameManager.Instance.uiManager.Cooltime2Chanage(Skill2Cool / defaultSkill2Cool);
            Debug.Log(Skill2Cool);
        }
    }

    private void Update()
    {
        if (IngameManager.Instance.uiManager.intro) return;
        Cheat();
        if (hp <= 0) return;
        ShootUpdate();
        Move();
        SkillKey();
        FuelUpdate();
    }
    private void FuelUpdate()
    {
        Fuel -= Time.deltaTime;
        if (Fuel <= 0)
        {
            IngameManager.Instance.GameOver();
        }
    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x == 0 && y == 0) return;
        float dag = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, dag - 90);
        transform.position = IngameManager.Instance.OutlineCheck(transform.position + (new Vector3(x, y) * moveSpeed * Time.deltaTime));
    }

    private void ShootUpdate()
    {
        if (Input.GetKey(KeyCode.X))
        {
            bool click = Input.GetKeyDown(KeyCode.X);
            attackDuration += Time.deltaTime;
            if (attackDuration >= attackSpeed || click)
            {
                if (!click)
                    attackDuration -= attackSpeed;

                Vector3 left = transform.position + new Vector3(0.2f * (multiplier - 1), 0, 0);

                for (int i = 0; i < multiplier; i++)
                {
                    GameObject obj = PoolManager.Instance.Init(playerBullet.gameObject);
                    obj.transform.position = left + new Vector3(-0.4f * i, 0, 0);
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag(nameof(Enemy)))
        {
            Hit();
        }
    }

    public void Hit()
    {
        if (!hitable || inv) return;

        Hp--;
        if (Hp <= 0)
            IngameManager.Instance.GameOver();
        else
        {
            IngameManager.Instance.uiManager.HitEffect();
            StartCoroutine(HitInv());
        }
    }

    public void AddSkill(SkillType type)
    {
        if (skillTypes.Count < level)
        {
            skillTypes.Add(type);
            IngameManager.Instance.uiManager.SkillSetting(skillTypes);
            switch (type)
            {
                case SkillType.Red:
                    if (upgradeLevel < 4)
                    {
                        upgradeLevel++;
                        multiplier++;
                    }
                    power++;
                    break;
                case SkillType.Yellow:
                    multiplier++;
                    break;
                case SkillType.Green:
                    Hp++;
                    invTime += 2;
                    break;
                case SkillType.Blue:
                    attackSpeed /= 1.5f;
                    Fuel = 100;
                    break;
            }
        }
        else
        {
            UseSkil(type);
        }
    }

    private void SkillKey()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (skillTypes.Count > 0)
            {
                SkillType type = skillTypes[skillTypes.Count - 1];
                skillTypes.RemoveAt(skillTypes.Count - 1);
                IngameManager.Instance.uiManager.SkillSetting(skillTypes);
                switch (type)
                {
                    case SkillType.Red:
                        power--;
                        break;
                    case SkillType.Yellow:
                        multiplier--;
                        break;
                    case SkillType.Green:
                        invTime -= 2;
                        break;
                    case SkillType.Blue:
                        attackSpeed *= 1.5f;
                        break;
                }
                UseSkil(type);
            }
        }
    }

    public void UseSkil(SkillType type)
    {
        switch (type)
        {
            case SkillType.Red:
                GameObject obj2 = PoolManager.Instance.Init(playerBulletMankai.gameObject);
                obj2.transform.position = transform.position;
                if (upgradeLevel < 4)
                {
                    upgradeLevel++;
                    multiplier++;
                }
                break;
            case SkillType.Yellow:
                StartCoroutine(YellowMankai());
                break;
            case SkillType.Green:
                Hp++;
                Hp++;
                break;
            case SkillType.Blue:
                StartCoroutine(Gatling());
                Fuel = 100;
                break;
        }
    }
    IEnumerator Gatling()
    {
        var wait = new WaitForSeconds(attackSpeed);
        for (int i = 0; i < 30; i++)
        {
            GameObject obj = PoolManager.Instance.Init(playerBullet.gameObject);
            obj.transform.position = transform.position;
            yield return wait;
        }
    }
    IEnumerator YellowMankai()
    {
        var wait = new WaitForSeconds(attackSpeed);
        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < 12; i++)
            {
                GameObject obj = PoolManager.Instance.Init(playerBullet.gameObject);
                obj.transform.position = transform.position;
                obj.transform.rotation = Quaternion.Euler(0, 0, i * 30);
            }
            yield return wait;
            for (int i = 0; i < 12; i++)
            {
                GameObject obj = PoolManager.Instance.Init(playerBullet.gameObject);
                obj.transform.position = transform.position;
                obj.transform.rotation = Quaternion.Euler(0, 0, i * 30 + 15);
            }
            yield return wait;
        }
    }

    IEnumerator HitInv()
    {
        gameObject.layer = LayerMask.NameToLayer("PlayerInv");
        spriteRenderer.color = new Color(1, 1, 1, 1f);
        hitable = false;
        IngameManager.Instance.CameraShake(0.3f, 0.2f);

        yield return new WaitForSeconds(0.2f);
        float recoverTime = invTime;
        while (recoverTime > 0)
        {
            recoverTime -= Time.deltaTime;
            spriteRenderer.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0.3f + (invTime - recoverTime) * 0.7f);
            yield return null;
        }

        gameObject.layer = LayerMask.NameToLayer("Player");
        spriteRenderer.color = defaultColor;
        hitable = true;
    }
}
