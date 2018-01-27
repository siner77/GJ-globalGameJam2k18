using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipStates
{
    public class GoToSatelite : IState<ShipController>
    {
        private Satellite _target;

        public GoToSatelite(Satellite target)
        {
            _target = target;
        }

        public void OnEnter(ShipController controller)
        {

        }

        public void OnExit(ShipController controller)
        {

        }

        public void OnUpdate(ShipController controller)
        {
            controller.transform.position = Vector3.MoveTowards(controller.transform.position, _target.transform.position, controller.FlySpeed * Time.deltaTime);

            // TODO:
            // Find a better way to rotate ship
            controller.transform.rotation = Quaternion.RotateTowards(controller.transform.rotation, Quaternion.LookRotation((_target.transform.position - controller.transform.position).normalized), Time.deltaTime * controller.RotateSpeed);

            if (Vector3.Distance(controller.transform.position, _target.transform.position) < controller.AttackRange)
            {
                controller.SetState(new AttackSatelite(_target));
            }
        }
    }

    public class AttackSatelite : IState<ShipController>
    {
        private enum AttackState
        {
            ROTATE_TOWARDS_ENEMY,
            WAIT_AFTER_ATTACK
        }

        private Satellite _target;
        private float _attackTimer;
        private float _waitTimer;
        private AttackState _attackState;

        public AttackSatelite(Satellite target)
        {
            _target = target;
        }

        public void OnEnter(ShipController controller)
        {
            _attackTimer = 0.0f;
            _waitTimer = 0.0f;
            _attackState = AttackState.ROTATE_TOWARDS_ENEMY;
        }

        public void OnExit(ShipController controller)
        {

        }

        public void OnUpdate(ShipController controller)
        {
            // TODO:
            // Check if satelite is still alive
            // If not then target next satelite

            if (_attackState == AttackState.ROTATE_TOWARDS_ENEMY)
            {
                UpdateRotateTowardsEnemy(controller);
            }
            else if (_attackState == AttackState.WAIT_AFTER_ATTACK)
            {
                UpdateWaitAfterAttack(controller);
            }
        }

        private void UpdateRotateTowardsEnemy(ShipController controller)
        {
            controller.transform.forward = (_target.transform.position - controller.transform.position).normalized;

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
        SetState(new ShipStates.GoToSatelite(FindObjectOfType<Satellite>()));
    }
}
