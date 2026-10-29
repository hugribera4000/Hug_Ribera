using UnityEngine;
using System.Collections.Generic;

public class PipeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pipePrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float minimumHeight = -1.25f;
    [SerializeField] private float maximumHeight = 1.25f;

    private readonly List<Pipe> spawnedPipes = new List<Pipe>();
    private float spawnTimer;

    private void Awake()
    {
        if (pipePrefab == null)
        {
            Debug.LogError("PipeSpawner needs a pipe prefab assigned in the Inspector.", this);
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying || pipePrefab == null)
        {
            return;
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnPipe();
        }
    }

    public void ResetPipes()
    {
        foreach (Pipe pipe in spawnedPipes)
        {
            if (pipe != null)
            {
                Destroy(pipe.gameObject);
            }
        }

        spawnedPipes.Clear();
        spawnTimer = 0f;
    }

    private void SpawnPipe()
    {
        float height = Random.Range(minimumHeight, maximumHeight);
        GameObject pipeObject = Instantiate(
            pipePrefab,
            new Vector3(transform.position.x, height, transform.position.z),
            Quaternion.identity);

        Pipe pipe = pipeObject.GetComponent<Pipe>();
        if (pipe == null)
        {
            Destroy(pipeObject);
            Debug.LogError("The assigned pipe prefab needs a Pipe component.", this);
            return;
        }

        spawnedPipes.RemoveAll(existingPipe => existingPipe == null);
        spawnedPipes.Add(pipe);
    }
}
