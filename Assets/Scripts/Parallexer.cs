using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallexer : MonoBehaviour
{
    class PoolObject
    {
        public Transform transform;
        public bool inUse;
        public PoolObject(Transform t) { transform = t; }
        public void Use() { inUse = true; }
        public void Dispose() { inUse = false; }
    }

    [System.Serializable]
    public struct YSpawnRange
    {
        public float min;
        public float max;
    }

    public GameObject Prefab;
    public int poolSize;
    public float shiftSpeed;
    public float spawnRate;

    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPosition;
    public bool spawnImmediate;
    public Vector3 ImmediateSpawnPosition;
    public Vector2 targetAspectRatio;

    float spawnTimer;
    float targetAspect;
    PoolObject[] poolObjects;
    GameManager game;

    void Awake()
    {
        Configure();
    }

    void Start()
    {
        game = GameManager.instance;
    }

    void OnEnable()
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameOverConfirmed()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
        }

        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

    void Update()
    {
        if (game.gameOver)
        {
            return;
        }

        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate)
        {
            Spawn();
            spawnTimer = 0;
        }
    }

    void Configure()
    {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for (int i = 0; i < poolObjects.Length; i++)
        {
            GameObject go = Instantiate(Prefab) as GameObject;
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            poolObjects[i] = new PoolObject(t);
        }

        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

    void Spawn()
    {
        Transform t = GetPoolObject();
        if (t == null)
        {
            return;
        }

        Vector3 position = Vector3.zero;
        position.x = defaultSpawnPosition.x;
        position.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = position;
    }

    void SpawnImmediate()
    {
        Transform t = GetPoolObject();
        if (t == null)
        {
            return;
        }

        Vector3 position = Vector3.zero;
        position.x = ImmediateSpawnPosition.x;
        position.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = position;
        Spawn();
    }

    void Shift()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].transform.position += -Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObjects[i]);
        }
    }

    void CheckDisposeObject(PoolObject poolObject)
    {
        if (poolObject.transform.position.x < -defaultSpawnPosition.x)
        {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * 1000;
        }
    }

    Transform GetPoolObject()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            if (!poolObjects[i].inUse)
            {
                poolObjects[i].Use();
                return poolObjects[i].transform;
            }
        }

        return null;
    }
}
