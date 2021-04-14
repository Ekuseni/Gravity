using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallSpawner : MonoBehaviour
{

    private static BallSpawner _instance;

    public static BallSpawner Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BallSpawner>();
            }

            return _instance;
        }
    }

    [SerializeField]
    Text ballCount;

    float minX;
    float maxX;
    float minY;
    float maxY;

    void Start()
    {
        minX = transform.position.x - Camera.main.orthographicSize * Camera.main.aspect;
        maxX = transform.position.x + Camera.main.orthographicSize * Camera.main.aspect;
        minY = transform.position.y - Camera.main.orthographicSize;
        maxY = transform.position.y + Camera.main.orthographicSize;
        
        InvokeRepeating(nameof(SpawnBall), 0.25f, 0.25f);
    }

    private void Update()
    {
        ballCount.text = GravityController.Instance.ActiveBalls.Count.ToString();
    }

    void SpawnBall()
    {
        if (GravityController.Instance.ActiveBalls.Count >= ObjectPool.Instance.MaxBalls) return;

        var ball = ObjectPool.Instance.GetFromPool();
        ball.SetParent(null);

        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);

        ball.position = new Vector2(x, y);

        ball.gameObject.SetActive(true);
       
        
       
    }
}
