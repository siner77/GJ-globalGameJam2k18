using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satellite : Emitter
{
    [SerializeField]
    private float _maxHP;
    [SerializeField]
    private float _armor;

    private float _currentHP;

    private Planet _anchoredPlanet;
    public Planet AnchoredPlanet
    {
        get { return this._anchoredPlanet; }
        set { _anchoredPlanet = value; }
    }

	// Use this for initialization
	void Start ()
    {

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
        Vector3 previousDirectionNormalized = hitInfo.point - previousEmmiter.transform.position;
        previousDirectionNormalized.Normalize();
        _signalRay.direction = Vector3.Reflect(previousDirectionNormalized, hitInfo.normal);
        SetRayParameters(transform.position, transform.up);
        EmmitSignal();
    }

    public bool IsAlive()
    {
        return gameObject.activeInHierarchy;
    }

    public void TakeDamage(float damage)
    {
        _currentHP -= Mathf.Max(damage - _armor, 0.0f);
        if(_currentHP <= 0.0f)
        {
            gameObject.SetActive(false);
            AnchoredPlanet.RemoveSatellite(this);
        }
    }
}
