using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

[Serializable]
public struct EnemyShipSpawnInfo
{
    public int Priority;
    public ShipController Prefab;
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
        if(_spawnCooldownRange.x > _spawnCooldownRange.y)
        {
            Utility.Swap(ref _spawnCooldownRange.x, ref _spawnCooldownRange.y);
        }
    }

    private void OnEnable()
    {
        _cooldown = Random.Range(_spawnCooldownRange.x, _spawnCooldownRange.y);
        _timer = 0.0f;
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

    }
}
