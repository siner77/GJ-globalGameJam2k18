using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipStates
{
    public class GoToPlanet : IState<ShipController>
    {
        private Vector3 _targetPosition;

        public GoToPlanet(Planet target)
        {

        }

        public void OnEnter(ShipController controller)
        {

        }

        public void OnExit(ShipController controller)
        {

        }

        public void OnUpdate(ShipController controller)
        {

        }
    }

    public class Defend : IState<ShipController>
    {
        public Defend(Planet target)
        {

        }

        public void OnEnter(ShipController controller)
        {
            // TODO:
            // Check if planet is under attack
            // Get ship attacking planet
            // Change state to attack

            // TODO:
            // Attach to planet and fly in its orbit
        }

        public void OnExit(ShipController controller)
        {

        }

        public void OnUpdate(ShipController controller)
        {
            // TODO:
            // Check if planet is under attack
            // Get ship attacking planet
            // Change state to attack
        }
    }

    public class Attack : IState<ShipController>
    {
        private enum AttackState
        {
            ROTATE_TOWARDS_ENEMY,
            WAIT_AFTER_ATTACK
        }

        private ShipController _enemy;
        private IState<ShipController> _stateAfterEnemyDeath;
        private float _attackTimer;
        private float _waitTimer;
        private AttackState _attackState;

        public Attack(ShipController enemy, IState<ShipController> stateAfterEnemyDeath)
        {
            _enemy = enemy;
            _stateAfterEnemyDeath = stateAfterEnemyDeath;
        }

        public void OnEnter(ShipController controller)
        {
            if (!_enemy.IsAlive())
            {
                controller.SetState(_stateAfterEnemyDeath);
                return;
            }

            _attackTimer = 0.0f;
            _waitTimer = 0.0f;
            _attackState = AttackState.ROTATE_TOWARDS_ENEMY;
        }

        public void OnExit(ShipController controller)
        {

        }

        public void OnUpdate(ShipController controller)
        {
            if (!_enemy.IsAlive())
            {
                controller.SetState(_stateAfterEnemyDeath);
                return;
            }

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
            controller.transform.forward = (_enemy.transform.position - controller.transform.position).normalized;

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

    public class Die : IState<ShipController>
    {
        public void OnEnter(ShipController controller)
        {
            controller.gameObject.SetActive(false);
        }

        public void OnExit(ShipController controller)
        {

        }

        public void OnUpdate(ShipController controller)
        {

        }
    }
}

public class ShipController : StateMachineController<ShipController>
{
    public float AttackCooldown = 3.0f;
    public float AfterAttackWaitTime = 1.0f;
    public float AttackDamage = 1.0f;
    public float AttackRange = 3.0f;
    public float Armor = 0.0f;
    public float FlySpeed = 1.0f;
    public float RotateSpeed = 1.0f;
    [SerializeField]
    protected float _maxHP = 1.0f;

    protected float _currentHP;
    protected int _shootRaycastLayerMask;

    protected virtual void OnEnable()
    {
        _currentHP = _maxHP;
        _shootRaycastLayerMask = LayerMask.GetMask("Ship", "Planet", "Satelite", "Obstacle");
    }

    public void TakeDamage(float damage, ShipController agressor)
    {
        _currentHP -= Mathf.Max(0.0f, damage - Armor);
        if (_currentHP <= 0.0f)
        {
            SetState(new ShipStates.Die());
        }
        else
        {
            SetState(new ShipStates.Attack(agressor, StateMachine.GetCurrentState()));
        }
    }

    public bool IsAlive()
    {
        return _currentHP > 0.0f;
    }

    public virtual void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, AttackRange, _shootRaycastLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider == null)
            {
                return;
            }

            ShipController enemy = hit.collider.GetComponent<ShipController>();
            if (enemy == null)
            {
                return;
            }

            enemy.TakeDamage(AttackDamage, this);
        }
    }
}
