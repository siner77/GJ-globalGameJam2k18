using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeCard : SelectableCard<EnemyShipController>
{
    [SerializeField]
    private float _freezeTime;

    private void OnEnable()
    {
        Init(LayerMask.GetMask("Ship"));
    }

    protected override void OnTargetClicked(EnemyShipController target)
    {
        base.OnTargetClicked(target);
        target.Freeze(_freezeTime);
    }
}
