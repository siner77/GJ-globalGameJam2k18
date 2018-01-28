using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    [SerializeField]
    private float _xScaleMultiplier;

    private ShipController _shipController;
    private SpriteRenderer _healthBar;

    private void Start()
    {
        _shipController = GetComponentInParent<ShipController>();
    }

    private void Update()
    {
        _healthBar.transform.localScale = new Vector3(_shipController.GetHealthPercent() * _xScaleMultiplier, _healthBar.transform.localScale.y, _healthBar.transform.localScale.z);
    }
}
