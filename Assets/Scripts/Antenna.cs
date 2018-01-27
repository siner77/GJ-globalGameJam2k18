﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EAntennaType
{
    EMITTER = 0,
    RECEIVER = 1
    
}

public class Antenna : Emitter {

    public EAntennaType Type = EAntennaType.EMITTER;
    private RoundController _roundController = null;
	// Use this for initialization
	void Start () {
		
	}

    void OnEnable()
    {
        if(Type == EAntennaType.RECEIVER)
        {
            _roundController = GameObject.FindObjectOfType<RoundController>();
            if(_roundController == null)
            {
                Debug.LogError("there is no roundController");
                Debug.Break();
            }
            this._signalPlaneObject.SetActive(false);
        }
    }

    public override void GetSignal(RaycastHit hitInfo, GameObject previousEmmiter)
    {
        if (Type == EAntennaType.RECEIVER)
        {
            _roundController.ImproveProgress();
        }
    }

    // Update is called once per frame
    void Update () {
            EmmitSignal(null);
	}

    protected override void EmmitSignal(GameObject emmiterObject)
    {
        if (Type == EAntennaType.EMITTER)
        {
            SetSignalRayParameters(_signalPlaneObject.transform.position, transform.up);
            base.EmmitSignal(emmiterObject);
        }
    }
}
