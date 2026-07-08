using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    //All pools for different object types can be stored in this dictionary
    private Dictionary<GameObject, IObjectPool<GameObject>> poolDictionary = new Dictionary<GameObject, IObjectPool<GameObject>>();

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject bulletPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Start()
    {
        CreatePool(enemyPrefab, 10);
        CreatePool(bulletPrefab, 20);
    }

    //Pool initialization
    public void CreatePool(GameObject prefab, int initialSize = 10)
    {
        poolDictionary[prefab] = new ObjectPool<GameObject>(() =>
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            return obj;
        }, obj =>
        {
            obj.SetActive(true);
        }, obj =>
        {
            obj.SetActive(false);
        }, obj =>
        {
            Destroy(obj);
        }, false, initialSize, initialSize * 2);
    }

    public GameObject GetObjectFromPool(GameObject prefab)
    {
        if (poolDictionary[prefab] == null)
        {
            Debug.LogError(prefab.name + " pool has not been created. Call CreatePool first.");
            return null;
        }
        return poolDictionary[prefab].Get();
    }

    public void ReturnObjectToPool(GameObject prefab, GameObject instance)
    {
        if (poolDictionary.ContainsKey(prefab))
        {
            poolDictionary[prefab].Release(instance);
        }
    }

    public GameObject GetEnemyPrefab()
    {
        return enemyPrefab;
    }

    public GameObject GetProjectilePrefab()
    {
        return bulletPrefab;
    }
}
