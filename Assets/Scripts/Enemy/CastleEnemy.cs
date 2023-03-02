using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleEnemy : Enemy
{
    float removeDuration;
    [SerializeField] Bullet bullet;

    protected override void OnEnable()
    {
        base.OnEnable();
        removeDuration = 0;
    }

    protected override void Update()
    {
        if (transform.position.y > 3f)
            base.Update();
        else
            ShootUpdate();
    }

    private void ShootUpdate()
    {
        removeDuration += Time.deltaTime;
        if (removeDuration >= 8)
        {
            Die();
            for (int i = 0; i < 36; i++)
            {
                GameObject obj = PoolManager.Instance.Init(bullet.gameObject);
                obj.transform.rotation = Quaternion.Euler(0, 0, i * 10);
                obj.transform.position = transform.position;
            }
        }
    }
}
