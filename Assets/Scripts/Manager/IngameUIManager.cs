using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameUIManager : MonoBehaviour
{
    [SerializeField] Image fadeOutBlack;
    [SerializeField] Text scoreText;
    [SerializeField] Text maxScoreText;

    [SerializeField] List<Image> lifeImage;
    [SerializeField] Text levelText;

    [SerializeField] Text expText;
    [SerializeField] Text expSlashText;
    [SerializeField] Text maxExpText;
    [SerializeField] Image expGauge;

    [Header("게임오버")]
    [SerializeField] GameObject gameOverWindow;
    [SerializeField] Text gameOverScoreText;
    [SerializeField] InputField gameOverInput;
    [SerializeField] Text gameOverRankingText;
    [SerializeField] Text gameOverText;

    [Header("스킬")]
    [SerializeField] Image lastSkill;
    [SerializeField] List<Image> skillList;

    [Header("보스")]
    public Image bossBar;
    public Image boosBarGauge;

    [Header("연출")]
    [SerializeField] RectTransform rightUI;
    [SerializeField] RectTransform leftUI;
    [SerializeField] Image hitEffect;

    public bool intro;

    private void Start()
    {
        StartCoroutine(FadeOutCoroutine());
        hitEffect.color = new Color(1, 0, 0, 0);
    }
    IEnumerator FadeOutCoroutine()
    {
        intro = true;
        float animationTime = 3;
        Player.Instance.transform.position = new Vector3(-2.1f, -5.5f);
        rightUI.anchoredPosition = new Vector2(400, 0);
        leftUI.anchoredPosition = new Vector2(-175f, 0);

        float time = animationTime;
        while (time > 0)
        {
            time -= Time.deltaTime;
            float multiplier = (time / animationTime);

            Player.Instance.transform.position = new Vector3(-2.1f, Mathf.Lerp(0, -5.5f, multiplier));
            rightUI.anchoredPosition = new Vector2(Mathf.Lerp(-300f, 400f, multiplier), 0);
            leftUI.anchoredPosition = new Vector2(Mathf.Lerp(75f, -175f, multiplier), 0);
            if (Input.anyKeyDown)
                time = 0;
            yield return null;
        }
        Player.Instance.transform.position = new Vector3(-2.1f, 0);
        rightUI.anchoredPosition = new Vector2(-300, 0);
        leftUI.anchoredPosition = new Vector2(75, 0);
        intro = false;
    }
    public void HitEffect()
    {
        StartCoroutine(HitEffectCoroutine());
    }
    IEnumerator HitEffectCoroutine()
    {
        float fadeInDuration = 0.2f;

        while (fadeInDuration > 0)
        {
            fadeInDuration -= Time.deltaTime;
            hitEffect.color = new Color(1, 0, 0, 1 - (fadeInDuration / 0.2f));
            yield return null;
        }
        hitEffect.color = new Color(1, 0, 0, 1);
        float fadeOutDuration = 0.4f;

        while (fadeOutDuration > 0)
        {
            fadeOutDuration -= Time.deltaTime;
            hitEffect.color = new Color(1, 0, 0, fadeOutDuration);
            yield return null;
        }
        hitEffect.color = new Color(1, 0, 0, 0);
    }

    public void ScoreChange(int score)
    {
        scoreText.text = score.ToString();
    }
    public void MaxScoreChange(int score)
    {
        maxScoreText.text = score.ToString();
    }

    public void GameOver()
    {
        gameOverWindow.gameObject.SetActive(true);
        gameOverText.text = "YOU DIED";
        gameOverText.color = Color.red;
        gameOverScoreText.text = "당신의 점수 : " + IngameManager.Instance.Score;
        List<PlayerData> data = GameManager.Instance.saveData.playerdatas;
        data = data.OrderByDescending((PlayerData x) => x.score).ToList();
        var sb = new StringBuilder();
        for (int i = 0; i < Mathf.Min(data.Count, 5); i++)
        {
            sb.AppendLine($"{i + 1}) {data[i].name} : {data[i].score}\n");
        }
        gameOverRankingText.text = sb.ToString();
    }

    public void Win()
    {
        gameOverWindow.gameObject.SetActive(true);
        gameOverText.text = "당신은 해냈습니다!";
        gameOverText.color = Color.green;
        gameOverScoreText.text = "당신의 점수 : " + IngameManager.Instance.Score;
        List<PlayerData> data = GameManager.Instance.saveData.playerdatas;
        data = data.OrderByDescending((PlayerData x) => x.score).ToList();
        var sb = new StringBuilder();
        for (int i = 0; i < Mathf.Min(data.Count, 5); i++)
        {
            sb.AppendLine($"{i + 1}) {data[i].name} : {data[i].score}\n");
        }
        gameOverRankingText.text = sb.ToString();
    }

    public void Submit()
    {
        if (string.IsNullOrEmpty(gameOverInput.text))
            return;

        GameManager.Instance.saveData.playerdatas.Add(new PlayerData()
        {
            name = gameOverInput.text,
            score = IngameManager.Instance.Score
        });
        Time.timeScale = 1;

        SceneManager.LoadScene("Title");
    }

    public void LifeChange(int life)
    {
        for (int i = 0; i < 3; i++)
            lifeImage[i].gameObject.SetActive(i < life);
    }

    public void LevelChange(int level)
    {
        if (level >= 10)
        {
            levelText.text = "Level MAX";
            expGauge.fillAmount = 1;
            expText.text = "";
            maxExpText.text = "";
            expSlashText.text = "";
            return;
        }
        levelText.text = "Level " + level;
    }

    public void ExpChange(int exp)
    {
        expText.text = exp.ToString();
        expGauge.fillAmount = (float)exp / Player.Instance.MaxExp;

    }
    public void MaxExpChange(int maxExp)
    {
        maxExpText.text = maxExp.ToString();
    }

    public void SkillSetting(List<SkillType> skillTypes)
    {
        if (skillTypes == null)
        {
            lastSkill.gameObject.SetActive(false);
            for (int i = 0; i < skillList.Count; i++)
                skillList[i].gameObject.SetActive(false);
            return;
        }
        if (skillTypes.Count > 0)
        {
            lastSkill.gameObject.SetActive(true);
            lastSkill.color = GetSkillColor(skillTypes[skillTypes.Count - 1]);
        }
        else
        {
            lastSkill.gameObject.SetActive(false);
        }

        for (int i = skillList.Count - 1; i >= 0; i--)
        {
            if (skillTypes.Count > i)
            {
                skillList[i].rectTransform.GetChild(0).GetComponent<Image>().color = GetSkillColor(skillTypes[i]);
                skillList[i].gameObject.SetActive(true);
            }
            else
            {
                skillList[i].gameObject.SetActive(false);
            }
        }
    }

    public static Color GetSkillColor(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.Yellow:
                return new Color(1, 1, 0);
            case SkillType.Green:
                return new Color(0, 1, 0);
            case SkillType.Blue:
                return new Color(0, 0, 1);
            default:
                return new Color(1, 0, 0);
        }
    }
}