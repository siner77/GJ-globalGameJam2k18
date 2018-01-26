using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipStates
{
    public class GoTo : IState<ShipController>
    {
        public GoTo(/*Planet target*/)
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
        public Defend(/*Planet planet*/)
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
        private ShipController _enemy;
        private float _attackTimer;

        public Attack(ShipController enemy/*, Planet planetToDefend*/)
        {
            _enemy = enemy;
        }

        public void OnEnter(ShipController controller)
        {
            if (!_enemy.IsAlive())
            {
                controller.SetState(new Defend());
                return;
            }

            _attackTimer = 0.0f;
        }

        public void OnExit(ShipController controller)
        {

        }

        public void OnUpdate(ShipController controller)
        {
            if(!_enemy.IsAlive())
            {
                controller.SetState(new Defend());
                return;
            }

            _attackTimer += Time.deltaTime;
            if(_attackTimer >= controller.AttackCooldown)
            {
                controller.Shoot();
                _attackTimer = 0.0f;
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
    public float AttackDamage = 1.0f;
    public float Armor = 0.0f;
    [SerializeField]
    protected float _maxHP = 1.0f;

    private float _currentHP;
    private int _shootRaycastLayerMask;

    protected virtual void OnEnable()
    {
        _currentHP = _maxHP;
        _shootRaycastLayerMask = LayerMask.GetMask("Ship", "Planet", "Obstacle");
    }

    public void TakeDamage(float damage)
    {
        _currentHP -= Mathf.Max(0.0f, damage - Armor);
        if(_currentHP <= 0.0f)
        {
            SetState(new ShipStates.Die());
        }
    }

    public bool IsAlive()
    {
        return _currentHP > 0.0f;
    }

    public void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, float.MaxValue, _shootRaycastLayerMask, QueryTriggerInteraction.Ignore))
        {
            if(hit.collider == null)
            {
                return;
            }

            ShipController enemy = hit.collider.GetComponent<ShipController>();
            if(enemy == null)
            {
                return;
            }

            enemy.TakeDamage(AttackDamage);
        }
    }
}
