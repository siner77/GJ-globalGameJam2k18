using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipStates
{
    public class GoToSatelite : IState<ShipController>
    {
        private Planet _targetPlanet;
        private Vector3 _offset;

        public GoToSatelite(Planet targetPlanet)
        {
            _targetPlanet = targetPlanet;
        }

        public void OnEnter(ShipController controller)
        {
            controller.NavMeshAgent.SetDestination(_targetPlanet.transform.position + (controller.transform.position - _targetPlanet.transform.position).normalized * _targetPlanet.GetOrbitDistanceFromPlanet());
        }

        public void OnExit(ShipController controller)
        {

        }

        public void OnUpdate(ShipController controller)
        {
            if(HasReachedPosition(controller))
            {
                controller.SetState(new AttackPlanet(_targetPlanet));
            }
        }

        private bool HasReachedPosition(ShipController controller)
        {
            return controller.NavMeshAgent.remainingDistance <= controller.NavMeshAgent.stoppingDistance && controller.NavMeshAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathComplete;
        }
    }

    public class AttackPlanet : IState<ShipController>
    {
        private enum AttackState
        {
            ROTATE_TOWARDS_ENEMY,
            WAIT_AFTER_ATTACK
        }

        private Planet _target;
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
                Debug.LogError("Handle this");
                controller.SetState(null);
                return;
            }

            _attackTimer = 0.0f;
            _waitTimer = 0.0f;
            _attackState = AttackState.ROTATE_TOWARDS_ENEMY;
            controller.NavMeshAgent.enabled = false;
        }

        public void OnExit(ShipController controller)
        {
            controller.NavMeshAgent.enabled = true;
        }

        public void OnUpdate(ShipController controller)
        {
            Satellite closestSatellite = _target.GetNearestSatellite(controller.transform.position);
            if(closestSatellite == null)
            {
                controller.SetState(null);
                return;
            }

            if (_attackState == AttackState.ROTATE_TOWARDS_ENEMY)
            {
                UpdateRotateTowardsEnemy(controller, closestSatellite);
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

            controller.transform.rotation = Quaternion.RotateTowards(controller.transform.rotation, Quaternion.LookRotation(targetForward.normalized), controller.RotateSpeed * Time.deltaTime);

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
        if (Physics.Raycast(transform.position, transform.forward, out hit, AttackRange, _shootRaycastLayerMask, QueryTriggerInteraction.Ignore))
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
        SetState(new ShipStates.GoToSatelite(FindObjectOfType<Planet>()));
    }
}
