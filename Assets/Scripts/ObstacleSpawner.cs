using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    public Transform spawnPoint;
    public float spawnIntervalMin = 0.8f;
    public float spawnIntervalMax = 1.6f;
    public float baseSpeed = 6f;
    public float speedIncreasePerSecond = 0.15f;

    private float timer;
    private float nextSpawnTime;
    private float elapsed;
    public bool isSpawning = false;

    void Update()
    {
        if (!isSpawning) return;

        elapsed += Time.deltaTime;
        timer += Time.deltaTime;

        if (timer >= nextSpawnTime)
        {
            timer = 0f;
            nextSpawnTime = Random.Range(spawnIntervalMin, spawnIntervalMax);
            SpawnObstacle();
        }
    }

    private void SpawnObstacle()
    {
        var prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        Vector3 spawnPos = new Vector3(spawnPoint.position.x, prefab.transform.position.y, 0f);
        var obj = Instantiate(prefab, spawnPos, Quaternion.identity);
        var obstacle = obj.GetComponent<Obstacle>();
        obstacle.speed = baseSpeed + (elapsed * speedIncreasePerSecond);
    }

    public void StartSpawning()
    {
        elapsed = 0f;
        timer = 0f;
        nextSpawnTime = Random.Range(spawnIntervalMin, spawnIntervalMax);
        isSpawning = true;
    }

    public void StopSpawning()
    {
        isSpawning = false;
        foreach (var obstacle in FindObjectsOfType<Obstacle>())
        {
            Destroy(obstacle.gameObject);
        }
    }
}