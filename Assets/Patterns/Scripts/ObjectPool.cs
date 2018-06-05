using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple object Pool
/// </summary>
public class ObjectPool
{
    //Gameobject in scene to hold inactive objects
    private static GameObject group;
    static ObjectPool()
    {
        group = new GameObject("ObjectPool");
    }

    //Dictionaries to hold all inactive / active objects
    private static Dictionary<int, Queue<GameObject>> inactiveDict = new Dictionary<int, Queue<GameObject>>();
    private static Dictionary<GameObject, int> activeDict = new Dictionary<GameObject, int>();

    private static GameObject InstantiateBase(GameObject prefab, Transform parent)
    {
        GameObject created;
        int id = prefab.GetInstanceID();
        if (!inactiveDict.ContainsKey(id))
        {
            inactiveDict.Add(id, new Queue<GameObject>());
        }

        if (inactiveDict[id].Count > 0)
        {
            created = inactiveDict[id].Dequeue();
        }
        else 
        {
            created = GameObject.Instantiate(prefab);
            created.name = prefab.name + created.GetInstanceID();
        }

        created.transform.SetParent(parent);

        activeDict.Add(created, id);

        return created;
    }

    private static void CopyTransform(GameObject from, GameObject to)
    {
        to.transform.SetPositionAndRotation(from.transform.position, from.transform.rotation);
    }

    //Use to create more objects in advance - inactive objects
    public static void PrewarmPrefab(GameObject prefab, int count)
    {
        int id = prefab.GetInstanceID();

        if (!inactiveDict.ContainsKey(id))
        {
            inactiveDict.Add(id, new Queue<GameObject>());
        }

        GameObject created;
        for (int i = 0; i < count; ++i)
        {
            created = GameObject.Instantiate(prefab, group.transform);
            created.SetActive(false);
            created.name = prefab.name + created.GetInstanceID();
        }
    }

    public static GameObject Instantiate(GameObject prefab)
    {
        GameObject created = InstantiateBase(prefab, null);

        CopyTransform(prefab, created);
            
        created.SetActive(true);

        return created;
    }

    public static GameObject Instantiate(GameObject prefab, Transform parent)
    {
        GameObject created = InstantiateBase(prefab, parent);

        CopyTransform(prefab, created);

        created.SetActive(true);

        return created;
    }

    public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject created = InstantiateBase(prefab, null);

        created.transform.SetPositionAndRotation(position, rotation);

        created.SetActive(true);

        return created;
    }

    public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject created = InstantiateBase(prefab, parent);

        created.transform.SetPositionAndRotation(position, rotation);

        created.SetActive(true);

        return created;
    }


    public static void Destroy(GameObject created)
    {
        if (activeDict.ContainsKey(created))
        {
            created.SetActive(false);
            created.transform.SetParent(group.transform);

            int prefabId = activeDict[created];
            activeDict.Remove(created);
            inactiveDict[prefabId].Enqueue(created);

        }
        else
            Debug.LogWarningFormat("Object {0} is not exist", created.name);
    }
}
