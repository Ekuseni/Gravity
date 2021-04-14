using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class GravityController : MonoBehaviour
{
    private static GravityController _instance;

    public static GravityController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GravityController>();
            }

            return _instance;
        }
    }

    public List<Ball> ActiveBalls = new List<Ball>();

    const float G = 6.67408f;

    void FixedUpdate()
    {
        var positions = new NativeArray<Vector2>(ActiveBalls.Count, Allocator.Persistent);
        var forces = new NativeArray<Vector2>(ActiveBalls.Count, Allocator.Persistent);
        var masses = new NativeArray<float>(ActiveBalls.Count, Allocator.Persistent);


        for(int i = 0; i < positions.Length; i++)
        {
            positions[i] = ActiveBalls[i].rb.position;
            masses[i] = ActiveBalls[i].rb.mass;
        }

        var job = new AttractJob()
        {
            positions = positions,
            forces = forces,
            masses = masses,
        };

        JobHandle jobHandle = job.Schedule(positions.Length, 32);

        jobHandle.Complete();

        for (int i = 0; i < positions.Length; i++)
        {
            if (ActiveBalls.Count >= ObjectPool.Instance.MaxBalls)
            {
                ActiveBalls[i].rb.AddForce(forces[i]);
            }
            else
            {
                ActiveBalls[i].rb.AddForce(-forces[i]);
            }
            
        }

        positions.Dispose();
        forces.Dispose();
        masses.Dispose();
    }

    struct AttractJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Vector2> positions;
        public NativeArray<Vector2> forces;
        [ReadOnly]
        public NativeArray<float> masses;

        public void Execute(int i)
        {
            for (int j = 0; j < positions.Length; j++)
            {
                if (j == i)
                {
                    continue;
                }

                Vector2 direction = positions[i] - positions[j];
                float distance = direction.magnitude;

                if (distance == 0f)
                    continue;

                float forceMagnitude = G * (masses[i] * masses[j]) / Mathf.Pow(distance, 2);
                forces[i] += direction.normalized * forceMagnitude;
            }
        }
    }
}
