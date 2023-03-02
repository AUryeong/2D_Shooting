using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [Header("시스템")]
    [SerializeField] GameObject howToPlayWindow;
    [SerializeField] GameObject rankingWindow;
    [SerializeField] Text rankingText;

    [Header("연출")]
    [SerializeField] Text[] fadeInTexts;
    [SerializeField] Image[] fadeInImages;
    public void GameStart()
    {
        StartCoroutine(FadeInCoroutine());
    }
    IEnumerator FadeInCoroutine()
    {
        float time = 1;
        while (time > 0)
        {
            time -= Time.deltaTime;
            foreach (var text in fadeInTexts)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, time);
            }
            foreach (var image in fadeInImages)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, time);
            }
            yield return null;
        }
        SceneManager.LoadScene("Ingame");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            if (rankingWindow.gameObject.activeSelf)
                Ranking();
            else
                RankingOff();
        }
    }

    public void HowToPlay()
    {
        howToPlayWindow.gameObject.SetActive(true);
    }
    public void HowToPlayOff()
    {
        howToPlayWindow.gameObject.SetActive(false);
    }

    public void Ranking()
    {
        rankingWindow.gameObject.SetActive(true);
        List<PlayerData> data = GameManager.Instance.saveData.playerdatas;
        data = data.OrderByDescending((PlayerData x) => x.score).ToList();
        var sb = new StringBuilder();
        for (int i = 0; i < Mathf.Min(data.Count, 5); i++)
        {
            sb.AppendLine($"{i + 1}) {data[i].name} : {data[i].score}\n");
        }
        rankingText.text = sb.ToString();
    }
    public void RankingOff()
    {
        rankingWindow.gameObject.SetActive(false);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
