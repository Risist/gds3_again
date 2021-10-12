using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/*
 * Basic object pool sensitive to Activation state
 * 
 * Sends events
 *  - OnSpawn : when object returned to pool
 *  - OnDespawn : when object is out of pool
 */
public class ObjectPool : MonoBehaviour
{
    class ObjectPoolItemMarker : MonoBehaviour
    {
        public ObjectPool pool;
        void OnDisable()
        {
            pool.ReturnToPool(gameObject);
        }

    }

    public GameObject prefab;
    public int initialPoolSize;


    List<GameObject> _freeObjectList = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < initialPoolSize; ++i)
        {
            ReturnToPool(CreateNewElement());
        }
    }


    GameObject CreateNewElement()
    {
        var obj = Instantiate(prefab, transform);
        var marker = obj.AddComponent<ObjectPoolItemMarker>();
        marker.pool = this;
        return obj;
    }
    public GameObject SpawnObject(Action<GameObject> initialize)
    {
        GameObject obj = null;
        if(_freeObjectList.Count != 0)
        {
            obj = _freeObjectList[_freeObjectList.Count - 1];
            _freeObjectList.RemoveAt(_freeObjectList.Count - 1);
        }else
        {
            obj = CreateNewElement();
        }
        obj.SetActive(true);

        initialize(obj);
        obj.SendMessage("OnSpawn", SendMessageOptions.DontRequireReceiver);
        
        return obj;
    }
    public void ReturnToPool(GameObject obj)
    {
        _freeObjectList.Add(obj);
        obj.SendMessage("OnDespawn", SendMessageOptions.DontRequireReceiver);
        obj.SetActive(false);
    }
}

