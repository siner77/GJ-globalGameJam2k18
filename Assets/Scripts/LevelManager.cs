using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class LevelManager : MonoBehaviour
{
    private List<Planet> _levelPlanets;

    private void Awake()
    {
        _levelPlanets = FindObjectsOfType<Planet>().ToList();
        foreach(Planet planet in _levelPlanets)
        {
            planet.OnLooseAllSatellites += OnLooseAllSatellites;
        }
    }

    private void OnLooseAllSatellites(Planet planet)
    {
        // It's working
        Debug.LogError("GAME OVER");
        Debug.Break();
    }

    public Planet FindClosestPlanet(Vector3 position)
    {
        return null;
    }
}