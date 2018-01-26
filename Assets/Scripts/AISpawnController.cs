using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

[Serializable]
public struct EnemyShipSpawnInfo
{
    public float Chance;
    public ShipController Prefab;

    private SpawnPool<ShipController> _shipsPool;

    public void Init()
    {
        _shipsPool = new SpawnPool<ShipController>(Prefab, 10);
    }

    public ShipController Spawn()
    {
        return _shipsPool.Get();
    }
}

public class AISpawnController : MonoBehaviour
{
    [SerializeField]
    private List<EnemyShipSpawnInfo> _enemyShips;

    private Vector2 _spawnCooldownRange;
    private float _cooldown;
    private float _timer;

    private void OnValidate()
    {
        if (_spawnCooldownRange.x > _spawnCooldownRange.y)
        {
            Utility.Swap(ref _spawnCooldownRange.x, ref _spawnCooldownRange.y);
        }
    }

    private void OnEnable()
    {
        _cooldown = Random.Range(_spawnCooldownRange.x, _spawnCooldownRange.y);
        _timer = 0.0f;

        _enemyShips.Sort((EnemyShipSpawnInfo e1, EnemyShipSpawnInfo e2) =>
        {
            return e1.Chance.CompareTo(e2.Chance);
        });
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if(_timer >= _cooldown)
        {
            SpawnEnemyShip();
        }
    }

    private void SpawnEnemyShip()
    {
        float randomValue = Random.Range(0.0f, 1.0f);
        for(int i = 0; i < _enemyShips.Count; ++i)
        {
            if (randomValue <= _enemyShips[i].Chance)
            {
                ShipController spawnedEnemy = _enemyShips[i].Spawn();
                // TODO:
                // Choose planet and set one of its satelites as target
                spawnedEnemy.SetState(new ShipStates.GoToSatelite(null));
                return;
            }
        }
    }
}
