using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    [SerializeField]
    private LayerMask _layerToSelect;

    private Vector3 _previousFrameMousePosition;
    private GameObject _selected;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 mousePosition = Input.mousePosition;
        if (_selected != null)
        {
            float prevAngleDegrees = (mousePosition.y - _previousFrameMousePosition.y) * Mathf.Rad2Deg * Time.deltaTime * 0.8f;
            _selected.transform.Rotate(Vector3.up, prevAngleDegrees);
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out hitInfo, float.MaxValue, _layerToSelect);
            if (hit)
            {
                _selected = hitInfo.collider.transform.parent.gameObject;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            _selected = null;
        }
        _previousFrameMousePosition = mousePosition;
    }
}
