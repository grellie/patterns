using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPoolExample : MonoBehaviour 
{
    [SerializeField] GameObject[] prefabs;
    [SerializeField] int prewarmNumber = 100;
    [SerializeField] int numberOfObjects = 1000;
    [SerializeField] Vector2 spawnArea = Vector3.one;

    private List<GameObject> createdList = new List<GameObject>();

    IEnumerator Start()
    {
        foreach (var prefab in prefabs)
            ObjectPool.PrewarmPrefab(prefab, prewarmNumber);
        
        while(true)
        {
            CreateRandom();
            if(createdList.Count > numberOfObjects)
                DeleteRandom();
            yield return new WaitForEndOfFrame();
        }    
    }

    private void CreateRandom()
    {
        int num = UnityEngine.Random.Range(0, prefabs.Length);
        Vector3 position =
            spawnArea.x * Vector3.Lerp(Vector3.up, Vector3.down, UnityEngine.Random.value) +
            spawnArea.y * Vector3.Lerp(Vector3.left, Vector3.right, UnityEngine.Random.value);

        GameObject created = ObjectPool.Instantiate(prefabs[num], position, Quaternion.identity);
        createdList.Add(created);
    }

    private void DeleteRandom()
    {
        int toDelete = UnityEngine.Random.Range(0, createdList.Count);
        ObjectPool.Destroy(createdList[toDelete]);
        createdList.RemoveAt(toDelete);
    }
}
