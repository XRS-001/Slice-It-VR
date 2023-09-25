using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SelectionGrab : XRSimpleInteractable
{
    public AudioSource sceneAudio;
    public GameManager gameManager;
    public GameObject weaponPrefab;
    public GameObject menu;
    public GameObject waveMenu;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        sceneAudio.Play();
        gameManager.SpawnItem(weaponPrefab);
        menu.SetActive(false);
        waveMenu.SetActive(true);
        base.OnSelectEntered(args);
    }
}
