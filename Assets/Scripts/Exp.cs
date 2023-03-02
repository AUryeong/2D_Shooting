using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : MonoBehaviour
{
    public int exp;
    [SerializeField] float moveSpeed;
    float speed;

    private void OnEnable()
    {
        speed = moveSpeed * Random.Range(0.8f, 1.2f);
    }

    private void Update()
    {
        MoveAndRotate();
    }

    void MoveAndRotate()
    {
        transform.position += new Vector3(0, Time.deltaTime * -speed);
        transform.eulerAngles += new Vector3(0, 0, Time.deltaTime * 60 * speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.Exp += exp;
            PoolManager.Instance.Init(IngameManager.Instance.expEffect.gameObject).transform.position = transform.position;
            gameObject.SetActive(false);
        }
    }
}
