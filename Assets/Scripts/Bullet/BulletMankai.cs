using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMankai : Bullet
{
    protected override void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (team == Team.Player)
        {
            if (collider2D.CompareTag(nameof(Enemy)))
            {
                Enemy enemy = collider2D.GetComponent<Enemy>();
                if (enemy == null)
                    enemy = collider2D.transform.parent.GetComponent<Enemy>();
                enemy.Hit(3);
            }
        }
    }
}
