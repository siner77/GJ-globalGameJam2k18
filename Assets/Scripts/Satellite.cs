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
    private GameObject _shield;
    [SerializeField]
    private float _maxHP;
    [SerializeField]
    private float _armor;

    private float _currentHP;
    private float _shieldTime;
    private int _signals;
    private cakeslice.Outline _outline;

    private bool _gotSignalThisFrame;

    // Use this for initialization
    private void Start ()
    {
        _shield.SetActive(false);
        _signals = 0;
    }

    private void OnEnable()
    {
        _currentHP = _maxHP;
        _outline = gameObject.GetComponentInChildren<cakeslice.Outline>(true);
        _signals = 0;
    }


    // Update is called once per frame
    private void Update ()
    {
        _gotSignalThisFrame = false;

        if(_shield.activeInHierarchy)
        {
            _shieldTime -= Time.deltaTime;
            if(_shieldTime <= 0.0f)
            {
                _shield.SetActive(false);
            }
        }
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
        _signals += 1;
        if (!_signalPlaneObject.activeSelf && _signals == 1)
        {
            if(_outline != null)
            {
                _outline.enabled = true;
            }
            _signalPlaneObject.SetActive(true);
        }
    }

    public void OnGettingSignalEnd()
    {
        _signals -= 1;
        _signals = Mathf.Min(_signals, 0);
        if (_signalPlaneObject.activeSelf && _signals == 0)
        {
            _signalPlaneObject.SetActive(false);
            if (_outline != null)
            {
                _outline.enabled = false;
            }
            if (_lastSatellite != null)
            {
                _lastSatellite.OnGettingSignalEnd();
                _lastSatellite = null;
            }
        }
    }

    public void ActivateShield(float shieldTime)
    {
        _shieldTime = shieldTime;
        _shield.SetActive(true);
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
