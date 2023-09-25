using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Score : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private GameManager gameManager;
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void Update()
    {
        scoreText.text = $"Objects Sliced: {gameManager.score}";
    }
}
