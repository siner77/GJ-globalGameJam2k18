using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Emitter : MonoBehaviour
{
    protected Ray _signalRay = new Ray();
    [SerializeField]
    protected GameObject _signalPlaneObject;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void SetRayParameters(Vector3 origin, Vector3 direction)
    {
        _signalRay.origin = origin;
        _signalRay.direction = direction;
    }

    protected virtual void SetSignalPlaneObject()
    {

    }

    protected virtual void EmmitSignal()
    {
        SetRayParameters(transform.position, transform.up);
        RaycastHit hitInfo;
        if (Physics.Raycast(_signalRay, out hitInfo))
        {
            Satellite hitSatellite = gameObject.GetComponent<Satellite>();
            if (hitSatellite != null)
            {
                hitSatellite.GetSignal(hitInfo, this.gameObject);
            }
        }
    }
}
