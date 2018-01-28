using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWinPanel : MonoBehaviour
{
    private LevelManager _levelManager;

    private void Start()
    {
         _levelManager = FindObjectOfType<LevelManager>();
        _levelManager.OnWin += OnWin;
        gameObject.SetActive(false);
    }

    private void OnWin()
    {
        gameObject.SetActive(true);
    }
}
