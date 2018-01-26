using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planete : MonoBehaviour {

    private float _rotationParameter = 5.0f;
    private List<Satellite> _satellistes = new List<Satellite>();


	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        this.gameObject.transform.Rotate(Vector3.forward * Time.deltaTime * _rotationParameter));
    }
}
