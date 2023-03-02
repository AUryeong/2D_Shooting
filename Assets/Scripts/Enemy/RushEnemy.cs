using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushEnemy : Enemy
{
    [SerializeField] SpriteRenderer warning;

    [SerializeField] float dashSpeed;
    [SerializeField] float shootCooltime;
    float shootDuartion;
    float removeDuration;
    bool isDashing = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        warning.gameObject.SetActive(false);
        removeDuration = 0;
        isDashing = false;
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
        if (isDashing) return;

        shootDuartion += Time.deltaTime;
        if (shootDuartion >= shootCooltime)
        {
            shootDuartion -= shootCooltime;
            StartCoroutine(Skill());
        }
        removeDuration += Time.deltaTime;
        if (removeDuration >= 10)
        {
            Die();
        }
    }


    IEnumerator Skill()
    {
        warning.transform.position = transform.position;
        warning.gameObject.SetActive(true);
        isDashing = true;

        Vector3 vector = Player.Instance.transform.position - transform.position;
        float deg = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, deg + 90);

        warning.transform.Translate(Vector2.down * 6);
        var wait = new WaitForSeconds(0.2f);
        for (int i = 0; i < 5; i++)
        {
            warning.color = new Color(warning.color.r, warning.color.g, warning.color.b, 0.45f);
            yield return wait;
            warning.color = new Color(warning.color.r, warning.color.g, warning.color.b, 0.15f);
            yield return wait;
        }
        warning.gameObject.SetActive(false);
        while (!IngameManager.Instance.OutlineBound(transform.position))
        {
            transform.Translate(0, -Time.deltaTime * dashSpeed, 0);
            yield return null;
        }
        transform.position = IngameManager.Instance.OutlineCheck(transform.position);
        isDashing = false;
    }
}
