using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIRestartButton : MonoBehaviour
{
    private Button _button;

	void Start ()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(RestartLevel);
	}

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
