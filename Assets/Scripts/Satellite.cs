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

    private Spot _usedSpot;
    public Spot UsedSpot
    {
        get { return this._usedSpot; }
        set { _usedSpot = value; }
    }

    [SerializeField]
    private float _maxHP;
    [SerializeField]
    private float _armor;

    private float _currentHP;

    private bool _gotSignalThisFrame;


    // Use this for initialization
    void Start () {
		
	}

    private void OnEnable()
    {
        _currentHP = _maxHP;
    }


    // Update is called once per frame
    void Update () {
        _gotSignalThisFrame = false;
	}

    public override void GetSignal(RaycastHit hitInfo, GameObject previousEmmiter)
    {
        if (_gotSignalThisFrame)
        {
            return;
        }
        SetSignalRayParameters(gameObject.transform.position, this.transform.forward);
        _gotSignalThisFrame = true;
        EmmitSignal(previousEmmiter);
    }

    public void OnGettingSignalStart()
    {
        if (!_signalPlaneObject.activeSelf)
        {
            _signalPlaneObject.SetActive(true);
        }
    }

    public void OnGettingSignalEnd()
    {
        if (_signalPlaneObject.activeSelf)
        {
            _signalPlaneObject.SetActive(false);
            if (_lastSatellite != null)
            {
                _lastSatellite.OnGettingSignalEnd();
                _lastSatellite = null;
            }
        }
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
