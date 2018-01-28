using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIStartGameButton : MonoBehaviour
{
    [SerializeField]
    private string _firstLevelName;

    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(LoadFirstLevel);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(_firstLevelName);
    }
}
