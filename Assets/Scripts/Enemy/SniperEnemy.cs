using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperEnemy : Enemy
{
    [SerializeField] SpriteRenderer warning;
    [SerializeField] Bullet bullet;
    [SerializeField] float shootCooltime;
    float shootDuartion;

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
        if(shootDuartion >= shootCooltime)
        {
            shootDuartion -= shootCooltime;
            warning.transform.position = transform.position;
            warning.gameObject.SetActive(true);

            Vector3 vector = Player.Instance.transform.position - transform.position;
            float deg = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, deg+90);

            warning.transform.Translate(Vector2.down * 10);
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(1.5f);
        GameObject bulletObj = PoolManager.Instance.Init(bullet.gameObject);
        bulletObj.transform.position = transform.position;
        bulletObj.transform.rotation = transform.rotation;
        warning.gameObject.SetActive(false);
    }
}
