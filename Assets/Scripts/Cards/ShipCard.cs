using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCard : SelectableCard<Planet>
{
    [SerializeField]
    private ShipController _shipPrefab;

    private SpawnPool<ShipController> _shipSpawnPool;

    private void OnEnable()
    {
        Init(LayerMask.GetMask("Planet"));
        _shipSpawnPool = new SpawnPool<ShipController>(_shipPrefab, 10);
    }

    protected override void OnTargetClicked(Planet target)
    {
        base.OnTargetClicked(target);
        if(!target.HasUnusedAllySpot())
        {
            return;
        }

        // TODO: Determine spawn position
        ShipController spawnedShip = _shipSpawnPool.Get();
        spawnedShip.SetState(new ShipStates.GoToPlanet(target, new ShipStates.Defend(target)));
    }
}