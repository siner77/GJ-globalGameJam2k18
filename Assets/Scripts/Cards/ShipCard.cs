using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipCard : SelectableCard<Planet>
{
    [SerializeField]
    private ShipController _shipPrefab;
    [SerializeField]
    private Text _attackStat;
    [SerializeField]
    private Text _hpStat;
    [SerializeField]
    private Text _speedStat;

    private SpawnPool<ShipController> _shipSpawnPool;
    private LevelManager _levelManager;

    private void OnEnable()
    {
        Init(LayerMask.GetMask("Planet"));
        _shipSpawnPool = new SpawnPool<ShipController>(_shipPrefab, 10);
        _levelManager = FindObjectOfType<LevelManager>();

        _attackStat.text = "Attack: " + _shipPrefab.AttackDamage;
        _hpStat.text = "HP: " + _shipPrefab.GetMaxHP();
        _speedStat.text = "Speed: " + _shipPrefab.FlySpeed;
    }

    protected override void OnTargetClicked(Planet target)
    {
        base.OnTargetClicked(target);
        if(!target.HasUnusedAllySpot())
        {
            return;
        }

        Planet startingPlanet = _levelManager.GetStartingPlanet();
        if(startingPlanet == null || !startingPlanet.HasUnusedAllySpot())
        {
            return;
        }

        ShipController spawnedShip = _shipSpawnPool.Get();
        Spot unusedSpot = startingPlanet.GetRandomUnusedAllySpot();
        spawnedShip.transform.position = unusedSpot.SpotTransform.position;
        spawnedShip.SetState(new ShipStates.GoToPlanet(target, new ShipStates.Defend(target)));
    }
}