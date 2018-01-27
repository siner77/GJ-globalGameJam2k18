using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteCard : SelectableCard<Planet>
{
    [SerializeField]
    private Satellite _satellitePrefab;

    private SpawnPool<Satellite> _satellitesSpawnPool;

    private void OnEnable()
    {
        Init(LayerMask.GetMask("Planet"));
        _satellitesSpawnPool = new SpawnPool<Satellite>(_satellitePrefab, 10);
    }

    protected override void OnTargetClicked(Planet target)
    {
        base.OnTargetClicked(target);
        if(!target.HasUnusedSatelliteSpot())
        {
            return;
        }

        Satellite satellite = _satellitesSpawnPool.Get();
        target.AddSatellite(satellite);
    }
}
