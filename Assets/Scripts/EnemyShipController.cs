using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipStates
{
    public class AttackPlanet : IState<ShipController>
    {
        private enum AttackState
        {
            TRY_SHOOT,
            WAIT_AFTER_ATTACK
        }

        private Planet _target;
        private Satellite _closestSatellite;
        private LevelManager _levelManager;
        private float _attackTimer;
        private float _waitTimer;
        private AttackState _attackState;

        public AttackPlanet(Planet targetPlanet)
        {
            _target = targetPlanet;
        }

        public void OnEnter(ShipController controller)
        {
            if(_target == null)
            {
                controller.SetState(null);
                return;
            }

            _levelManager = Object.FindObjectOfType<LevelManager>();

            _attackTimer = 0.0f;
            _waitTimer = 0.0f;
            _attackState = AttackState.TRY_SHOOT;
            controller.NavMeshAgent.enabled = false;
            _closestSatellite = _target.GetNearestSatellite(controller.transform.position);
        }

        public void OnExit(ShipController controller)
        {
            controller.NavMeshAgent.enabled = true;
        }

        public void OnUpdate(ShipController controller)
        {
            if (_closestSatellite == null || !_closestSatellite.IsAlive() || !controller.IsTargetInSight(_closestSatellite))
            {
                Satellite closestSatellite = _target.GetNearestSatellite(controller.transform.position);
                if (closestSatellite == null)
                {
                    Planet nextPlanetToAttack = _levelManager.GetPlanetToAttack();
                    if(nextPlanetToAttack != null)
                    {
                        controller.SetState(new GoToPlanet(nextPlanetToAttack, new AttackPlanet(nextPlanetToAttack)));
                    }
                    else
                    {
                        controller.SetState(null);
                    }
                    return;
                }
                _closestSatellite = closestSatellite;
            }

            Vector3 targetForward = (_closestSatellite.transform.position - controller.transform.position);
            targetForward.y = 0.0f;

            controller.RotateTowards(targetForward.normalized);

            if (_attackState == AttackState.TRY_SHOOT)
            {
                UpdateRotateTowardsEnemy(controller, _closestSatellite);
            }
            else if (_attackState == AttackState.WAIT_AFTER_ATTACK)
            {
                UpdateWaitAfterAttack(controller);
            }
        }

        private void UpdateRotateTowardsEnemy(ShipController controller, Satellite closestSatellite)
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= controller.AttackCooldown)
            {
                controller.Shoot();
                _waitTimer = 0.0f;
                _attackState = AttackState.WAIT_AFTER_ATTACK;
            }
        }

        private void UpdateWaitAfterAttack(ShipController controller)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= controller.AfterAttackWaitTime)
            {
                _attackTimer = 0.0f;
                _attackState = AttackState.TRY_SHOOT;
            }
        }
    }

    public class Freeze : IState<ShipController>
    {
        private IState<ShipController> _stateToRestore;
        private float _freezeTime;

        private float _timer;

        public Freeze(float freezeTime, IState<ShipController> stateToSet)
        {
            _freezeTime = freezeTime;
            _stateToRestore = stateToSet;
        }

        public void OnEnter(ShipController controller)
        {
            _timer = 0.0f;
            controller.NavMeshAgent.isStopped = true;
        }

        public void OnExit(ShipController controller)
        {
            controller.NavMeshAgent.isStopped = false;
        }

        public void OnUpdate(ShipController controller)
        {
            _timer += Time.deltaTime;
            if(_timer > _freezeTime)
            {
                controller.SetState(_stateToRestore);
            }
        }
    }
}

public class EnemyShipController : ShipController
{
    private Planet _planetToAttack;

    public void SetPlanetToAttack(Planet planet)
    {
        _planetToAttack = planet;
        planet.AddEnemyShip(this);
        transform.forward = planet.transform.position - transform.position;
    }

    public override void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(_raycastOrigin.transform.position, transform.forward, out hit, float.MaxValue, _shootRaycastLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider == null)
            {
                return;
            }

            Satellite target = hit.collider.GetComponentInParent<Satellite>();
            ShipController enemy = hit.collider.GetComponentInParent<ShipController>();
            if (target == null && enemy == null)
            {
                return;
            }

            if (target != null)
            {
                target.TakeDamage(AttackDamage);
            }
            else if (enemy != null)
            {
                enemy.TakeDamage(AttackDamage, this);
            }
        }
    }

    public void Freeze(float freezeTime)
    {
        SetState(new ShipStates.Freeze(freezeTime, StateMachine.GetCurrentState()));
    }

    public override bool IsEnemy()
    {
        return true;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        _planetToAttack.RemoveEnemyShip(this);
    }
}
