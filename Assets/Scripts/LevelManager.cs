using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

class LevelManager : MonoBehaviour
{
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

    private void OnLooseAllSatellites(Planet planet)
    {
        Debug.LogError("GAME OVER");
        Debug.Break();
    }

    public Planet GetPlanetToAttack()
    {
        _planetCandidatesToAttack.Clear();
        int minAttackingShips = int.MaxValue;

        foreach (Planet planet in _levelPlanets)
        {
            int attackingShipsCount = planet.GetAttackingShipsCount();
            if (attackingShipsCount <= minAttackingShips)
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
}