using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float timer = 120;
    private void Start()
    {
        timer = 120;
    }

    private void Update()
    {
        if (timer > 1)
        {
            timer -= Time.deltaTime;
            UpdateCountdownText();
        }
    }

    private void UpdateCountdownText()
    {
        int seconds = Mathf.FloorToInt(timer % 60);
        int minutes = Mathf.FloorToInt(timer / 60);
        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
