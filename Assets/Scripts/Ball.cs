using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody2D rb;
    Collider2D col;
    float splitMass;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        splitMass = rb.mass * 50f;
    }

    private void OnEnable()
    {
        rb.mass = 1f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GravityController.Instance.ActiveBalls.Count < ObjectPool.Instance.MaxBalls)
        {
            var otherBall = collision.gameObject.GetComponent<Ball>();

            if (otherBall.transform.localScale.x > transform.localScale.x)
            {
                Collide(otherBall, this);
            }
            else
            {
                Collide(this, otherBall);
            }
        }
    }


    void Collide(Ball a, Ball b)
    {
        a.transform.localScale = GetNewScale(a.transform, b.transform);
        a.rb.mass += b.rb.mass;

        ObjectPool.Instance.ReturnToPool(b.transform);

        if (a.rb.mass > a.splitMass)
        {
            var amount = Mathf.CeilToInt(a.rb.mass);
            var position = a.transform.position;
            ObjectPool.Instance.ReturnToPool(a.transform);
            Split(amount, position);
        }
    }

    Vector2 GetNewScale(Transform a, Transform b)
    {
        float totalArea = Mathf.PI * Mathf.Pow(a.localScale.x / 2f, 2f) + Mathf.PI * Mathf.Pow(b.localScale.x / 2f, 2f);

        return Mathf.Sqrt(totalArea / Mathf.PI) * 2f * Vector2.one;
    }

    void Split(int amount, Vector3 newSpawnPos)
    {

        for (int i = 0; i < amount; i++)
        {
            var tempBall = ObjectPool.Instance.GetFromPool().GetComponent<Ball>();
            tempBall.rb.bodyType = RigidbodyType2D.Kinematic;
            tempBall.transform.SetParent(null);
            tempBall.transform.position = newSpawnPos;
            tempBall.col.isTrigger = true;
            tempBall.gameObject.SetActive(true);
            Vector2 randomVector = Random.insideUnitCircle;
            randomVector *= 50f;
            tempBall.rb.velocity = randomVector;
            tempBall.Invoke(nameof(EnableCollider), 0.5f);
        }
    }

    void EnableCollider()
    {
        col.isTrigger = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
