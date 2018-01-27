using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipStates
{
    public class AttackPlanet : IState<ShipController>
    {
        private enum AttackState
        {
            ROTATE_TOWARDS_ENEMY,
            WAIT_AFTER_ATTACK
        }

        private Planet _target;
        private Satellite _closestSatellite;
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

            _attackTimer = 0.0f;
            _waitTimer = 0.0f;
            _attackState = AttackState.ROTATE_TOWARDS_ENEMY;
            controller.NavMeshAgent.enabled = false;
            _closestSatellite = _target.GetNearestSatellite(controller.transform.position);

            _target.AddEnemyShip((EnemyShipController)controller);
        }

        public void OnExit(ShipController controller)
        {
            controller.NavMeshAgent.enabled = true;
            _target.RemoveEnemyShip((EnemyShipController)controller);
        }

        public void OnUpdate(ShipController controller)
        {
            if (_closestSatellite == null || !_closestSatellite.IsAlive() || !controller.IsTargetInSight(_closestSatellite.gameObject))
            {
                Satellite closestSatellite = _target.GetNearestSatellite(controller.transform.position);
                if (closestSatellite == null)
                {
                    controller.SetState(null);
                    return;
                }
                _closestSatellite = closestSatellite;
            }

            if (_attackState == AttackState.ROTATE_TOWARDS_ENEMY)
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
            Vector3 targetForward = (closestSatellite.transform.position - controller.transform.position);
            targetForward.y = 0.0f;

            controller.RotateTowards(targetForward.normalized);

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
                _attackState = AttackState.ROTATE_TOWARDS_ENEMY;
            }
        }
    }
}

public class EnemyShipController : ShipController
{
    public override void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, float.MaxValue, _shootRaycastLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider == null)
            {
                return;
            }

            Satellite target = hit.collider.GetComponent<Satellite>();
            ShipController enemy = hit.collider.GetComponent<ShipController>();
            if (target == null && enemy == null)
            {
                return;
            }

            if (target != null)
            {

            }
            else if (enemy != null)
            {
                enemy.TakeDamage(AttackDamage, this);
            }
        }
    }

    [ContextMenu("test")]
    private void Test()
    {
        Planet p = FindObjectOfType<Planet>();
        SetState(new ShipStates.GoToPlanet(p, new ShipStates.AttackPlanet(p)));
    }
}
