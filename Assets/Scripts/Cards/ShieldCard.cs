using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCard : SelectableCard<Planet>
{
    [SerializeField]
    private float _shieldTime;

    private void OnEnable()
    {
        Init(LayerMask.GetMask("Planet"));
    }

    protected override void OnTargetClicked(Planet target)
    {
        base.OnTargetClicked(target);
        target.ActivateShield(_shieldTime);
    }
}
