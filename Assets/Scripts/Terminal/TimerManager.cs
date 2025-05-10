using UnityEngine;
using TMPro;
using System;

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float startTime = 60f;
    private float timer;
    public event Action OnTimerExpired;

    void Start() { timer = startTime; UpdateText(); }
    void Update()
    {
        if (timer <= 0f) return;
        timer -= Time.deltaTime;
        UpdateText();
        if (timer <= 0f) OnTimerExpired?.Invoke();
    }
    public void ResetTimer() { timer = startTime; UpdateText(); }
    private void UpdateText() { timerText.text = Mathf.CeilToInt(timer).ToString(); }
}