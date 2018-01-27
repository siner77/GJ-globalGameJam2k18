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

	// Use this for initialization
	void Start () {
		
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

}
