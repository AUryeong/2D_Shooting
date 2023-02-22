using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameManager : Singleton<IngameManager>
{
    public IngameUIManager uiManager;
    [SerializeField] Vector2 minPos;
    [SerializeField] Vector2 maxPos;

    
    
    public void CameraShake(float power, float duration)
    {
        StartCoroutine(CameraShakeCoroutine(power, duration));
    }

    IEnumerator CameraShakeCoroutine(float power, float duration)
    {
        Vector3 center = new Vector3(0, 0, -10);
        while(duration > 0)
        {
            duration -= Time.deltaTime;
            Camera.main.transform.position = center + Random.onUnitSphere * power;
            yield return null;
        }
    }

    public Vector3 OutlineCheck(Vector3 vector)
    {
        return new Vector3(Mathf.Clamp(vector.x, minPos.x, maxPos.x), Mathf.Clamp(vector.y, minPos.y, maxPos.y), vector.z);
    }

    public bool OutlineBound(Vector3 vector)
    {
        return vector.x < minPos.x || vector.x > maxPos.x || vector.y < minPos.y || vector.y > maxPos.y;
    }
    protected override void OnCreated()
    {
        base.OnCreated();
    }

    public void GameOver()
    {

    }
}
