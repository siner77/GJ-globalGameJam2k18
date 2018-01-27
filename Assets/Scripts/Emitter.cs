using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Emitter : MonoBehaviour
{
    protected Ray _signalRay = new Ray();
    [SerializeField]
    protected GameObject _signalPlaneObject;
    [SerializeField]
    protected LayerMask _signalLayerMask;

    private Satellite _lastSatellite = null;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void SetSignalRayParameters(Vector3 origin, Vector3 direction)
    {
        _signalRay.origin = origin;
        _signalRay.direction = direction;
    }

    protected virtual void SetSignalPlaneObject(Vector3 directionVector, float distance)
    {
        _signalPlaneObject.transform.forward = directionVector;
        _signalPlaneObject.transform.localScale = new Vector3(_signalPlaneObject.transform.localScale.x, _signalPlaneObject.transform.localScale.y, distance);
    }

    protected virtual void EmmitSignal()
    {
        RaycastHit[] hitInfos = Physics.RaycastAll(_signalRay, float.MaxValue, _signalLayerMask).OrderBy(h => h.distance).ToArray();
        int hitInfosLength = hitInfos.Length;
        if (hitInfosLength > 0)
        {
            for(int i = 0; i < hitInfosLength; ++i)
            {
                Satellite hitSatellite = hitInfos[i].collider.transform.parent.GetComponent<Satellite>();
                if (hitSatellite != null )   
                {
                    if( _lastSatellite != null)
                    {
                        if( hitSatellite != this)
                        {
                            _lastSatellite.OnGettingSignalEnd();
                            hitSatellite.OnGettingSignalStart();
                        }        
                    }
                    else
                    {
                        hitSatellite.OnGettingSignalStart();
                        _lastSatellite = hitSatellite;
                    }
                    hitSatellite.GetSignal(hitInfos[i], this.gameObject);
                    SetSignalPlaneObject(_signalRay.direction, hitInfos[i].distance);
                    break; 
                }
            }
        }
        else
        {
            if (_lastSatellite != null)
            {
                _lastSatellite.OnGettingSignalEnd();
                _lastSatellite = null;
            }
        }
    }


}
