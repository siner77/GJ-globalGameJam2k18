using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antenna : Emitter {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void EmmitSignal()
    {
        SetRayParameters(transform.position, transform.up);
        RaycastHit hitInfo;
        if ( Physics.Raycast(_signalRay, out hitInfo))
        {
            Satellite hitSatellite = gameObject.GetComponent<Satellite>();
            if(hitSatellite != null)
            {
                hitSatellite.GetSignal(hitInfo, this.gameObject);
            }
        }
    }
}
