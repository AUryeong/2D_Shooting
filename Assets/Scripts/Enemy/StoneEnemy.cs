using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneEnemy : Enemy
{
    [SerializeField] Bullet bullet;
    [SerializeField] float shootCooltime;
    private float shootDuration;

    protected override void OnEnable()
    {
        base.OnEnable();
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
    }

    protected override void Update()
    {
        base.Update();
        ShootUpdate();
        transform.Rotate(new Vector3(0, 0, Time.deltaTime * 80));
    }

    private void ShootUpdate()
    {
        shootDuration += Time.deltaTime;
        if (shootDuration >= shootCooltime)
        {
            shootDuration -= shootCooltime;
            GameObject bulletObj = PoolManager.Instance.Init(bullet.gameObject);
            bulletObj.transform.position = transform.position;
            bulletObj.transform.rotation = transform.rotation;
        }
    }
}
