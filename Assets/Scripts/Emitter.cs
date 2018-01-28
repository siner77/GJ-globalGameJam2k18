using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Emitter : MonoBehaviour
{
    public class Signal
    {
        public List<Emitter> AllEmmiters = new List<Emitter>();
    }

    protected Ray _signalRay = new Ray();
    [SerializeField]
    protected GameObject _signalPlaneObject;
    [SerializeField]
    protected LayerMask _signalLayerMask;
    [SerializeField]
    protected Transform _raycastOrigin;

    protected Satellite _lastSatellite = null;
    protected GameObject _lastObstacle = null;

    public virtual void GetSignal(RaycastHit hitInfo, GameObject previousEmmiter, Signal signal)
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

    protected virtual void EmmitSignal(GameObject emmiterObject, Signal signal)
    {
        RaycastHit[] hitInfos = Physics.RaycastAll(_signalRay, float.MaxValue, _signalLayerMask).OrderBy(h => h.distance).ToArray();
        int hitInfosLength = hitInfos.Length;
        Satellite hitSatellite = null;
        if (hitInfosLength > 0)
        {
            for (int i = 0; i < hitInfosLength; ++i)
            {
                if (hitInfos[i].collider.gameObject.layer == LayerMask.NameToLayer("Obstacle") || hitInfos[i].collider.gameObject.layer == LayerMask.NameToLayer("Planet"))
                {
                    SetSignalPlaneObject(_signalRay.direction, hitInfos[i].distance);
                    break;
                    //cakeslice.Outline outline = hitInfos[i].collider.gameObject.GetComponentInChildren<cakeslice.Outline>();
                    //if(outline != null)
                    //{
                    //    outline.enabled = true;
                    //}
                    //SetSignalPlaneObject(_signalRay.direction, hitInfos[i].distance);
                    //TryClearLastSatellite();
                    //break;
                }

                SetSignalPlaneObject(_signalRay.direction, hitInfos[i].distance);

                Satellite hitSatelliteLocal = hitInfos[i].collider.transform.GetComponentInParent<Satellite>();
                if (hitSatelliteLocal != null && hitSatelliteLocal != this && emmiterObject != hitSatelliteLocal.gameObject && emmiterObject != this.gameObject)
                {
                    hitSatellite = hitSatelliteLocal;
                    if(hitSatellite != null)
                    {
                        if(signal.AllEmmiters.Contains(hitSatellite))
                        {
                            break;
                        }
                        signal.AllEmmiters.Add(hitSatellite);
                    }
                    else
                    {
                        Debug.Log("Ups?");
                    }
                    if (_lastSatellite != null)
                    {
                        if (hitSatellite != _lastSatellite)
                        {
                            hitSatellite.OnGettingSignalStart();
                            _lastSatellite.OnGettingSignalEnd();
                            _lastSatellite = hitSatellite;
                        }
                    }
                    else
                    {
                        hitSatellite.OnGettingSignalStart();
                        _lastSatellite = hitSatellite;
                    }
                    hitSatellite.GetSignal(hitInfos[i], this.gameObject, signal);
                    break;
                }
                Antenna hitAntenna = hitInfos[i].collider.transform.GetComponentInParent<Antenna>();
                if (hitAntenna != null && hitAntenna.Type == EAntennaType.RECEIVER)
                {
                    hitAntenna.GetSignal(hitInfos[i], this.gameObject, signal);
                    TryClearLastSatellite();
                    break;
                }
            }
        }
        else
        {
            SetSignalPlaneObject(_signalRay.direction, 30.0f);
        }

        if (_lastSatellite != null)
        {
            if (hitSatellite == null)
            {
                _lastSatellite.OnGettingSignalEnd();
                _lastSatellite = null;
            }
        }
    }

    private void TryClearLastSatellite()
    {
        if (_lastSatellite != null)
        {
            _lastSatellite.OnGettingSignalEnd();
            _lastSatellite = null;
        }
    }
    
}
