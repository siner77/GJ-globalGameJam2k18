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

    private Planet _startPlanet;

    private void Awake()
    {
        _levelPlanets = FindObjectsOfType<Planet>().ToList();
        foreach(Planet planet in _levelPlanets)
        {
            planet.OnLooseAllSatellites += OnLooseAllSatellites;
            Antenna antenna = planet.GetAntenna();
            if(antenna != null && antenna.Type == EAntennaType.EMITTER)
            {
                if(_startPlanet != null)
                {
                    Debug.LogError("There are multiple start planets on the level");
                }
                _startPlanet = planet;
            }
        }
        _planetCandidatesToAttack = new List<Planet>();
    }

    void Update()
    {
        _gameTime += Time.deltaTime;
        if (_gameTime > _gameTimeLimit)
        {
            GameOver();
        }
    }

    public void ImproveProgress()
    {
        _progress += Time.deltaTime * _progressModifier;
        if (_progress >= _progressLimit)
        {
            Win();
        }
    }

    public Planet GetPlanetToAttack()
    {
        _planetCandidatesToAttack.Clear();
        int minAttackingShips = int.MaxValue;

        foreach (Planet planet in _levelPlanets)
        {
            int attackingShipsCount = planet.GetAttackingShipsCount();
            if (attackingShipsCount <= minAttackingShips && planet.HasUnusedEnemySpot() && planet.HasSatellites())
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

    public Planet GetStartingPlanet()
    {
        return _startPlanet;
    }

    private void OnLooseAllSatellites(Planet planet)
    {

    }

    private void GameOver()
    {
        Debug.LogError("GAME OVER");
        Debug.Break();
    }

    private void Win()
    {
        Debug.Log("Win, GZ");
        Debug.Break();
    }
}