using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{

    private static ObjectPool instance;
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();

    //对象池节点
    private GameObject pool = null;

    //单例
    public static ObjectPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ObjectPool();
            }
            return instance;
        }
    }

    public void init()
    {
        objectPool.Clear();
    }

    public int getCount()
    {
        if(objectPool != null)
        {
            return objectPool.Count;
        } else
        {
            return 0;
        }
    }

    //创建
    public GameObject Get(GameObject prefab, Vector3 pos, Vector3 scale, bool disableScale = false)
    {
        GameObject obj;
        if (!objectPool.ContainsKey(prefab.name) || objectPool[prefab.name].Count == 0)
        {
            obj = GameObject.Instantiate(prefab);
            Push(obj);
            if (pool == null)
            {
                pool = new GameObject("ObjectPool");
            }
            //给每个对象都创建父节点对象池，用来收纳，因为Find需要遍历，所以为了节省性能，最好指定路径
            GameObject childPool = GameObject.Find("ObjectPool/" + prefab.name + "Pool");
            if (!childPool)
            {
                childPool = new GameObject(prefab.name + "Pool");
                childPool.transform.SetParent(pool.transform);
            }
            obj.transform.SetParent(childPool.transform);
        }
        obj = objectPool[prefab.name].Dequeue();
        obj.transform.position = pos;
        if(!disableScale)
        {
            obj.transform.localScale = scale;
        }
        obj.SetActive(true);
        return obj;
    }

    //销毁 （实际是回收）
    public void Push(GameObject prefab)
    {
        string name = prefab.name.Replace("(Clone)", string.Empty);
        if (!objectPool.ContainsKey(name))
        {
            objectPool.Add(name, new Queue<GameObject>());
        }
        objectPool[name].Enqueue(prefab);
        prefab.SetActive(false);
    }

}
