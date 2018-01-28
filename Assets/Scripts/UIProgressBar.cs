using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProgressBar : MonoBehaviour
{
    private LevelManager _levelManager;
    private Image _barImage;

    private void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        _barImage = GetComponent<Image>();
        _barImage.fillAmount = 0;
    }

    private void Update()
    {
       _barImage.fillAmount = _levelManager.GetProgress() * 0.01f;
    }
}
