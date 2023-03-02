using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicEnemy : Enemy
{
    [SerializeField] Bullet bullet;
    [SerializeField] float shootCooltime;
    float shootDuartion;
    float removeDuration = 0;

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
        shootDuartion += Time.deltaTime;
        if (shootDuartion >= shootCooltime)
        {
            shootDuartion -= shootCooltime;
            Vector3 vector = Player.Instance.transform.position - transform.position;
            float deg = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, deg + 90);

            for (int i = 0; i < 6; i++)
            {
                GameObject obj = PoolManager.Instance.Init(bullet.gameObject);
                obj.transform.rotation = Quaternion.Euler(0, 0, i * 20 + 30 + deg);
                obj.transform.position = transform.position;
            }
        }
        removeDuration += Time.deltaTime;
        if(removeDuration >= 10)
        {
            Die();
        }
    }
}
