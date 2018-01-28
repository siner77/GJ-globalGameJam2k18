using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

class LevelManager : MonoBehaviour
{
    public delegate void OnWinLooseDelegate();

    public OnWinLooseDelegate OnWin;
    public OnWinLooseDelegate OnLoose;

    [SerializeField]
    private float _progressModifier = 1.0f;
    [SerializeField]
    private float _gameTimeLimit = 300.0f;
    [SerializeField]
    private string _nextSceneName;

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

        Time.timeScale = 1;
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

    public float GetProgress()
    {
        return _progress;
    }

    public float GetTimeLeft()
    {
        return _gameTimeLimit - _gameTime;
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

    public void GoToNextLevel()
    {
        SceneManager.LoadScene(_nextSceneName);
    }

    private void OnLooseAllSatellites(Planet planet)
    {

    }

    private void GameOver()
    {
        Time.timeScale = 0;
        if(OnLoose != null)
        {
            OnLoose();
        }
    }

    private void Win()
    {
        Time.timeScale = 0;
        if (OnWin != null)
        {
            OnWin();
        }
    }
}