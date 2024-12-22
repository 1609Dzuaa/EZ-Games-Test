using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

[System.Serializable]
public struct PoolableObject
{
    public EPoolable PrefabName;
    public GameObject Prefab;
    public int Amount;
}

public class Pool : BaseSingleton<Pool>
{
    [SerializeField] List<PoolableObject> _listPoolableObj = new();
    private Dictionary<EPoolable, List<GameObject>> _dictPool = new();

    protected override void Awake()
    {
        base.Awake();
        FillDictionary();
        InstantiateGameObjects();
    }

    private void FillDictionary()
    {
        for (int i = 0; i < _listPoolableObj.Count; i++)
            if (!_dictPool.ContainsKey(_listPoolableObj[i].PrefabName))
                _dictPool.Add(_listPoolableObj[i].PrefabName, new());
    }

    private void InstantiateGameObjects()
    {
        for (int i = 0; i < _listPoolableObj.Count; i++)
        {
            for (int j = 0; j < _listPoolableObj[i].Amount; j++)
            {
                GameObject gObj = Instantiate(_listPoolableObj[i].Prefab);
                gObj.SetActive(false);
                _dictPool[_listPoolableObj[i].PrefabName].Add(gObj);
            }
        }
    }

    public GameObject GetObjectInPool(EPoolable gObj)
    {
        for (int i = 0; i < _dictPool[gObj].Count; i++)
            if (!_dictPool[gObj][i].activeInHierarchy)
                return _dictPool[gObj][i];

        GameObject gO = Instantiate(_dictPool[gObj][0], transform);
        _dictPool[gObj].Add(gO);
        Debug.Log("out of " + gObj + " create new one!");
        return gO;
    }

}