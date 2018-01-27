using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ShipStates
{
    public class GoToPlanet : IState<ShipController>
    {
        private static int CURRENT_MOVING_SHIPS = 1;

        private Planet _targetPlanet;
        private Spot _targetSpot;
        private Vector3 _offset;

        private IState<ShipController> _afterReachState;

        public GoToPlanet(Planet targetPlanet, IState<ShipController> afterReachState)
        {
            _targetPlanet = targetPlanet;
            _afterReachState = afterReachState;
        }

        public void OnEnter(ShipController controller)
        {
            if (_targetSpot == null)
            {
                if (controller.IsEnemy())
                {
                    _targetSpot = _targetPlanet.GetClosestUnusedEnemySpot(controller.transform.position);
                }
                else
                {
                    _targetSpot = _targetPlanet.GetClosestUnusedAllySpot(controller.transform.position);
                }

                controller.SetUsedSpot(_targetSpot);
            }

            controller.NavMeshAgent.avoidancePriority = 50 + CURRENT_MOVING_SHIPS;
            CURRENT_MOVING_SHIPS += 1;

            controller.NavMeshAgent.SetDestination(_targetSpot.SpotTransform.position);
        }

        public void OnExit(ShipController controller)
        {
            controller.NavMeshAgent.avoidancePriority = 50;
            CURRENT_MOVING_SHIPS -= 1;
        }

        public void OnUpdate(ShipController controller)
        {
            controller.NavMeshAgent.SetDestination(_targetSpot.SpotTransform.position);
            if (HasReachedPosition(controller))
            {
                controller.SetState(_afterReachState);
            }
        }

        private bool HasReachedPosition(ShipController controller)
        {
            return controller.NavMeshAgent.remainingDistance <= controller.NavMeshAgent.stoppingDistance && controller.NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete;
        }
    }

    public class Defend : IState<ShipController>
    {
        private Planet _target;

        public Defend(Planet target)
        {
            _target = target;
        }

        public void OnEnter(ShipController controller)
        {
            controller.transform.parent = _target.transform;
            controller.NavMeshAgent.enabled = false;

            EnemyShipController enemy = _target.GetNearestShip(controller.transform.position);
            if(enemy != null)
            {
                controller.SetState(new Attack(enemy, this));
                return;
            }
        }

        public void OnExit(ShipController controller)
        {
            if(controller.StateMachine.NextState == null || controller.StateMachine.NextState.GetType() != typeof(Attack))
            {
                controller.transform.parent = null;
            }
            controller.NavMeshAgent.enabled = true;
        }

        public void OnUpdate(ShipController controller)
        {
            EnemyShipController enemy = _target.GetNearestShip(controller.transform.position);
            if (enemy != null)
            {
                controller.SetState(new Attack(enemy, this));
                return;
            }
            
            Vector3 targetForward = (controller.transform.position - _target.transform.position);
            targetForward.y = 0.0f;

            controller.RotateTowards(targetForward.normalized);
        }
    }

    public class Attack : IState<ShipController>
    {
        private enum AttackState
        {
            TRY_SHOOT,
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
            _attackState = AttackState.TRY_SHOOT;
            controller.NavMeshAgent.enabled = false;
        }

        public void OnExit(ShipController controller)
        {
            controller.NavMeshAgent.enabled = true;
        }

        public void OnUpdate(ShipController controller)
        {
            if (!_enemy.IsAlive())
            {
                controller.SetState(_stateAfterEnemyDeath);
                return;
            }

            Vector3 targetForward = (_enemy.transform.position - controller.transform.position);
            targetForward.y = 0.0f;

            controller.RotateTowards(targetForward.normalized);

            if (!controller.IsTargetInSight(_enemy.gameObject))
            {
                return;
            }

            if (_attackState == AttackState.TRY_SHOOT)
            {
                UpdateShoot(controller);
            }
            else if (_attackState == AttackState.WAIT_AFTER_ATTACK)
            {
                UpdateWaitAfterAttack(controller);
            }
        }

        private void UpdateShoot(ShipController controller)
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

[RequireComponent(typeof(NavMeshAgent))]
public class ShipController : StateMachineController<ShipController>
{
    public float AttackCooldown = 3.0f;
    public float AfterAttackWaitTime = 1.0f;
    public float AttackDamage = 1.0f;
    public float Armor = 0.0f;
    public float FlySpeed = 3.0f;
    public float RotateSpeed = 120.0f;
    [SerializeField]
    protected float _maxHP = 1.0f;
    [SerializeField]
    protected Transform _raycastOrigin;

    protected Spot _usedSpot;
    protected float _currentHP;
    protected int _shootRaycastLayerMask;
    protected int _sightObstacleLayerMask;

    public NavMeshAgent NavMeshAgent
    {
        get;
        protected set;
    }

    private void OnEnable()
    {
        _currentHP = _maxHP;
        _shootRaycastLayerMask = LayerMask.GetMask("Ship", "Planet", "Satelite", "Obstacle");
        _sightObstacleLayerMask = LayerMask.GetMask("Planet", "Obstacle");
        NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.angularSpeed = RotateSpeed;
        NavMeshAgent.speed = FlySpeed;
    }

    public void TakeDamage(float damage, ShipController agressor)
    {
        _currentHP -= Mathf.Max(0.0f, damage - Armor);
        if (_currentHP <= 0.0f)
        {
            OnDeath();
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
        if (Physics.Raycast(_raycastOrigin.transform.position, transform.forward, out hit, float.MaxValue, _shootRaycastLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider == null)
            {
                return;
            }

            ShipController enemy = hit.collider.GetComponentInParent<ShipController>();
            if (enemy == null)
            {
                return;
            }

            enemy.TakeDamage(AttackDamage, this);
        }
    }

    public bool IsTargetInSight(GameObject target)
    {
        RaycastHit hit;
        bool obscured = Physics.Raycast(transform.position, (target.transform.position - transform.position).normalized, out hit, float.MaxValue, _shootRaycastLayerMask, QueryTriggerInteraction.Ignore);

        if(obscured)
        {
            obscured = (hit.collider.gameObject.layer & _shootRaycastLayerMask) != 0 || hit.collider.gameObject != target;
        }

        return !obscured;
    }

    public void RotateTowards(Vector3 forward)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward), RotateSpeed * Time.deltaTime);
    }

    public virtual bool IsEnemy()
    {
        return false;
    }

    public void SetUsedSpot(Spot spot)
    {
        spot.IsUsed = true;
        _usedSpot = spot;
    }

    protected virtual void OnDeath()
    {
        if(_usedSpot != null)
        {
            _usedSpot.IsUsed = false;
        }
    }
}
