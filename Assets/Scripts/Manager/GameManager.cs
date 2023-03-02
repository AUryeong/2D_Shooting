using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string name;
    public int score;
}
[System.Serializable]
public class SaveData
{
    public int maxScore;
    public List<PlayerData> playerdatas = new List<PlayerData>()
    {
        new PlayerData(){
            name="¾Ù¸®½º", 
            score=10000
        },
        new PlayerData(){
            name="±èÅ×½ºÆ®¾¾",
            score=5000
        },
        new PlayerData(){
            name="±×¸²",
            score=57000
        }
    };
}
public class GameManager : Singleton<GameManager>
{
    public int maxScore;
    private SaveData _saveData;
    public SaveData saveData
    {
        get
        {
            if (_saveData == null)
                LoadSaveData();
            return _saveData;
        }
    }
    void LoadSaveData()
    {
        string s = PlayerPrefs.GetString("save", "null");
        if (s == "null" || string.IsNullOrEmpty(s))
        {
            _saveData = new SaveData();
        }
        else
        {
            _saveData = JsonUtility.FromJson<SaveData>(s);
        }
    }
    private void OnApplicationQuit()
    {
        saveData.maxScore = maxScore;
        PlayerPrefs.SetString("save", JsonUtility.ToJson(saveData));
    }

    protected override void OnCreated()
    {
        base.OnCreated();
        DontDestroyOnLoad(gameObject);
        maxScore = saveData.maxScore;
    }
}
