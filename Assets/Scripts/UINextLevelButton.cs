using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UINextLevelButton : MonoBehaviour
{
    private LevelManager _levelManager;
    private Button _button;

    private void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(GoToNextLevel);
    }

    private void GoToNextLevel()
    {
        _levelManager.GoToNextLevel();
    }
}
