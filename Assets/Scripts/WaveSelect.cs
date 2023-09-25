using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSelect : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject cannonParent;
    public Timer timer;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    public void StartWave(string difficulty)
    {
        timer.timer = 120;
        gameManager.StartWave(difficulty, cannonParent);
    }
}
