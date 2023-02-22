using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameManager : Singleton<IngameManager>
{
    public IngameUIManager uiManager;
    protected override void OnCreated()
    {
        base.OnCreated();
    }

    public void GameOver()
    {

    }
}
