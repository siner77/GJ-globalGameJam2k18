using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

[Serializable]
public class EnemyShipSpawnInfo
{
    public float Chance;
    public EnemyShipController Prefab;

    private SpawnPool<EnemyShipController> _shipsPool;

    public void Init()
    {
        _shipsPool = new SpawnPool<EnemyShipController>(Prefab, 10);
    }

    public EnemyShipController Spawn()
    {
        return _shipsPool.Get();
    }
}

public class AISpawnController : MonoBehaviour
{
    private enum SpawnArea
    {
        LEFT        = 0x0001,
        RIGHT       = 0x0010,
        TOP         = 0x0100,
        BOTTOM      = 0x1000,

        TOP_LEFT = LEFT | TOP,
        TOP_RIGHT = RIGHT | TOP,
        BOTTOM_LEFT = LEFT | BOTTOM,
        BOTTOM_RIGHT = RIGHT | BOTTOM
    }

    [SerializeField]
    private List<EnemyShipSpawnInfo> _enemyShips;
    [SerializeField]
    private Vector2 _spawnCooldownRange;

    private LevelManager _levelManager;

    private float _cooldown;
    private float _timer;

    private void OnEnable()
    {
        if (_spawnCooldownRange.x > _spawnCooldownRange.y)
        {
            Utility.Swap(ref _spawnCooldownRange.x, ref _spawnCooldownRange.y);
        }

        _levelManager = FindObjectOfType<LevelManager>();
        _cooldown = Random.Range(_spawnCooldownRange.x, _spawnCooldownRange.y);
        _timer = 0.0f;

        _enemyShips.Sort((EnemyShipSpawnInfo e1, EnemyShipSpawnInfo e2) =>
        {
            return e1.Chance.CompareTo(e2.Chance);
        });

        foreach(EnemyShipSpawnInfo enemyShipSpawnInfo in _enemyShips)
        {
            enemyShipSpawnInfo.Init();
        }
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if(_timer >= _cooldown)
        {
            SpawnEnemyShip();
            _timer = 0.0f;
        }
    }

    private void SpawnEnemyShip()
    {
        Planet planetToAttack = _levelManager.GetPlanetToAttack();
        if(planetToAttack == null)
        {
            return;
        }

        Vector3 planetScreenPosition = Camera.main.WorldToScreenPoint(planetToAttack.transform.position);
        Vector2 xSpawnRange = new Vector2(0.0f, Screen.width);
        Vector2 ySpawnRange = new Vector2(0.0f, Screen.height);
        SpawnArea spawnArea = SpawnArea.BOTTOM;

        if (planetScreenPosition.x > Screen.width * 0.5f)
        {
            xSpawnRange.x = Screen.width * 0.5f;
            spawnArea = SpawnArea.RIGHT;
        }
        else
        {
            xSpawnRange.y = Screen.width * 0.5f;
            spawnArea = SpawnArea.LEFT;
        }

        if (planetScreenPosition.y > Screen.height * 0.5f)
        {
            ySpawnRange.x = Screen.height * 0.5f;
            spawnArea |= SpawnArea.BOTTOM;
        }
        else
        {
            ySpawnRange.y = Screen.height * 0.5f;
            spawnArea |= SpawnArea.TOP;
        }

        Vector3 spawnScreenPosition = new Vector3(Random.Range(xSpawnRange.x, xSpawnRange.y), Random.Range(ySpawnRange.x, ySpawnRange.y), Camera.main.transform.position.y);

        Rect rectangle = new Rect(xSpawnRange.x, ySpawnRange.x, xSpawnRange.y - xSpawnRange.x, ySpawnRange.y - ySpawnRange.x);

        switch (spawnArea)
        {
            case SpawnArea.TOP_LEFT:
                {
                    Vector2 diagonal = new Vector2(xSpawnRange.y - xSpawnRange.x, ySpawnRange.y - ySpawnRange.x);
                    if (IsPointAboveDiagonal(rectangle, diagonal, spawnScreenPosition))
                    {
                        spawnScreenPosition.y = -20.0f;
                    }
                    else
                    {
                        spawnScreenPosition.x = -20.0f;
                    }
                }
                break;
            case SpawnArea.TOP_RIGHT:
                {
                    Vector2 diagonal = new Vector2(xSpawnRange.y - xSpawnRange.x, ySpawnRange.x - ySpawnRange.y);
                    if (IsPointAboveDiagonal(rectangle, diagonal, spawnScreenPosition))
                    {
                        spawnScreenPosition.y = -20.0f;
                    }
                    else
                    {
                        spawnScreenPosition.x = Screen.width + 20.0f;
                    }
                }
                break;
            case SpawnArea.BOTTOM_LEFT:
                {
                    Vector2 diagonal = new Vector2(xSpawnRange.y - xSpawnRange.x, ySpawnRange.y - ySpawnRange.x);
                    if (IsPointAboveDiagonal(rectangle, diagonal, spawnScreenPosition))
                    {
                        spawnScreenPosition.y = Screen.height + 20.0f;
                    }
                    else
                    {
                        spawnScreenPosition.x = -20.0f;
                    }
                }
                break;
            case SpawnArea.BOTTOM_RIGHT:
                {
                    Vector2 diagonal = new Vector2(xSpawnRange.y - xSpawnRange.x, ySpawnRange.x - ySpawnRange.y);
                    if (IsPointAboveDiagonal(rectangle, diagonal, spawnScreenPosition))
                    {
                        spawnScreenPosition.y = Screen.height + 20.0f;
                    }
                    else
                    {
                        spawnScreenPosition.x = Screen.width + 20.0f;
                    }
                }
                break;
            default:
                Debug.LogError("This should not happen");
                return;
        }
        
        Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(spawnScreenPosition);
        spawnPosition.y = 0.0f;

        float randomValue = Random.Range(0.0f, 1.0f);
        for(int i = 0; i < _enemyShips.Count; ++i)
        {
            if (randomValue <= _enemyShips[i].Chance)
            {
                EnemyShipController spawnedEnemy = _enemyShips[i].Spawn();
                spawnedEnemy.transform.position = spawnPosition;
                spawnedEnemy.SetPlanetToAttack(planetToAttack);
                spawnedEnemy.SetState(new ShipStates.GoToPlanet(planetToAttack, new ShipStates.AttackPlanet(planetToAttack)));
                return;
            }
        }
    }

    private bool IsPointAboveDiagonal(Rect rectangle, Vector2 diagonal, Vector2 point)
    {
        // https://stackoverflow.com/questions/20245104/point-inside-a-rectangle-on-which-side-of-the-diagonal-is-it
        Vector2 toPoint = point - rectangle.position;

        float diagonalSlope = diagonal.y / diagonal.x;
        float toPointSlope = toPoint.y / toPoint.x;

        return diagonalSlope > toPointSlope;
    }
}
