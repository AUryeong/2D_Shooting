using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperEnemy : Enemy
{
    [SerializeField] SpriteRenderer warning;
    [SerializeField] Bullet bullet;
    [SerializeField] float shootCooltime;
    float shootDuartion;
    float removeDuration;

    protected override void OnEnable()
    {
        base.OnEnable();
        warning.gameObject.SetActive(false);
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
        removeDuration += Time.deltaTime;
        if (removeDuration >= 10)
        {
            Die();
        }
    }

    IEnumerator Shoot()
    {
        var wait = new WaitForSeconds(0.05f);
        for(int i = 0; i < 15; i++)
        {
            warning.color = new Color(warning.color.r, warning.color.g, warning.color.b, 0.45f);
            yield return wait;
            warning.color = new Color(warning.color.r, warning.color.g, warning.color.b, 0.15f);
            yield return wait;
        }
        GameObject bulletObj = PoolManager.Instance.Init(bullet.gameObject);
        bulletObj.transform.position = transform.position;
        bulletObj.transform.rotation = transform.rotation;
        warning.gameObject.SetActive(false);
    }
}
