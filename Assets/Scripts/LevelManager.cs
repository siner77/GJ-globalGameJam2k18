using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

class LevelManager : MonoBehaviour
{
    [SerializeField]
    private float _progressModifier = 1.0f;
    [SerializeField]
    private float _gameTimeLimit = 300.0f;

    private float _progress = 0.0f;
    private float _progressLimit = 100.0f;
    private float _gameTime = 0.0f;

    private List<Planet> _levelPlanets;
    private List<Planet> _planetCandidatesToAttack;

    private void Awake()
    {
        _levelPlanets = FindObjectsOfType<Planet>().ToList();
        foreach(Planet planet in _levelPlanets)
        {
            planet.OnLooseAllSatellites += OnLooseAllSatellites;
        }
        _planetCandidatesToAttack = new List<Planet>();
    }

    void Update()
    {
        _gameTime += Time.deltaTime;
        if (_gameTime > _gameTimeLimit)
        {
            Debug.Log("game over");
            Debug.Break();
        }
    }

    public void ImproveProgress()
    {
        _progress += Time.deltaTime * _progressModifier;
        Debug.Log(string.Format("Progress = {0}", _progress));
        if (_progress > _progressLimit)
        {
            Debug.Log("win, gz");
            Debug.Break();
        }
    }

    public Planet GetPlanetToAttack()
    {
        _planetCandidatesToAttack.Clear();
        int minAttackingShips = int.MaxValue;

        foreach (Planet planet in _levelPlanets)
        {
            int attackingShipsCount = planet.GetAttackingShipsCount();
            if (attackingShipsCount <= minAttackingShips && planet.HasUnusedEnemySpot())
            {
                if(attackingShipsCount < minAttackingShips)
                {
                    _planetCandidatesToAttack.Clear();
                }
                minAttackingShips = attackingShipsCount;
                _planetCandidatesToAttack.Add(planet);
            }
        }

        if(_planetCandidatesToAttack.Count == 0)
        {
            return null;
        }

        return _planetCandidatesToAttack[Random.Range(0, _planetCandidatesToAttack.Count)];
    }

    private void OnLooseAllSatellites(Planet planet)
    {
        Debug.LogError("GAME OVER");
        Debug.Break();
    }
}