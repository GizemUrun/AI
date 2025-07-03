using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    public GameObject boidPrefab;
    public int boidCount = 20;
    public Vector3 spawnArea = new Vector3(20, 20, 20);

    void Start()
    {
        for (int i = 0; i < boidCount; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                Random.Range(-spawnArea.y / 2, spawnArea.y / 2),
                Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
            );
            GameObject boid = Instantiate(boidPrefab, position, Quaternion.identity);
            boid.name = "Boid " + i;
        }
    }
}
