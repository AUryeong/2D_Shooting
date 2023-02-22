using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<GameObject, List<GameObject>> pools = new Dictionary<GameObject, List<GameObject>>();

    public GameObject Init(GameObject baseObj)
    {
        if (pools.ContainsKey(baseObj))
        {
            GameObject disableObj = pools[baseObj].Find(x => !x.gameObject.activeSelf);
            if(disableObj != null)
            {
                disableObj.gameObject.SetActive(true);
                return disableObj;
            }
        }
        else
        {
            pools.Add(baseObj, new List<GameObject>());
        }

        GameObject initObj = Instantiate(baseObj);
        initObj.SetActive(true);
        pools[baseObj].Add(initObj);

        return initObj;
    }
}
