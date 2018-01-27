using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Antenna : Emitter {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        EmmitSignal();
	}

    protected override void EmmitSignal()
    {
        //Debug.Log("emmit signal" + this.GetInstanceID());
        SetSignalRayParameters(_signalPlaneObject.transform.position, transform.up);
        base.EmmitSignal();
    }
}
