using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Satellite : Emitter
{

    private Planet _anchoredPlanet;
    public Planet AnchoredPlanet
    {
        get { return this._anchoredPlanet; }
        set { _anchoredPlanet = value; }
    }

    [SerializeField]
    private float _maxHP;
    [SerializeField]
    private float _armor;

    private float _currentHP;


    // Use this for initialization
    void Start () {
		
	}

    private void OnEnable()
    {
        _currentHP = _maxHP;
    }


    // Update is called once per frame
    void Update () {
		
	}

    public void GetSignal(RaycastHit hitInfo, GameObject previousEmmiter)
    {
        Vector3 previousDirection = hitInfo.point - previousEmmiter.transform.position;
        Vector3 directionNormalized = Vector3.Reflect((previousDirection), hitInfo.normal);
        directionNormalized.y = 0;
        directionNormalized.Normalize();
        if (Vector3.Dot(previousDirection, directionNormalized) > 0) return;
        SetSignalRayParameters(gameObject.transform.position, directionNormalized);
        SetSignalPlaneObject(directionNormalized, hitInfo.distance);
        EmmitSignal();
    }

    public void OnGettingSignalStart()
    {
        _signalPlaneObject.SetActive(true);
    }

    public void OnGettingSignalEnd()
    {
        _signalPlaneObject.SetActive(false);
    }


    public bool IsAlive()
    {
        return gameObject.activeInHierarchy;
    }

    public void TakeDamage(float damage)
    {
        _currentHP -= Mathf.Max(damage - _armor, 0.0f);
        if (_currentHP <= 0.0f)
        {
            gameObject.SetActive(false);
            AnchoredPlanet.RemoveSatellite(this);
        }
    }

}
