using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableCard<T> : CardBase where T : MonoBehaviour
{
    private T _target;
    private int _layerMask;
    protected bool _startUsing;

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (!_startUsing)
        {
            return;
        }

        TrySelect();
        TryUse();
    }

    protected override void Use()
    {
        _startUsing = true;
    }

    protected void Init(int layerMask)
    {
        _layerMask = layerMask;
    }

    protected void TrySelect()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, _layerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider != null)
            {
                _target = hit.collider.GetComponentInParent<T>();
            }
        }
    }

    protected void TryUse()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _startUsing = false;
            _target = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _startUsing = false;
            if (_target != null)
            {
                OnUsed();
                OnTargetClicked(_target);
                _target = null;
            }
        }
    }

    protected virtual void OnTargetClicked(T target)
    {

    }
}
