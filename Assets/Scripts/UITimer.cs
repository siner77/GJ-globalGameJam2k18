using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour
{
    [SerializeField]
    private string _prefix;

    private LevelManager _levelManager;
    private Text _timerText;

    private void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        _timerText = GetComponent<Text>();
    }

    private void Update()
    {
        _timerText.text = _prefix + GetTimeLeftString(_levelManager.GetTimeLeft());
    }

    private string GetTimeLeftString(float timeLeft)
    {
        int minutes = (int)(timeLeft / 60.0f);
        int minutesToSeconds = minutes * 60;
        int seconds = (int)(timeLeft - (float)minutesToSeconds);
        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
