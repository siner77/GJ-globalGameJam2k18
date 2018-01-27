using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour {

    private float _progress = 0.0f;
    private float _progressLimit = 100.0f;
    [SerializeField]
    private float _progressModifier = 1.0f;
    private float _gameTime = 0.0f;
    [SerializeField]
    private float _gameTimeLimit = 300.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        _gameTime += Time.deltaTime;
        if(_gameTime > _gameTimeLimit)
        {
            Debug.Log("game over");
            Debug.Break();
        }
	}

    public void ImproveProgress()
    {
        _progress += Time.deltaTime * _progressModifier;
        Debug.Log(string.Format("Progress = {0}", _progress));
        if(_progress > _progressLimit)
        {
            Debug.Log("win, gz");
            Debug.Break();
        }
    }


}
