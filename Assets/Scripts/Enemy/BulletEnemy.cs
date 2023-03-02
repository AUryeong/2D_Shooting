using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : Enemy
{
    [SerializeField] Bullet bullet;
    [SerializeField] float shootCooltime;
    private float shootDuration;

    protected override void Update()
    {
        base.Update();
        ShootUpdate();
    }

    private void ShootUpdate()
    {
        shootDuration += Time.deltaTime;
        if (shootDuration >= shootCooltime)
        {
            shootDuration -= shootCooltime;
            for (int i = 0; i < 3; i++)
            {
                GameObject bulletObj = PoolManager.Instance.Init(bullet.gameObject);
                bulletObj.transform.position = transform.position;
                bulletObj.transform.rotation = Quaternion.Euler(0, 0, -30 + i * 30);
            }
        }
    }
}
