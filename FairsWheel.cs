using UnityEngine;
using System.Collections.Generic;

public class FairsWheel : MonoBehaviour
{
    public GameObject platformdurrr;

    public int platformAmount = 8;
    public float radius = 5f;
    public float rotationSpeed = 30f;

    private GameObject[] platforms;

    void Start()
    {
        platforms = new GameObject[platformAmount];

        for (int i = 0; i < platformAmount; i++)
        {
            float angle = (360f / platformAmount) * i;
            float radians = angle * Mathf.Deg2Rad;

            float x = Mathf.Cos(radians) * radius;
            float y = Mathf.Sin(radians) * radius;

            Vector3 position = transform.position + new Vector3(x, y, 0);

            platforms[i] = Instantiate(
                platformdurrr,
                position,
                Quaternion.identity
            );
        }
    }

    void Update()
    {
        // Move each platform around the center
        for (int i = 0; i < platformAmount; i++)
        {
            float angle = (360f / platformAmount) * i;

            // Add time-based rotation
            angle += Time.time * rotationSpeed;

            float radians = angle * Mathf.Deg2Rad;

            float x = Mathf.Cos(radians) * radius;
            float y = Mathf.Sin(radians) * radius;

            platforms[i].transform.position =
                transform.position + new Vector3(x, y, 0);

            // Keep the platform upright
            platforms[i].transform.rotation = Quaternion.identity;
        }
    }
}


