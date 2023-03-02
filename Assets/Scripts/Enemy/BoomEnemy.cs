using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BoomEnemy : Enemy
{
    [SerializeField] Bullet bullet;
    public override void Die()
    {
        base.Die();
        for (int i = 0; i < 18; i++)
        {
            GameObject obj = PoolManager.Instance.Init(bullet.gameObject);
            obj.transform.rotation = Quaternion.Euler(0, 0, i * 20);
            obj.transform.position = transform.position;
        }
    }
}
