using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Planet : MonoBehaviour
{
    public delegate void OnLooseAllSatellitesDelegate(Planet planet);

    [SerializeField]
    private float _rotationParameter = 5.0f;
    [SerializeField]
    private float _additionalOrbitRadius = 1.5f;
    [SerializeField]
    private Antenna _antenna;
    private List<Satellite> _satellistes = new List<Satellite>();
    private List<EnemyShipController> _enemyShips = new List<EnemyShipController>();
    private List<ShipController> _friendlyShips = new List<ShipController>();

    public OnLooseAllSatellitesDelegate OnLooseAllSatellites;

    // Use this for initialization
    void Start ()
    {
        _satellistes = GetComponentsInChildren<Satellite>().ToList();	
        foreach(Satellite satellite in _satellistes)
        {
            satellite.AnchoredPlanet = this;
        }
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

    public void AddSatellite(Satellite satellie)
    {
        _satellistes.Add(satellie);
        satellie.transform.parent = this.transform;
    }

    public void RemoveSatellite(Satellite satellie)
    {
        satellie.transform.parent = null;
        _satellistes.Remove(satellie);

        if(_satellistes.Count == 0 && OnLooseAllSatellites != null)
        {
            OnLooseAllSatellites(this);
        }
    }
}
