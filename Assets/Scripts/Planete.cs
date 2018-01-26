using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planete : MonoBehaviour {

    private float _rotationParameter = 5.0f;
    private ArrayList<Satellite> _satellistes = new ArrayList<Satellite>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.gameObject.transform.Rotate(new Vector3(0f, 0f, Time.deltaTime * _rotationParameter));
	}
}
