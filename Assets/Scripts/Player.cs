using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{

    public int level = 1;
    public int exp = 0;

    public int defaultMaxExp = 100;
    public int maxExp;

    public int defaulthp = 3;
    private int hp;

    [Header("ÃÑ¾Ë °ü·Ã")]
    public float defaultPower = 1;
    public float power;

    public float defaultAttackSpeed = 0.1f;
    public float attackSpeed;

    public int defaultMultiplier = 1;
    public int multiplier;

    [Space(20f)]
    private float attackDuration; 

    public int GetMaxNextExp(int nowMaxExp)
    {
        return (int)Mathf.Pow(nowMaxExp, 1.1f);
    }

    protected override void OnCreated()
    {
        base.OnCreated();
        level = 1;
        exp = 0;

        hp = defaulthp;
        power = defaultPower;
        multiplier = defaultMultiplier;
        attackSpeed = defaultAttackSpeed;
    }

    private void Update()
    {
        ShootUpdate();
    }

    private void ShootUpdate()
    {
        attackDuration += Time.deltaTime;
        if(attackDuration >= attackSpeed)
        {
            attackDuration -= attackSpeed;
            // È÷È÷ ÃÑ¾Ë ¹ß»ç
        }
    }

    public void Hit()
    {

    }
}
