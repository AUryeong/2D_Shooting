using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillItem : MonoBehaviour
{
    public SkillType type;
    SpriteRenderer spriteRenderer;
    [SerializeField] float moveSpeed;
    float speed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        type = (SkillType)Random.Range(0, 4);

        spriteRenderer.color = IngameUIManager.GetSkillColor(type);
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
            Player.Instance.AddSkill(type);
            PoolManager.Instance.Init(IngameManager.Instance.expEffect.gameObject).transform.position = transform.position;
            gameObject.SetActive(false);
        }
    }
}
