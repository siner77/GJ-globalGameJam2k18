using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameOverPanel : MonoBehaviour
{
    private LevelManager _levelManager;

    private void Start()
    {
         _levelManager = FindObjectOfType<LevelManager>();
        _levelManager.OnLoose += OnLoose;
        gameObject.SetActive(false);
    }

    private void OnLoose()
    {
        gameObject.SetActive(true);
    }
}
