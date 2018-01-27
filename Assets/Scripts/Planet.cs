using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
public class Spot
{
    public Transform SpotTransform;
    [HideInInspector]
    public bool IsUsed;
}

public class Planet : MonoBehaviour
{
    public delegate void OnLooseAllSatellitesDelegate(Planet planet);

    [SerializeField]
    private GameObject _satellitePrefab;
    [SerializeField]
    private float _rotationParameter = 5.0f;
    [SerializeField]
    private float _additionalOrbitRadius = 1.5f;
    [SerializeField]
    private Antenna _antenna;
    [SerializeField]
    private List<Spot> _enemySpots;
    [SerializeField]
    private List<Spot> _allySpots;
    [SerializeField]
    private List<Spot> _satelliteSpots;
    private List<Satellite> _satellistes = new List<Satellite>();
    private List<EnemyShipController> _enemyShips = new List<EnemyShipController>();
    private List<ShipController> _friendlyShips = new List<ShipController>();

    public OnLooseAllSatellitesDelegate OnLooseAllSatellites;

    public List<Spot> EnemySpots
    {
        get { return _enemySpots; }
        set { _enemySpots = value; }
    }

    public List<Spot> AllySpots
    {
        get { return _allySpots; }
        set { _allySpots = value; }
    }

    public List<Spot> SatelliteSpots
    {
        get { return _satelliteSpots; }
        set { _satelliteSpots = value; }
    }

    // Use this for initialization
    void Start ()
    {
        if(_satellitePrefab != null)
        {
            GameObject satelliteObject = Instantiate(_satellitePrefab);
            Satellite satellite = satelliteObject.GetComponent<Satellite>();
            if(satellite != null)
            {
                AddSatellite(satellite);
            }
        }

        _satellistes = GetComponentsInChildren<Satellite>().ToList();	
        foreach(Satellite satellite in _satellistes)
        {
            satellite.AnchoredPlanet = this;
        }

        SetProperSpotPositions(_enemySpots);
        SetProperSpotPositions(_allySpots);
	}

    // Update is called once per frame
    void Update() {
        this.gameObject.transform.Rotate(Vector3.up * Time.deltaTime * _rotationParameter);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.DrawWireDisc(transform.position, Vector3.up, GetOrbitDistanceFromPlanet());
    }
#endif

    public Antenna GetAntenna()
    {
        return _antenna;
    }

    public EnemyShipController GetNearestShip(Vector3 position)
    {
        EnemyShipController nearestEnemy = null;
        float minDistance = float.MaxValue;
        int enemyShipsLength = _enemyShips.Count;
        if (enemyShipsLength > 0)
        {
            for(int i = 0; i < enemyShipsLength; ++i)
            {
                float distance = Vector3.Distance(_enemyShips[i].transform.position, position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = _enemyShips[i];
                }
            }
        }
        return nearestEnemy;
    }

    public Satellite GetNearestSatellite(Vector3 position)
    {
        Satellite nearestSatellite = null;
        float minDistance = float.MaxValue;
        int satellitesLength = _satellistes.Count;
        if (satellitesLength > 0)
        {
            for (int i = 0; i < satellitesLength; ++i)
            {
                float distance = Vector3.Distance(_satellistes[i].transform.position, position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestSatellite = _satellistes[i];
                }
            }
        }
        return nearestSatellite;
    }

    public float GetOrbitDistanceFromPlanet()
    {
        return Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z) + _additionalOrbitRadius;
    }

    public int GetAttackingShipsCount()
    {
        return _enemyShips.Count;
    }

    public void AddEnemyShip(EnemyShipController enemyShip)
    {
        if(enemyShip == null || _enemyShips.Contains(enemyShip))
        {
            return;
        }
        _enemyShips.Add(enemyShip);
    }

    public void RemoveEnemyShip(EnemyShipController enemyShip)
    {
        if (enemyShip == null || !_enemyShips.Contains(enemyShip))
        {
            return;
        }
        _enemyShips.Remove(enemyShip);
    }

    public void AddFriendlyShip(ShipController friendlyShip)
    {
        _friendlyShips.Add(friendlyShip);
    }

    public void RemoveFriendlyShip(ShipController friendlyShip)
    {
        _friendlyShips.Remove(friendlyShip);
    }

    public void AddSatellite(Satellite satellite)
    {
        if(HasUnusedSatelliteSpot())
        {
            Spot satelliteSpot = GetClosestUnusedSatelliteSpot(Vector3.zero);
            satellite.AnchoredPlanet = this;
            satellite.transform.parent = satelliteSpot.SpotTransform;
            satellite.transform.localPosition = Vector3.zero;
            satelliteSpot.IsUsed = true;
            _satellistes.Add(satellite);
        }
    }

    public void RemoveSatellite(Satellite satellie)
    {
        satellie.transform.parent = null;
        satellie.UsedSpot.IsUsed = false;
        _satellistes.Remove(satellie);

        if(_satellistes.Count == 0 && OnLooseAllSatellites != null)
        {
            OnLooseAllSatellites(this);
        }
    }

    public Spot GetRandomUnusedAllySpot()
    {
        return GetClosestUnusedAllySpot(Vector3.zero);
    }

    public Spot GetClosestUnusedEnemySpot(Vector3 position)
    {
        return GetClosestUnusedSpot(position, _enemySpots);
    }

    public Spot GetClosestUnusedAllySpot(Vector3 position)
    {
        return GetClosestUnusedSpot(position, _allySpots);
    }

    public Spot GetClosestUnusedSatelliteSpot(Vector3 position)
    {
        return GetClosestUnusedSpot(position, _satelliteSpots);
    }

    public bool HasSatellites()
    {
        return _satellistes.Count > 0;
    }

    public bool HasUnusedEnemySpot()
    {
        return HasUnusedSpot(_enemySpots);
    }

    public bool HasUnusedAllySpot()
    {
        return HasUnusedSpot(_allySpots);
    }

    public bool HasUnusedSatelliteSpot()
    {
        return HasUnusedSpot(_satelliteSpots);
    }

    private bool HasUnusedSpot(List<Spot> spotList)
    {
        foreach(Spot spot in spotList)
        {
            if(!spot.IsUsed)
            {
                return true;
            }
        }

        return false;
    }

    private Spot GetClosestUnusedSpot(Vector3 position, List<Spot> spotList)
    {
        float minDistance = float.MaxValue;
        Spot unusedSpot = null;

        foreach (Spot spot in spotList)
        {
            float distance = Vector3.Distance(spot.SpotTransform.position, position);
            if (distance < minDistance && !spot.IsUsed)
            {
                minDistance = distance;
                unusedSpot = spot;
            }
        }

        return unusedSpot;
    }

    private void SetProperSpotPositions(List<Spot> spotList)
    {
        foreach(Spot spot in spotList)
        {
            if(spot.SpotTransform == null)
            {
                continue;
            }
            spot.SpotTransform.position = (spot.SpotTransform.position - transform.position).normalized * GetOrbitDistanceFromPlanet() + transform.position;
        }
    }

    public void ActivateShield(float shieldTime)
    {
        foreach(Satellite satellite in _satellistes)
        {
            satellite.ActivateShield(shieldTime);
        }
    }
}
