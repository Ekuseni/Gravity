using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private static ObjectPool _instance;

    public static ObjectPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ObjectPool>();
            }

            return _instance;
        }
    }


    
    public int MaxBalls = 250;

    private int poolSize = 1000;

    [SerializeField]
    GameObject ballPrefab;



    Queue<Transform> pooledObjects = new Queue<Transform>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var go = Instantiate(ballPrefab, transform);
            go.name = i.ToString();
            pooledObjects.Enqueue(go.transform);
            go.SetActive(false);
        }
    }

    public Transform GetFromPool()
    {
        if(pooledObjects.Count > 0)
        {
            var pooledObject = pooledObjects.Dequeue();

            GravityController.Instance.ActiveBalls.Add(pooledObject.GetComponent<Ball>());
            return pooledObject;
        }

        
        GravityController.Instance.ActiveBalls.Add(Instantiate(ballPrefab, null).GetComponent<Ball>());
        return GravityController.Instance.ActiveBalls[GravityController.Instance.ActiveBalls.Count - 1].transform;

    }

    public void ReturnToPool(Transform transform)
    {
        if (GravityController.Instance != null)
        {
            GravityController.Instance.ActiveBalls.Remove(transform.GetComponent<Ball>());
        }

        transform.gameObject.SetActive(false);
        transform.localScale = Vector3.one;
        transform.SetParent(this.transform);
        pooledObjects.Enqueue(transform);
    }
}
