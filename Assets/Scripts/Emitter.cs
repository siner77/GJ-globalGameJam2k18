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

    public virtual void EmmitSignal()
    {

    }
}
