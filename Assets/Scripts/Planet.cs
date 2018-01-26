using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    private float _rotationParameter = 5.0f;
    private List<Satellite> _satellistes = new List<Satellite>();
    private List<EnemyShipController> _enemyShip = new List<EnemyShipController>();
    private List<ShipController> _shipController = new List<ShipController>();


	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        this.gameObject.transform.Rotate(Vector3.forward * Time.deltaTime * _rotationParameter);
    }
}
