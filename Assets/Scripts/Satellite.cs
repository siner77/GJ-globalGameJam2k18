using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satellite : Emitter
{

    private Planet _anchoredPlanet;
    public Planet AnchoredPlanet
    {
        get { return this._anchoredPlanet; }
        set { _anchoredPlanet = value; }
    }

	// Use this for initialization
	void Start () {
		
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
}
