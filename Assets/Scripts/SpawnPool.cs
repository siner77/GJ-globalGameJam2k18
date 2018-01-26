using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPool<T> where T : MonoBehaviour
{
    private List<T> _pool;
    private T _original;

    public SpawnPool(T original, int initialCount)
    {
        _original = original;
        _pool = new List<T>();
        for(int i = 0; i < initialCount; ++i)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        T newObject = GameObject.Instantiate<T>(_original);
        newObject.gameObject.SetActive(false);
        _pool.Add(newObject);
    }

    public T Get()
    {
        for(int i = 0; i < _pool.Count; ++i)
        {
            if (!_pool[i].gameObject.activeInHierarchy)
            {
                _pool[i].gameObject.SetActive(true);
                return _pool[i];
            }
        }

        Spawn();
        _pool[_pool.Count - 1].gameObject.SetActive(true);
        return _pool[_pool.Count - 1];
    }
}
